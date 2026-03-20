using System.Collections;
using UnityEngine;

public class PotatoMine : BasePlant
{
    bool ready;

    // 获取灯的图片组件
    [SerializeField] private SpriteRenderer lightSpriteRender;
    // 闪烁前图片
    [SerializeField] private Sprite originLight;
    // 闪烁后图片
    [SerializeField] private Sprite detectedLight;
    /// <summary>
    /// 僵尸层级
    /// </summary>
    [SerializeField] private LayerMask zombieLayer;
    /// <summary>
    /// 最长等待闪烁间隔
    /// </summary>
    private const float MAX_FLICKER_INTERVAL = 3;
    /// <summary>
    /// 最短等待闪烁间隔
    /// </summary>
    private const float MIN_FLICKER_INTERVAL = 0.4f;
    /// <summary>
    /// 闪烁间隔（即亮与暗之间的间隔）
    /// </summary>
    private const float FLICKER_DURATION = 0.25f;
    /// <summary>
    /// 下次等待闪烁间隔
    /// </summary>
    [SerializeField] private float nextFlickerInterval;
    /// <summary>
    /// 距离下次闪烁间隔
    /// </summary>
    [SerializeField] private float currentFlickerInterval = 1f;
    /// <summary>
    /// 是否正在闪烁
    /// </summary>
    [SerializeField] private bool isFlickering;
    /// <summary>
    /// 探测范围
    /// </summary>
    [SerializeField] private float detectRadius = 8f;
    /// <summary>
    /// 出土时间
    /// </summary>
    [SerializeField] private float growTime = 15f;

    private Coroutine flickerCoroutine;
    private Coroutine growCoroutine;

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetState();
        growCoroutine = StartCoroutine(WaitForGrow());
    }

    protected override void Update()
    {
        if (!ready)
            return;

        UpdateFlickerLogic();
    }

    private void OnDisable()
    {
        StopAllCoroutinesSafe();
    }

    /// <summary>
    /// 重置所有状态（对象池回收后重用）
    /// </summary>
    private void ResetState()
    {
        ready = false;
        isFlickering = false;
        currentFlickerInterval = MAX_FLICKER_INTERVAL;

        // 确保灯光初始状态
        if (lightSpriteRender != null)
            lightSpriteRender.sprite = originLight;
    }

    /// <summary>
    /// 更新闪烁逻辑
    /// </summary>
    private void UpdateFlickerLogic()
    {
        // 检测最近僵尸距离
        float nearestDistance = DetectNearestZombie();

        // 根据距离计算闪烁间隔（越近越快）
        float t = Mathf.Clamp01(nearestDistance / detectRadius);
        nextFlickerInterval = Mathf.Lerp(MIN_FLICKER_INTERVAL, MAX_FLICKER_INTERVAL, t);

        // 更新计时
        currentFlickerInterval -= Time.deltaTime;

        if (currentFlickerInterval <= 0 && !isFlickering)
        {
            flickerCoroutine = StartCoroutine(FlickerRoutine());
        }
    }

    /// <summary>
    /// 检测最近僵尸的距离（圆形范围）
    /// </summary>
    private float DetectNearestZombie()
    {
        // 使用 OverlapCircle 检测周围所有僵尸
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, zombieLayer);
        // 默认最大距离
        float minDist = detectRadius;

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            if (hit.GetComponent<BaseZombie>() != null || hit.CompareTag("Zombie"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                    minDist = dist;
            }
        }

        return minDist;
    }

    /// <summary>
    /// 闪烁协程
    /// </summary>
    private IEnumerator FlickerRoutine()
    {
        // 亮起
        lightSpriteRender.sprite = detectedLight;
        yield return new WaitForSeconds(FLICKER_DURATION);

        // 熄灭
        lightSpriteRender.sprite = originLight;

        currentFlickerInterval = nextFlickerInterval;
        isFlickering = false;
        flickerCoroutine = null;
    }

    IEnumerator WaitForGrow()
    {
        tag = "Plant";
        yield return new WaitForSeconds(growTime);
        tag = "Untagged";
        ready = true;
        animator.SetTrigger("ready");
    }

    private void StopAllCoroutinesSafe()
    {
        if (growCoroutine != null)
        {
            StopCoroutine(growCoroutine);
            growCoroutine = null;
        }
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!ready)
            return;

        if (collision.GetComponent<NormalZombie>() == null)
            return;

        GameObject boomParticle = ObjectPool.Instance.GetFromPool("PotatoBoom");
        boomParticle.transform.position = transform.position;
        GameObject boom = ObjectPool.Instance.GetFromPool("BoomTrigger");
        boom.transform.position = transform.position;
        boom.GetComponent<Boom>().isCharred = false;
        boom.GetComponent<Boom>().damage = 1800;
        boom.GetComponent<Boom>().BoomRadius = 0.36f;
        AudioManager.Instance.sfxPool.PlaySFX("potato_mine");
        LevelManager.Instance.UpdateGrid(cellPosition, 1);
        beforePlaceScript.enabled = true;
        enabled = false;
        GameEvents.OnReturnToPool(gameObject.name, gameObject);
    }

    protected override void UpdateAction() { }

    protected override void UpdateIdle() { }
}
