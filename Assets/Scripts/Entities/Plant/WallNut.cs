using UnityEngine;

public class WallNut : BasePlant
{
    [SerializeField] private Sprite originAppearance;
    private int criticalHP1;
    [SerializeField] private Sprite criticalAppearance1;
    private int criticalHP2;
    [SerializeField] private Sprite criticalAppearance2;

    protected override void Awake()
    {
        base.Awake();
        criticalHP1 = maxHealth * 2 / 3;
        criticalHP2 = maxHealth / 3;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (spriteRenderers.Length == 0)
        {
            Debug.LogError("Õº∆¨Œ¥º”‘ÿ£°£°£°");
            return;
        }
        spriteRenderers[0].sprite = originAppearance;
        Health = maxHealth;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<BaseZombie>() != null)
            animator.SetBool("beAttacking", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<BaseZombie>() != null)
            animator.SetBool("beAttacking", false);
    }

    protected override void UpdateAction() { }

    protected override void UpdateIdle() { }

    protected override void OnHealthChanged()
    {
        if (spriteRenderers.Length == 0)
        {
            Debug.LogError("Õº∆¨Œ¥º”‘ÿ£°£°£°");
            return;
        }
        if (Health <= criticalHP2)
        {
            spriteRenderers[0].sprite = criticalAppearance2;
        }
        else if (Health <= criticalHP1)
        {
            spriteRenderers[0].sprite = criticalAppearance1;
        }
        else
        {
            spriteRenderers[0].sprite = originAppearance;
        }
    }
}
