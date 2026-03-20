using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 选项卡
    [SerializeField] private OptionMenu optionMenu;
    // 图鉴
    [SerializeField] private GameObject almanac;

    private void Awake()
    {
        if (optionMenu == null) Debug.LogError("选项卡没赋值！");
        if (almanac == null) Debug.LogError("图鉴没赋值！");
    }

    private void OnEnable()
    {
        GameEvents.OnGamePause += Pause;
        GameEvents.OnGameUnPause += UnPause;
    }

    private void OnDisable()
    {
        GameEvents.OnGamePause -= Pause;
        GameEvents.OnGameUnPause -= UnPause;
    }

    public void Pause()
    {
        optionMenu.gameObject.SetActive(true);
        optionMenu.GameButton.SetActive(true);
        optionMenu.bottomButtonText.text = "返回游戏";
    }

    private void UnPause()
    {
        optionMenu.gameObject.SetActive(false);
    }

    public void OpenOptionMenu(bool inTheGame)
    {
        optionMenu.gameObject.SetActive(true);
        optionMenu.GameButton.SetActive(inTheGame);
        optionMenu.bottomButtonText.text = inTheGame ? "返回游戏" : "确定";
    }

    public OptionMenu GetOptionMenu() { return optionMenu; }
    public GameObject GetAlmanac() {  return almanac; }

    public void OpenAlmanac()
    {
        almanac.SetActive(true);
    }
}
