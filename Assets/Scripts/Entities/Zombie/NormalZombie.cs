using UnityEngine;

public class NormalZombie : BaseZombie
{
    [Header("行走动画")]
    public AnimationClip walkAnimation;

    int randomEatingSound;
    int randomMovingState;

    /// <summary>
    /// 生成行
    /// </summary>
    public int spawnLine;

    protected override void OnEnable()
    {
        base.OnEnable();
        randomEatingSound = Random.Range(1, 4);
        do
        {
            randomMovingState = Random.Range(-1, 2);
        } while (randomMovingState == 0);
        animator.SetFloat("Moving", randomMovingState);
        animator.SetFloat("LossArm", -1);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void UpdateWalking()
    {
        // 检查前方是否有植物
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, detectRange, plantLayer);
        if (hit.collider != null)
        {
            targetPlant = hit.collider.GetComponent<BasePlant>();
            ChangeState(ZombieState.Attacking);
        }
    }

    public void FallSound()
    {
        int randomSound = Random.Range(1, 3);
        AudioManager.Instance.sfxPool.PlaySFX("zombie_falling_" + randomSound);
    }

    protected override void UpdateAttacking()
    {
        if (targetPlant == null || !targetPlant.gameObject.activeInHierarchy)
        {
            targetPlant = null;
            ChangeState(ZombieState.Walking);
            return;
        }

        // 检查植物是否还在前方攻击范围内
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, detectRange, plantLayer);
        bool isPlantStillInRange = hit.collider != null && hit.collider.GetComponent<BasePlant>() == targetPlant;

        if (!isPlantStillInRange)
        {
            // 如果植物还在，但已不在前方
            targetPlant = null;
            ChangeState(ZombieState.Walking);
            return;
        }

        if (currentAttackTime >= attackDuration)
        {
            AudioManager.Instance.sfxPool.PlaySFX("chomp" + randomEatingSound);
            currentAttackTime = 0;

            // 造成伤害
            targetPlant.GetComponent<BaseCharacter>().TakeDamage(50, transform.position);

            // 攻击后立即检查植物是否被摧毁
            if (!targetPlant.gameObject.activeInHierarchy)
            {
                targetPlant = null;
                ChangeState(ZombieState.Walking);
            }
        }
    }
}
