using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;

    public void SelectedGame()
    {
        GlobalManager.Instance.currentLevel = levelConfig;
        SceneManager.LoadSceneAsync(LoadingManager.GAME);
    }
}
