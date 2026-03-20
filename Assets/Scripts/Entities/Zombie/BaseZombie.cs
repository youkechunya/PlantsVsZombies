using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public abstract class BaseZombie : BaseCharacter
{
    public Action<GameObject> LastZombieDieEvent;

    private bool waitingStart;
    public bool WaitingStart
    {
        get { return waitingStart; }
        set
        {
            waitingStart = value;
            if (waitingStart)
            {
                animator.SetFloat("Moving", -3);
                int randomIdle = UnityEngine.Random.Range(0, 2);
                animator.SetFloat("Idle", randomIdle);
            }
        }
    }

    [Header("检测设置")]
    /// <summary>
    /// 攻击距离
    /// </summary>
    protected float detectRange = 0.5f;
    /// <summary>
    /// 植物图层
    /// </summary>
    public LayerMask plantLayer;

    // 内部状态
    protected ZombieState currentState = ZombieState.Walking;
    // 当前攻击目标
    protected BasePlant targetPlant;
    /// <summary>
    /// 死亡标记
    /// </summary>
    private bool isDead;
    /// <summary>
    /// 被烧死的标记
    /// </summary>
    public bool charred;

    /// <summary>
    /// 断手血量
    /// </summary>
    public int lossArmHP;
    /// <summary>
    /// 死亡血量
    /// </summary>
    public int deadHP;
    /// <summary>
    /// 掉头后每滴血失去的所需时间
    /// </summary>
    private readonly float rateOfBloodLoss = 0.032f;
    /// <summary>
    /// 判断僵尸是否断手
    /// </summary>
    bool lossArm;
    /// <summary>
    /// 手臂预制体
    /// </summary>
    [SerializeField] private GameObject arm;
    /// <summary>
    /// 手臂位置
    /// </summary>
    [SerializeField] private Transform armPosition;
    private GameObject lossArmObj;

    /// <summary>
    /// 是否有防具
    /// </summary>
    public Armor armor;
    /// <summary>
    /// 所属波次
    /// </summary>
    public int belongingWave;
    /// <summary>
    /// 生成器
    /// </summary>
    public ZombieSpawner spawner;

    /// <summary>
    /// 攻击间隔：实际为每 0.04 秒计算一次，每次4点血
    /// </summary>
    protected readonly float attackDuration = 0.04f * 12.5f;
    protected float currentAttackTime = 0;

    [SerializeField] private ParticleSystem headDrop;
    /// <summary>
    /// 头的位置
    /// </summary>
    [SerializeField] private Transform headPosition;
    /// <summary>
    /// 头的图像
    /// </summary>
    [SerializeField] private Sprite headSprite;

    /// <summary>
    /// 透明度协程
    /// </summary>
    private Coroutine deadAlphaCoroutine;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        armor = GetComponent<Armor>();
        // 重置标记
        charred = false;
        isDead = false;
        UpdateColor(new Color(1, 1, 1, 1));
        ChangeState(currentState = ZombieState.Walking);
    }

    protected virtual void Update()
    {
        currentAttackTime = Mathf.Clamp(currentAttackTime + Time.deltaTime, 0, attackDuration);
        switch (currentState)
        {
            case ZombieState.Walking:
                UpdateWalking();
                break;
            case ZombieState.Attacking:
                UpdateAttacking();
                break;
            case ZombieState.Dead:
                UpdateDead();
                break;
            case ZombieState.Charred:
                UpdateCharred();
                break;
        }
    }

    public void ChangeState(ZombieState state)
    {
        currentState = state;
        animator.SetBool("Attacking", currentState == ZombieState.Attacking);
    }

    // 各状态更新方法（子类可重写）
    protected abstract void UpdateWalking();
    protected abstract void UpdateAttacking();
    protected void UpdateDead()
    {
        if (isDead)
            return;

        isDead = true;
        headDrop.Play();
        animator.SetTrigger("Dead");
    }

    protected void UpdateCharred()
    {
        if (isDead)
            return;

        isDead = true;
        animator.SetTrigger("BeCharred");
    }

    public override void TakeDamage(int damage, Vector2 hitPoint, bool ignoreArmor = false)
    {
        base.TakeDamage(damage, hitPoint, ignoreArmor);
        // 如果有防具且伤害无法无视防具
        if (armor != null && !ignoreArmor)
        {
            // 优先对防具计算伤害
            armor.TakeDamage(damage, hitPoint);
        }
        else
        {
            if (isDead)
                return;

            // 对波长的影响
            if (spawner != null)
            {
                if (belongingWave == spawner.wave)
                {
                    spawner.currentWaveHP -= Mathf.Min(damage, Health);
                }
            }
            Health -= damage;
            // 如果是僵尸没有被灰烬秒杀且未断手
            if (!charred && !lossArm)
            {
                // 如果血量小于断手血量
                if (Health <= lossArmHP)
                {
                    // 触发断手
                    lossArm = true;
                    StartCoroutine(LossArm());
                }
            }
            // 如果血量小于死亡血量
            if (Health <= deadHP)
            {
                // 触发死亡
                Dead();
            }
        }
    }

    public IEnumerator LossArm()
    {
        animator.SetFloat("LossArm", 1);
        // 播放音效
        AudioManager.Instance.sfxPool.PlaySFX("loadingbar_flower");
        // 透明度临时变量
        float a = 1;
        // 获取手臂预制体
        lossArmObj = ObjectPool.Instance.GetFromPool(arm.name);
        lossArmObj.transform.position = armPosition.position;
        // 获取手臂的 SpriteRenderer 组件
        SpriteRenderer armSprite = lossArmObj.GetComponent<SpriteRenderer>();
        armSprite.color = new Color(1, 1, 1, a);
        // 掉落手臂
        lossArmObj.transform.DOMoveY(lossArmObj.transform.position.y - 0.3f, 1f);
        lossArmObj.transform.DORotate(new Vector3(0, 0, lossArmObj.transform.rotation.z + 90), 1f);
        yield return new WaitForSeconds(1f);
        // 断手消失
        armSprite.DOColor(Color.clear, 1f).WaitForCompletion();
        GameEvents.OnReturnToPool(lossArmObj.name, lossArmObj);
    }

    protected override void Dead()
    {
        if (isDead)
            return;

        StartCoroutine(DeadIEnumerator());
    }

    private IEnumerator DeadIEnumerator()
    {
        spawner.zombieList.Remove(gameObject.GetInstanceID());
        spawner.SetZombieRemainText();
        // 如果不是被灰烬植物秒杀
        if (!charred)
        {
            // 播放掉头音效
            AudioManager.Instance.sfxPool.PlaySFX("limbs_pop");
            ChangeState(ZombieState.Dead);
        }
        else
        {
            if (armor != null)
            {
                if (spawner != null)
                {
                    if (belongingWave == spawner.wave)
                    {
                        armor.BeCharred();
                    }
                }
            }
            ChangeState(ZombieState.Charred);
            yield return new WaitForSeconds(3f);
            LastZombieDieEvent?.Invoke(gameObject);
            GameEvents.OnReturnToPool(gameObject.name, gameObject);
            yield break;
        }

        // 当血量到达掉头血量
        while (Health > 0)
        {
            Health -= 1;
            yield return new WaitForSeconds(rateOfBloodLoss);
        }
        deadAlphaCoroutine = StartCoroutine(DeadAlpha());
        yield return new WaitForSeconds(1.5f);
        LastZombieDieEvent?.Invoke(gameObject);
        if (lossArmObj.activeInHierarchy)
            GameEvents.OnReturnToPool(lossArmObj.name, lossArmObj);
        if (deadAlphaCoroutine != null)
            StopCoroutine(deadAlphaCoroutine);
        GameEvents.OnReturnToPool(gameObject.name, gameObject);
    }

    private IEnumerator DeadAlpha()
    {
        float a = 1;
        Color color = propertyBlocks.GetColor(ColorID);
        color.a = a;
        while (a > 0)
        {
            UpdateColor(color);
            a -= Time.deltaTime;
            color.a = a;
            yield return null;
        }
    }
}
