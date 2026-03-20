using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }
    private void Awake()
    {
        if (ui == null)
        {
            Debug.Log("注意！你没挂 ui 脚本！");
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InTheGame = false;
        gameInput = new Game();
        mainCamera = Camera.main;
    }

    Game gameInput;

    /// <summary>
    /// 存储当前选中的关卡数据
    /// </summary>
    public LevelConfig currentLevel;

    private bool inTheGame;
    /// <summary>
    /// 是否在进行游戏
    /// </summary>
    public bool InTheGame
    {
        get { return inTheGame; }
        set
        {
            inTheGame = value;
        }
    }

    public Camera mainCamera;

    public UIManager ui;

    private void OnEnable()
    {
        if (gameInput != null)
        {
            gameInput.UI.Enable();
            gameInput.UI.OptionMenu.performed += OpenOptionMenu;
        }
        GameEvents.OnReturnToMenu += ReturnToMenu;
    }

    private void Update()
    {
        Vector3 position = mainCamera.transform.position;
        position.z = 0;
        transform.position = position;
    }

    void OpenOptionMenu(InputAction.CallbackContext ctx)
    {
        if (InTheGame)
        {
            GameEvents.OnGamePause?.Invoke();
        }
    }

    public void CloseBoard(GameObject obj)
    {
        AudioManager.Instance.sfxPool.PlaySFX("buttonclick");
        obj.SetActive(false);
    }

    private void OnDisable()
    {
        if (gameInput != null)
        {
            gameInput.UI.OptionMenu.performed -= OpenOptionMenu;
            gameInput.UI.Disable();
        }
        GameEvents.OnReturnToMenu -= ReturnToMenu;
    }

    private void ReturnToMenu()
    {
        InTheGame = false;
        transform.position = new Vector3(0, 0, 0);
    }
}
