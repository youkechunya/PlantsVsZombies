using System.Collections;
using UnityEngine;

public class Boom : MonoBehaviour
{
    /// <summary>
    /// 是否为灰烬伤害
    /// </summary>
    public bool isCharred = true;
    public int damage = 1800;
    private float boomRadius = 1.4f;
    /// <summary>
    /// 伤害范围（请在最后修改这个值）
    /// </summary>
    public float BoomRadius
    {
        get => boomRadius;
        set
        {
            boomRadius = value;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, boomRadius, zombieLayer);
            foreach (Collider2D collider in colliders)
            {
                BaseZombie zombie = collider.GetComponent<BaseZombie>();
                zombie.charred = isCharred;
                zombie.spawner.currentWaveHP -= damage;
                zombie.TakeDamage(damage, transform.position, isCharred);
                StartCoroutine(ReturnToPool());
            }
        }
    }

    [SerializeField] private LayerMask zombieLayer;

    IEnumerator ReturnToPool()
    {
        yield return null;
        GameEvents.OnReturnToPool(gameObject.name, gameObject);
    }
}
