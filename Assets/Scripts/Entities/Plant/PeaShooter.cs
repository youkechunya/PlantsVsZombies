using UnityEngine;

/// <summary>
/// 豌豆射手
/// </summary>
public class PeaShooter : BasePlant
{
    /// <summary>
    /// 子弹
    /// </summary>
    [SerializeField] private GameObject pea;
    /// <summary>
    /// 发射点
    /// </summary>
    [SerializeField] private Transform shootPoint;
    /// <summary>
    /// 僵尸层级
    /// </summary>
    [SerializeField] private LayerMask zombieLayer;
    /// <summary>
    /// 下次射击间隔
    /// </summary>
    [SerializeField] private float shootDuration = 1.4f;
    private readonly float shootDurationMin = 1.36f;
    private readonly float shootDurationMax = 1.5f;
    private bool shootCompleted = true;
    /// <summary>
    /// 一次射击数量
    /// </summary>
    private int shootCount = 6;

    protected override void Update()
    {
        base.Update();
        if (shootCompleted)
            shootDuration -= Time.deltaTime;
    }

    public void Attack()
    {
        float x = shootPoint.position.x;
        for (int i = 0; i < shootCount; i++)
        {
            // 发射豌豆
            GameObject pea = ObjectPool.Instance.GetFromPool(this.pea.name);
            pea.transform.position = new Vector2(x,shootPoint.position.y);
            pea.GetComponentInChildren<BaseProjectile>().Shooter = gameObject;
            x -= 0.3f;
        }
        // 播放声音
        int randomSound = Random.Range(1, 3);
        AudioManager.Instance.sfxPool.PlaySFX("throw" + randomSound);
        // 刷新冷却时间
        shootDuration = Random.Range(shootDurationMin, shootDurationMax);
    }

    protected override void UpdateAction() { }

    public void ShootCompleted()
    {
        shootCompleted = true;
        animator.SetBool("Action", false);
        ChangeState(PlantState.Idle);
    }

    protected override void UpdateIdle()
    {
        // 检测前方是否有僵尸
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 8f, zombieLayer);
        if (hit.collider != null)
        {
            if (shootDuration > 0)
                return;

            shootCompleted = false;
            animator.SetBool("Action", true);
            ChangeState(PlantState.Action);
        }
    }
}
