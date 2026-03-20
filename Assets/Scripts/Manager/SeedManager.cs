using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 当前卡槽的最后一格位置
    /// </summary>
    private Vector3 currentChoosePos = new(396.5f, -69f);

    /// <summary>
    /// 选卡槽第一格
    /// </summary>
    private readonly Vector3 chooserVertex = new(301.6f, -276, 0);

    [Header("卡槽")]
    public GameObject seedBank;

    [Header("选卡槽")]
    public GameObject seedChooser;

    [Header("空槽")]
    public GameObject blank;

    public List<GameObject> seedList;

    /// <summary>
    /// 可选种子数量
    /// </summary>
    private readonly int bankSize = 12;

    private int currentSize = 0;

    [SerializeField] private EventTrigger checkAlmanac;

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        EventTrigger.Entry entry = new()
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((date) =>
        {
            CheckAlmanac();
        });
        checkAlmanac.triggers.Add(entry);
    }

    private void CheckAlmanac()
    {
        GlobalManager.Instance.ui.OpenAlmanac();
    }

    public void ChooseSeed(SeedChoose seed)
    {
        if (currentSize < bankSize)
        {
            if (!seed.Disable)
                StartCoroutine(ChooseSeedAnimation(seed));
        }
    }

    IEnumerator ChooseSeedAnimation(SeedChoose seed)
    {
        // 使种子不可再次被选中
        seed.Disable = true;
        seed.disableMask.SetActive(seed.Disable);
        // 获取种子在卡槽中的位置
        int position = currentSize++;
        // 生成对应卡片
        GameObject obj = Instantiate(seed.seed, transform);
        seedList.Add(obj);
        // 获取卡片的 RectTransform
        RectTransform objTransform = obj.GetComponent<RectTransform>();
        // 设置卡片定点
        objTransform.anchorMin = new Vector3(0, 1);
        objTransform.anchorMax = new Vector3(0, 1);
        // 获取卡片位置
        objTransform.position = seed.transform.GetComponent<RectTransform>().position;
        objTransform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
        objTransform.DOAnchorPos(currentChoosePos, 0.1f);
        // 将下一张卡片的位置后移
        currentChoosePos.x += 86;
        yield return new WaitForSeconds(0.1f);
        // 将卡片放入卡槽
        obj.transform.SetParent(seedBank.transform);
        objTransform.localScale = new Vector3(1, 1, 1);
        // 销毁最后面的一个空槽（相当于销毁当前格）
        Destroy(seedBank.transform.GetChild(bankSize - 1).gameObject);
        // 设置卡片的位置
        obj.transform.SetSiblingIndex(position);
        // 播放声音
        int randomSound = Random.Range(1, 3);
        AudioManager.Instance.sfxPool.PlaySFX("tap" + randomSound);
    }

    public void CancelSeed(Seed seed)
    {
        StartCoroutine(CancelSeedAnimation(seed));
    }

    IEnumerator CancelSeedAnimation(Seed seed)
    {
        // 获取种子在选卡槽的卡片
        SeedChoose seedChoose = seedChooser.transform.Find(seed.SeedChooseName).GetComponent<SeedChoose>();
        // 获取对应选卡槽卡片在选卡槽的相对位置
        Vector3 returnPos = seedChoose.GetComponent<RectTransform>().anchoredPosition;
        // 转换卡片的相对坐标
        returnPos = AnchoredSwitcher(returnPos);
        returnPos += chooserVertex;
        // 将卡片从卡槽中抽出
        seed.transform.SetParent(transform);
        seedList.Remove(seed.gameObject);
        seed.GetComponent<RectTransform>().DOAnchorPos(returnPos, 0.1f);
        // 生成一个空槽
        GameObject blank = Instantiate(this.blank, seedBank.transform);
        blank.transform.localScale = new Vector3(1, 1, 1);
        currentSize--;
        currentChoosePos.x -= 86;
        yield return new WaitForSeconds(0.1f);
        // 使选卡槽的对应卡片可被选中
        seedChoose.Disable = false;
        seedChoose.disableMask.SetActive(seedChoose.Disable);
        // 销毁卡片
        Destroy(seed.gameObject);
    }

    /// <summary>
    /// 将卡片在选卡槽的相对坐标转为世界坐标
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector2 AnchoredSwitcher(Vector2 pos)
    {
        int x = (int)pos.x / 50;
        int y = (int)pos.y / 70 + 1;
        return new Vector2(88.4f * x, 122.8f * y);
    }
}
