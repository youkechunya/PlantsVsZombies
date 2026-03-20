using System.Collections;
using UnityEngine;

public class SunFlower : BasePlant
{
    /// <summary>
    /// 冷却时间
    /// </summary>
    private float cooldown = 24f;
    /// <summary>
    /// 最快冷却时间
    /// </summary>
    private readonly float cooldownMin = 23.5f;
    /// <summary>
    /// 最慢冷却时间
    /// </summary>
    private readonly float cooldownMax = 25f;
    /// <summary>
    /// 生产时的亮度
    /// </summary>
    private readonly float processBrightness = 4;

    /// <summary>
    /// 阳光预制体
    /// </summary>
    [SerializeField] private GameObject sun;

    /// <summary>
    /// 一次产生的数量
    /// </summary>
    private int produceCount = 4;

    protected override void OnEnable()
    {
        base.OnEnable();
        cooldown = Random.Range(3.0f, 12.5f);
    }

    private void Start()
    {
        ChangeState(PlantState.Action);
    }

    protected override void UpdateAction()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
        {
            StartCoroutine(ProcessSun());
        }
    }

    /// <summary>
    /// 生产阳光
    /// </summary>
    /// <returns></returns>
    IEnumerator ProcessSun()
    {
        brightness = 1;
        cooldown = Random.Range(cooldownMin, cooldownMax);
        // 更新亮度
        while (brightness < processBrightness)
        {
            UpdateLight(brightness);
            brightness += Time.deltaTime * 4;
            yield return null;
        }
        for (int i = 0; i < produceCount; i++)
        {
            // 获取阳光
            GameObject sun = ObjectPool.Instance.GetFromPool(this.sun.name);
            sun.transform.position = transform.position;
            // 随机偏移
            float xOffset = Random.Range(-0.75f, 0.75f);
            sun.GetComponent<Sun>().StartSpawn(transform.position, new Vector2(transform.position.x + xOffset, transform.position.y));
        }
        // 恢复亮度
        while (brightness > 1f)
        {
            brightness = Mathf.Clamp(brightness -= Time.deltaTime * 4, 1, 5);
            UpdateLight(brightness);
            yield return null;
        }
    }

    protected override void UpdateIdle() { }
}
