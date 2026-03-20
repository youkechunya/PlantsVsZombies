using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    protected Rigidbody2D rb;

    [SerializeField] protected readonly float moveSpeed = 5f;
    [SerializeField] protected int damage = 20;

    /// <summary>
    /// 上一帧的位置
    /// </summary>
    protected Vector2 lastPosition;
    /// <summary>
    /// 僵尸层级
    /// </summary>
    [SerializeField] protected LayerMask zombieMask;

    [SerializeField] protected AudioClip[] soundClips;

    [SerializeField] protected GameObject hitParticle;

    private GameObject shooter;
    public GameObject Shooter
    {
        get => shooter;
        set
        {
            shooter = value;
            lastPosition = Shooter.transform.position;
        }
    }

    protected virtual void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    protected virtual void OnEnable()
    {
        rb.velocity = moveSpeed * transform.right;
    }

    protected virtual void Update()
    {
        if (transform.position.x > 10f)
            GameEvents.OnReturnToPool(transform.parent.name, transform.parent.gameObject);

        Vector2 currentPosition = transform.position;
        Vector2 direction = (currentPosition - lastPosition).normalized;
        float distance = Vector2.Distance(lastPosition, currentPosition);
        RaycastHit2D hit = Physics2D.Raycast(lastPosition, direction, distance, zombieMask);
        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<BaseZombie>() != null || hit.collider.GetComponent<BaseBoss>() != null)
                HitEvent(hit.collider.GetComponent<BaseCharacter>());
        }
        lastPosition = currentPosition;
    }

    protected virtual void OnDisable()
    {
        // 回收时清零速度，避免下次启用时残留
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    protected virtual void HitEvent(BaseCharacter beHitObject)
    {
        GameObject hitParticle = ParticlePool.Instance.GetFromPool(this.hitParticle.name);
        if (hitParticle != null)
            hitParticle.transform.position = transform.position;
        else
        {
            hitParticle = ObjectPool.Instance.GetFromPool(this.hitParticle.name);
            hitParticle.transform.position = transform.position;
        }
        int randomSound = Random.Range(0, soundClips.Length);
        AudioManager.Instance.sfxPool.PlaySFX(soundClips[randomSound].name);
        beHitObject.TakeDamage(damage, transform.position);
        GameEvents.OnReturnToPool(transform.parent.name, transform.parent.gameObject);
    }
}
