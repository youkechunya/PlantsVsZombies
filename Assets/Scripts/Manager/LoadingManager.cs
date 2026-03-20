using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public const string START_MENU = "StartMenu";
    public const string GAME = "Game";
}
