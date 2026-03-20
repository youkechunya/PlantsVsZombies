using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BaseBoss : BaseCharacter
{
    [SerializeField] protected Slider healthRemain;
    protected Collider2D coll;

    protected override void Awake()
    {
        base.Awake();
        coll = GetComponent<Collider2D>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(AppearAnimation());
    }

    private IEnumerator AppearAnimation()
    {
        healthRemain.value = 0;
        coll.enabled = false;
        while (healthRemain.value < 1)
        {
            healthRemain.value += Time.deltaTime / 2;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        coll.enabled = true;
    }

    public override void TakeDamage(int damage, Vector2 hitPoint, bool ignoreArmor = false)
    {
        Health -= damage;
        if (Health - damage < 0)
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(FlashIEnumerator());
        healthRemain.value = (float)Health / (float)maxHealth;
    }
}
