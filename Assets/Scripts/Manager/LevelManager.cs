using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        zombieSpawner.level = this;
    }

    /// <summary>
    /// 种子卡槽
    /// </summary>
    public RectTransform seedBank;
    /// <summary>
    /// 种子选取槽
    /// </summary>
    public RectTransform seedChooser;
    /// <summary>
    /// 铲子槽
    /// </summary>
    public GameObject shovelBank;
    /// <summary>
    /// 种植网格
    /// </summary>
    public Grid grid;
    /// <summary>
    /// 格子的信息（5行9列）
    /// </summary>
    private readonly bool[,] gridInfo = new bool[9, 5];

    private bool isStart;
    public bool IsStart
    {
        get => isStart;
        set => isStart = value;
    }

    /// <summary>
    /// 自动收集
    /// </summary>
    public bool autoCollector = true;

    /// <summary>
    /// 选卡时音乐
    /// </summary>
    [SerializeField] private AudioClip selectMusic;
    /// <summary>
    /// 失败音乐
    /// </summary>
    [SerializeField] private AudioClip loseMusic;
    /// <summary>
    /// 游戏结束界面
    /// </summary>
    [SerializeField] private GameObject gameOver;

    /// <summary>
    /// 初始植物列表
    /// </summary>
    [SerializeField] private GameObject[] originPlantList;

    /// <summary>
    /// 僵尸生成器
    /// </summary>
    [SerializeField] private ZombieSpawner zombieSpawner;
    /// <summary>
    /// 胜利奖杯
    /// </summary>
    [SerializeField] private GameObject trophy;
    /// <summary>
    /// 是否生成奖杯
    /// </summary>
    public bool spawnTrophy;
    /// <summary>
    /// 关卡配置
    /// </summary>
    public LevelConfig levelConfig;

    private bool lastWaveSpawn;
    public bool LastWaveSpawn
    {
        get => lastWaveSpawn;
        set
        {
            lastWaveSpawn = value;
            // 获取场上所有僵尸
            GameObject[] allZombies = GameObject.FindGameObjectsWithTag("Zombie");
            // 为每个僵尸注册事件
            foreach (var zombie in allZombies)
            {
                Debug.Log("最后一波：给在场所有僵尸注册事件");
                if (zombie.GetComponent<BaseZombie>() != null)
                    zombie.GetComponent<BaseZombie>().LastZombieDieEvent += WhenZombieDieInLastWave;
            }
        }
    }

    private OptionMenu optionMenu;
    private Coroutine selectedSeedCoroutine;

    /// <summary>
    /// 选卡时的画面移动
    /// </summary>
    Tween startMoveTween;

    private void OnEnable()
    {
        GameEvents.OnReturnToMenu += ReturnToMenu;
        optionMenu = GlobalManager.Instance.ui.GetOptionMenu();
    }

    private void Start()
    {
        // 将全局变量中的游戏中设为 false
        GlobalManager.Instance.InTheGame = false;
        // 从全局管理中获取当前关卡配置
        levelConfig = GlobalManager.Instance.currentLevel;
        zombieSpawner.levelConfig = levelConfig;
        int zombieTypeCount = levelConfig.zombieList.Count;
        for (int i = 0; i < zombieTypeCount; i++)
        {
            GameObject waitingZombie = ObjectPool.Instance.GetFromPool(levelConfig.zombieList[i].name);
            waitingZombie.GetComponent<BaseZombie>().WaitingStart = true;
            float x = Random.Range(6.4f, 7.7f);
            float y = Random.Range(-2.46f, 0.78f);
            waitingZombie.transform.position = new Vector3(x, y);
        }
        // 播放选卡音乐
        GameEvents.OnBGMPlay(selectMusic);
        // 开启选卡协程
        selectedSeedCoroutine = StartCoroutine(SelectedSeed());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Time.timeScale = 3f;
        if (Input.GetKeyDown(KeyCode.M))
            Time.timeScale = 1f;
        if (Input.GetKeyDown(KeyCode.B))
            Time.timeScale = 0.3f;

        if (Input.GetKeyDown(KeyCode.L))
            autoCollector = !autoCollector;

    }

    private void OnDisable()
    {
        GameEvents.OnReturnToMenu -= ReturnToMenu;
    }

    IEnumerator SelectedSeed()
    {
        // 不可视铲子槽
        shovelBank.SetActive(false);
        yield return new WaitForSeconds(0.75f);
        // 移动相机
        startMoveTween = GlobalManager.Instance.mainCamera.transform.DOMoveX(4, 1.5f)
            .SetEase(Ease.InQuad);
        yield return startMoveTween;
        yield return new WaitForSeconds(0.5f);
        seedBank.DOAnchorPosY(-70, 0.4f);
        seedChooser.DOAnchorPosY(-70, 0.3f);
    }

    /// <summary>
    /// 更新网格状态，status = 0时为放置，= 1时为销毁
    /// </summary>
    /// <param name="position"></param>
    /// <param name="status"></param>
    public void UpdateGrid(Vector3Int position, int status)
    {
        gridInfo[position.x, position.y] = status == 0;
    }

    /// <summary>
    /// 查看该格子是否被占用
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool CheckGrid(Vector3Int position)
    {
        return gridInfo[position.x, position.y];
    }

    public void ZombieSpawner()
    {
        zombieSpawner.SummonZombie();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") && !gameOver.activeInHierarchy)
        {
            GameEvents.OnBGMPlay(loseMusic);
            gameOver.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ReStart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void OriginPlant()
    {
        StartCoroutine(OriginPlantIEnumerator());
    }

    IEnumerator OriginPlantIEnumerator()
    {
        int y = 1;
        int x = -6;
        for (int i = 0; i < 9; i++)
        {
            y = 1;
            for (int j = 0; j < 5; j++)
            {
                GameObject peaShooter = ObjectPool.Instance.GetFromPool(originPlantList[0].name);
                peaShooter.GetComponent<BeforePlace>().PlacePosition(new Vector3Int(x, y--));
            }
            x += 1;
        }
        yield return null;
    }

    /// <summary>
    /// 当僵尸在最后一波后死亡
    /// </summary>
    /// <param name="obj"></param>
    public void WhenZombieDieInLastWave(GameObject obj)
    {
        // 如果场上没有僵尸且未生成奖杯
        Debug.LogWarning($"剩余僵尸数量：{zombieSpawner.zombieList.Count} \t 是否生成奖杯：{spawnTrophy}");
        if (zombieSpawner.zombieList.Count == 0 && !spawnTrophy)
        {
            spawnTrophy = true;
            GameObject drop = ParticlePool.Instance.GetFromPool("TrophyDrop");
            drop.GetComponent<ParticleSystem>().Play();
            drop.transform.position = obj.transform.position;
        }
        obj.GetComponent<BaseZombie>().LastZombieDieEvent -= WhenZombieDieInLastWave;
    }

    public void GetMenu()
    {
        if (IsStart)
        {
            GameEvents.OnGamePause?.Invoke();
        }
        else
        {
            StopCoroutine(selectedSeedCoroutine);
            GlobalManager.Instance.mainCamera.transform.position = new Vector3(0, 0, -10);
            optionMenu.ReturnToMenu();
        }
    }

    private void ReturnToMenu()
    {
        if (startMoveTween != null)
        {
            startMoveTween.Kill();
        }
    }
}
