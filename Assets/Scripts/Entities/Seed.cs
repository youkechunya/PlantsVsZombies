using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Seed : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("对应植物")]
    public GameObject plant;
    [Header("价格")]
    [SerializeField] private int cost;

    public GameObject disable;
    private bool isSelected;
    public bool isSpawning;
    private bool isCoolingDown;
    private Coroutine coolingDownCoroutine;
    [SerializeField] private Image coolDownImage;
    [SerializeField] private float cooldown;
    /// <summary>
    /// 当前冷却剩余时间
    /// </summary>
    [SerializeField] private float currentCooldown = 0;

    [SerializeField] private string seedChooseName;
    public string SeedChooseName
    {
        get { return seedChooseName; }
    }

    Game gamePlay;
    private bool mouseFocus;

    private void Start()
    {
        cooldown *= LevelManager.Instance.levelConfig.cooldownMagnification;
    }

    private void OnEnable()
    {
        if (gamePlay == null)
        {
            gamePlay = new Game();
            gamePlay.GamePlay.Enable();
            gamePlay.GamePlay.Select.performed += Selected;
        }
    }

    private void Update()
    {
        if (!LevelManager.Instance.IsStart)
            return;

        // 植物不可点击的遮罩
        disable.SetActive(SunManager.Instance.Sun < cost || currentCooldown > 0 || isSelected);
    }

    private void OnDisable()
    {
        if (gamePlay != null)
        {
            gamePlay.GamePlay.Select.performed -= Selected;
            gamePlay.GamePlay.Disable();
        }
        if (coolingDownCoroutine != null)
        {
            StopCoroutine(coolingDownCoroutine);
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }

    public void Selected(InputAction.CallbackContext ctx)
    {
        if (!mouseFocus || isSpawning || isCoolingDown)
            return;

        if (!LevelManager.Instance.IsStart)
        {
            SeedManager.Instance.CancelSeed(this);
            return;
        }

        if (currentCooldown > 0 || SunManager.Instance.Sun < cost)
        {
            AudioManager.Instance.sfxPool.PlaySFX("buzzer");
            return;
        }

        // 生成植物
        GameObject spawnedPlant = ObjectPool.Instance.GetFromPool(seedChooseName);
        if (spawnedPlant == null)
        {
            Debug.LogError($"生成植物失败: {seedChooseName}");
            return;
        }

        if (!spawnedPlant.TryGetComponent<BeforePlace>(out var beforePlace))
        {
            Debug.LogError($"生成的植物缺少 BeforePlace 脚本！猪笔: {seedChooseName}");
            GameEvents.OnReturnToPool(seedChooseName, spawnedPlant);
            return;
        }

        isSelected = true;
        beforePlace.NeedTipObj = true;
        beforePlace.seed = this;
        beforePlace.cost = cost;

        AudioManager.Instance.sfxPool.PlaySFX("seedlift");
        isSpawning = true;
    }

    public void CancelPlace()
    {
        isSelected = false;
        isSpawning = false;
    }

    public void StartCooldown()
    {
        if (isCoolingDown)
            return;

        StartCoroutine(Cooling());
    }

    IEnumerator Cooling()
    {
        isCoolingDown = true;
        isSelected = true;
        currentCooldown = cooldown;

        while (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            // 更新冷却图片
            coolDownImage.fillAmount = currentCooldown / cooldown;
            yield return null;
        }

        currentCooldown = 0;
        coolDownImage.fillAmount = 0;
        isCoolingDown = false;
        isSelected = false;
        coolingDownCoroutine = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseFocus = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseFocus = false;
    }
}
