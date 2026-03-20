using System.Collections;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    /// <summary>
    /// 动画机
    /// </summary>
    protected Animator animator;
    protected const float ANIMATOR_ORIGIN_SPEED = 1f;
    protected const float ANIMATOR_DECELERATION_SPEED = 0.5f;

    /// <summary>
    /// 最大血量
    /// </summary>
    [SerializeField] protected int maxHealth;
    /// <summary>
    /// 剩余血量
    /// </summary>
    [SerializeField] private int health;
    public int Health
    {
        get => health;
        protected set
        {
            health = value;
            OnHealthChanged();
        }
    }


    /// <summary>
    /// 亮度
    /// </summary>
    protected float brightness = 1f;

    // PropertyBlock只在渲染时临时修改，不占用额外显存
    /// <summary>
    /// 材质属性块，每个物体都有一个独立的属性块，互不影响
    /// </summary>
    protected MaterialPropertyBlock propertyBlocks;
    /// <summary>
    /// 渲染器
    /// </summary>
    [SerializeField] protected SpriteRenderer[] spriteRenderers;
    protected static readonly int BrightnessID = Shader.PropertyToID("_Brightness");
    protected static readonly int ColorID = Shader.PropertyToID("_Color");

    /// <summary>
    /// 闪烁协程
    /// </summary>
    private Coroutine flashCoroutine;
    /// <summary>
    /// 减速协程
    /// </summary>
    private Coroutine slowedCoroutine;

    [Header("视觉效果")]
    [SerializeField] private float flashBrightness = 3f;
    [SerializeField] private float flashSpeed = 2f;
    [SerializeField] private Color slowedColor = new Color(0.27f, 0.35f, 1f, 1f);

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        propertyBlocks = new MaterialPropertyBlock();
        CacheRenderers();
    }

    protected virtual void OnEnable()
    {
        animator.speed = ANIMATOR_ORIGIN_SPEED;
        Health = maxHealth;
        UpdateLight(1);
    }

    /// <summary>
    /// 缓存所有 SpriteRenderer 和对应的 PropertyBlock
    /// </summary>
    protected virtual void CacheRenderers()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true); // 包含 inactive

        if (spriteRenderers.Length == 0)
        {
            Debug.LogWarning($"[{nameof(BaseCharacter)}] {gameObject.name} 上找不到任何 SpriteRender 组件");
            return;
        }
    }

    /// <summary>
    /// 重置视觉状态（血量满、亮度正常、颜色默认）
    /// </summary>
    protected virtual void ResetVisuals()
    {
        brightness = 1f;
        UpdateLight(brightness);
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="hitPoint"></param>
    public virtual void TakeDamage(int damage, Vector2 hitPoint, bool ignoreArmor = false)
    {
        if (Health - damage > 0)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashIEnumerator());
        }
    }

    /// <summary>
    /// 死亡行为
    /// </summary>
    protected virtual void Dead() { }

    /// <summary>
    /// 闪烁协程
    /// </summary>
    /// <returns></returns>
    protected IEnumerator FlashIEnumerator()
    {
        brightness = flashBrightness;
        while (brightness > 1)
        {
            UpdateLight(brightness);
            brightness -= Time.deltaTime * flashSpeed;
            yield return null;
        }
        brightness = 1f;
        UpdateLight(brightness);
    }

    public void Slowed(float decelerationDuration)
    {
        if (slowedCoroutine != null)
            StopCoroutine(slowedCoroutine);
        slowedCoroutine = StartCoroutine(SlowedIEnumerator(decelerationDuration));
    }

    /// <summary>
    /// 减速协程
    /// </summary>
    /// <param name="decelerationDuration"></param>
    /// <returns></returns>
    private IEnumerator SlowedIEnumerator(float decelerationDuration)
    {
        float timer = 0f;
        animator.speed = ANIMATOR_DECELERATION_SPEED;
        UpdateColor(slowedColor);
        while (timer < decelerationDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        animator.speed = ANIMATOR_ORIGIN_SPEED;
        UpdateColor(new Color(1, 1, 1, 1));
    }

    /// <summary>
    /// 设置材质属性块
    /// </summary>
    /// <param name="setter"></param>
    private void SetPropertyBlock(System.Action<MaterialPropertyBlock> setter)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            // 必须先获取 PropertyBlock
            spriteRenderers[i].GetPropertyBlock(propertyBlocks);
            // 修改对应数值
            setter(propertyBlocks);
            // 再进行应用
            spriteRenderers[i].SetPropertyBlock(propertyBlocks);
        }
    }

    /// <summary>
    /// 更新灯光
    /// </summary>
    /// <param name="brightness"></param>
    public void UpdateLight(float brightness)
    {
        SetPropertyBlock(block => block.SetFloat(BrightnessID, brightness));
    }

    /// <summary>
    /// 更新颜色
    /// </summary>
    /// <param name="color"></param>
    public void UpdateColor(Color color)
    {
        SetPropertyBlock(block => block.SetColor(ColorID, color));
    }

    protected virtual void OnHealthChanged() { }
}
