using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ZombieSpawner : MonoBehaviour
{
    class SummonZombieInfo
    {
        public ZombieType type;
        public int count;

        public SummonZombieInfo(ZombieType type, int count)
        {
            this.type = type;
            this.count = count;
        }
    }

    /// <summary>
    /// 级别上限，用于控制当前波次生成的僵尸总战力值
    /// </summary>
    private int levelCap;
    /// <summary>
    /// 当前波次
    /// </summary>
    public int wave = 0;
    /// <summary>
    /// 当前旗帜波次
    /// </summary>
    private int currentFlagWave = 1;

    /// <summary>
    /// 波次间隔
    /// </summary>
    [SerializeField] private float nextWaveRemain;
    private readonly float waveIntervalMin = 25f;
    private readonly float waveIntervalMax = 31f;

    /// <summary>
    /// 当前波次的总血量
    /// </summary>
    [SerializeField] private int waveHP;
    /// <summary>
    /// 触发下一波的临界血量
    /// </summary>
    [SerializeField] private int criticalWaveHP;
    /// <summary>
    /// 当前波次的剩余血量
    /// </summary>
    public int currentWaveHP;
    /// <summary>
    /// 本关信息
    /// </summary>
    public LevelConfig levelConfig;

    [SerializeField] private Slider slider;
    public FlagBar flagBar;
    [SerializeField] private GameObject flag;
    [SerializeField] private GameObject hugeWaveText;

    public LevelManager level;

    /// <summary>
    /// 当前生成的僵尸列表
    /// </summary>
    public Dictionary<int, GameObject> zombieList = new();
    /// <summary>
    /// 剩余僵尸数
    /// </summary>
    [SerializeField] private TMP_Text zombieCountText;

    private void Start()
    {
        flagBar = slider.GetComponent<FlagBar>();
        // 动态生成本关旗帜数
        for (int i = 0; i < levelConfig.flagWave; i++)
        {
            float flagPos = -63 + (63 + 72) / levelConfig.flagWave * i;
            GameObject f = Instantiate(flag);
            f.transform.SetParent(slider.gameObject.transform);
            f.transform.localScale = new Vector3(1, 1, 1);
            f.GetComponent<RectTransform>().anchoredPosition = new Vector3(flagPos, 7, 0);
            flagBar.flags.Add(f);
        }
        flagBar.flagPart.SetSiblingIndex(slider.transform.childCount - 1);
        slider.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 下一波倒计时
        nextWaveRemain -= Time.deltaTime;
    }

    public void SummonZombie()
    {
        slider.gameObject.SetActive(true);
        // 首波僵尸出现音效
        AudioManager.Instance.sfxPool.PlaySFX("awooga");
        StartCoroutine(SummonZombieIEnumerator());
    }

    IEnumerator SummonZombieIEnumerator()
    {
        // 当没到最后一波时
        while (wave < levelConfig.totalWave)
        {
            if (nextWaveRemain > 0)
            {
                if (currentWaveHP <= criticalWaveHP && nextWaveRemain >= 2f)
                    nextWaveRemain = 2f;

                yield return null;
                continue;
            }
            // 进行当前一波
            wave++;
            // 重置下一波出现时间
            nextWaveRemain = Random.Range(waveIntervalMin, waveIntervalMax);
            // 获取当前波僵尸情况
            List<SummonZombieInfo> waveInfo = GetWaveZombie();
            // 计算当前波僵尸总血量
            CalculateWaveTotalHP(waveInfo);
            // 如果是一大波
            if (wave == levelConfig.totalWave / levelConfig.flagWave * (currentFlagWave - 1))
                yield return new WaitForSeconds(6f);
            // 如果是最后一波
            if (wave == levelConfig.totalWave)
                yield return new WaitForSeconds(3f);
            // 生成僵尸
            for (int i = 0; i < waveInfo.Count; i++)
            {
                for (int j = 0; j < waveInfo[i].count; j++)
                {
                    int randomRow = Random.Range(0, 5);
                    GameObject newZombie = ObjectPool.Instance.GetFromPool(waveInfo[i].type.ToString() + "Zombie");
                    newZombie.transform.position = new Vector2(6.2f, 1.4f - randomRow);
                    newZombie.GetComponent<BaseZombie>().belongingWave = wave;
                    newZombie.GetComponent<BaseZombie>().spawner = this;
                    newZombie.GetComponent<SortingGroup>().sortingOrder = randomRow;
                    zombieList.Add(newZombie.GetInstanceID(), newZombie);
                    SetZombieRemainText();
                }
            }
            if (wave == levelConfig.totalWave)
            {
                level.LastWaveSpawn = true;
                if (level.levelConfig.bossList.Count != 0)
                {
                    GameObject boss = ObjectPool.Instance.GetFromPool(level.levelConfig.bossList[0].name.Replace("(Clone)", ""));
                    boss.transform.position = new Vector2(4.43f, -0.46f);
                }
            }
            StartCoroutine(OnWaveMoving());
        }
    }

    /// <summary>
    /// 获取当前波次的僵尸种类及数量
    /// </summary>
    /// <returns></returns>
    private List<SummonZombieInfo> GetWaveZombie()
    {
        List<SummonZombieInfo> result = new();
        // 先计算级别上限
        CalculateLevelCap();
        int remainCap = levelCap;

        // 如果剩余级别上限大于 0 
        while (remainCap > 0)
        {
            // 筛选出可选僵尸种类并通过战力值降序排序
            var option = levelConfig.zombieList.Where(z => z.powerValue <= remainCap)
                .OrderByDescending(z => z.powerValue)
                .ToList();

            if (option.Count == 0)
            {
                Debug.LogWarning($"第{wave}波：没有可用的僵尸配置");
                return result;
            }
            // 根据权重抽取僵尸
            var select = GetWeightedZombieData(option, wave);
            // 将抽取到的僵尸添加至结果
            AddOrIncrement(result, select.type);
            remainCap -= select.powerValue;
        }
        Debug.Log($"第{wave}波生成: {string.Join(", ", result.Select(r => $"{r.type}x{r.count}"))}, 总战力:{levelCap}");
        return result;
    }

    /// <summary>
    /// 加权算法获取随机僵尸
    /// </summary>
    /// <param name="available"></param>
    /// <param name="currentWave"></param>
    /// <returns></returns>
    private ZombieData GetWeightedZombieData(List<ZombieData> available, int currentWave)
    {
        // 获取当前可选取僵尸的权重
        List<int> weights = available.Select(z => z.GetWeight(currentWave)).ToList();
        // 获取权重合
        int sumWeight = weights.Sum();
        // 随机抽取一个数然后根据这个数落在哪个权重区来选取僵尸
        int random = Random.Range(0, sumWeight);
        // 当前权重区
        int current = 0;
        for (int i = 0; i < available.Count; i++)
        {
            current += weights[i];
            if (random < current)
            {
                return available[i];
            }
        }
        // 保底（一般不会执行到这）
        return available.Last();
    }

    private void AddOrIncrement(List<SummonZombieInfo> list, ZombieType type)
    {
        var existing = list.Find(z => z.type == type);
        if (existing != null)
        {
            existing.count++;
        }
        else
        {
            list.Add(new SummonZombieInfo(type, 1));
        }
    }

    /// <summary>
    /// 计算当前波次的级别上限
    /// </summary>
    private void CalculateLevelCap()
    {
        // 每波级别上限 = int(int((当前波数 + 已完成选卡数 * 每轮总波数) * 0.8) / 2) + 1，旗帜波再* 2.5并向零取整。
        levelCap = (int)((wave + (currentFlagWave - 1) * levelConfig.totalWave / levelConfig.flagWave) * 0.8f * ((wave - levelConfig.protectWaveNumber > 0) ? levelConfig.spawnMagnification : 1) / 2 + 1);
        if (levelConfig.flagWave == 0)
        {
            Debug.LogError("警告！当前关卡未设置旗帜波！");
            return;
        }
        if (wave == levelConfig.totalWave / levelConfig.flagWave * currentFlagWave)
        {
            levelCap = (int)(levelCap * 2.5f);
            currentFlagWave++;
            hugeWaveText.SetActive(true);
            Debug.Log($"当前为旗帜波，级别上限为：{levelCap}");
        }
    }

    /// <summary>
    /// 进度条移动
    /// </summary>
    /// <returns></returns>
    IEnumerator OnWaveMoving()
    {
        while (slider.value < (float)wave / (float)levelConfig.totalWave)
        {
            slider.value += Time.deltaTime / 10;
            yield return null;
        }
        slider.value = (float)wave / (float)levelConfig.totalWave;
    }

    /// <summary>
    /// 计算当前波次生成的僵尸总血量。
    /// 注：二类防具的血量只取 20% 且舞王召唤的伴舞以及蹦极僵尸等不计入总血量
    /// </summary>
    /// <param name="waveInfomation"></param>
    private void CalculateWaveTotalHP(List<SummonZombieInfo> waveInfomation)
    {
        waveHP = 0;
        for (int i = 0; i < waveInfomation.Count; i++)
        {
            for (int j = 0; j < waveInfomation[i].count; j++)
            {
                switch (waveInfomation[i].type)
                {
                    case ZombieType.Regular:
                        waveHP += 270;
                        break;
                    case ZombieType.ConeHead:
                        waveHP += 270 + 370;
                        break;
                    case ZombieType.BucketHead:
                        waveHP += 270 + 1100;
                        break;
                    default:
                        break;
                }
            }
        }
        currentWaveHP = waveHP;
        criticalWaveHP = (int)(waveHP * Random.Range(0.5f, 0.65f));
    }

    public void SetZombieRemainText()
    {
        zombieCountText.text = "剩余僵尸数：" + zombieList.Count;
    }
}
