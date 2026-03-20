using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class StartMenu : MonoBehaviour
{
    public Image startBtn;
    [SerializeField] private GameObject challengeBoard;
    private GameObject almanacBoard;
    [SerializeField] private GameObject helpBoard;

    // 实现按任意键进行操作的 InputAction
    private InputAction anyInputAction;

    [SerializeField] private RectTransform tree;
    [SerializeField] private RectTransform tombStone;
    [SerializeField]private RectTransform woodSign1;
    [SerializeField] private RectTransform woodSign2;
    [SerializeField] private RectTransform woodSign3;

    private void Awake()
    {
        almanacBoard = GlobalManager.Instance.ui.GetAlmanac();
        anyInputAction = new InputAction(type: InputActionType.Button);
        anyInputAction.AddBinding("<Keyboard>/anyKey");
        anyInputAction.AddBinding("<Mouse>/leftButton");
        anyInputAction.AddBinding("<Mouse>/rightButton");
        anyInputAction.AddBinding("<Mouse>/middleButton");
        anyInputAction.performed += _ => CloseHelp();
        ResetMenu();
    }

    public void ResetMenu()
    {
        tree.anchoredPosition = new Vector2(-800, 0);
        tombStone.anchoredPosition = new Vector2(-365, -600);
        woodSign1.anchoredPosition = new Vector2(0, 700);
        woodSign2.anchoredPosition = new Vector2(0, 700);
        woodSign3.anchoredPosition = new Vector2(-96.8f, 700);
        tree.DOAnchorPos(new Vector2(238, 0), 0.5f);
        tombStone.DOAnchorPos(new Vector2(-365, 280), 0.5f);
        woodSign1.DOAnchorPos(new Vector2(0,56),1f);
        woodSign2.DOAnchorPos(new Vector2(0, -40.8f), 1f);
        woodSign3.DOAnchorPos(new Vector2(-96.8f, -83.7f), 1f);
    }

    public void SelectedGame()
    {
        challengeBoard.SetActive(true);
    }

    public void OpenAlmanac()
    {
        almanacBoard.SetActive(true);
    }

    public void OpenOptionMenu()
    {
        GlobalManager.Instance.ui.OpenOptionMenu(false);
    }

    public void OpenHelp()
    {
        helpBoard.SetActive(true);
        anyInputAction.Enable();
    }

    private void CloseHelp()
    {
        helpBoard.SetActive(false);
        anyInputAction.Disable();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CloseBoard(GameObject obj)
    {
        AudioManager.Instance.sfxPool.PlaySFX("buttonclick");
        obj.SetActive(false);
        ResetMenu();
    }

    public void ClickSound()
    {
        AudioManager.Instance.sfxPool.PlaySFX("tap1");
    }

    private void OnDestroy()
    {
        anyInputAction.performed -= _ => CloseHelp();
        anyInputAction?.Dispose();
    }
}
