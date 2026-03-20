using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionMenu : MonoBehaviour
{
    /// <summary>
    /// 中间三个按钮
    /// </summary>
    public GameObject GameButton;
    public TMP_Text bottomButtonText;

    /// <summary>
    /// 重开二次提示
    /// </summary>
    [SerializeField] private GameObject confirmRestart;
    /// <summary>
    /// 返回大厅二次提示
    /// </summary>
    [SerializeField] private GameObject confirmReturnToMenu;

    public void CheckAlmanac()
    {
        AudioManager.Instance.sfxPool.PlaySFX(SfxType.GRAVEBUTTON);
        GlobalManager.Instance.ui.OpenAlmanac();
    }

    public void Restart()
    {
        AudioManager.Instance.sfxPool.PlaySFX(SfxType.GRAVEBUTTON);
        confirmRestart.SetActive(false);
        gameObject.SetActive(false);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        GameEvents.OnGameUnPause?.Invoke();
    }

    public void ConfirmRestart() => confirmRestart.SetActive(true);

    public void ConfirmReturnToMenu() => confirmReturnToMenu.SetActive(true);

    public void ReturnToMenu()
    {
        confirmReturnToMenu.SetActive(false);
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameEvents.OnReturnToMenu?.Invoke();
        SceneManager.LoadSceneAsync(LoadingManager.START_MENU);
    }

    public void ReturnToGame()
    {
        AudioManager.Instance.sfxPool.PlaySFX(SfxType.GRAVEBUTTON);
        GameEvents.OnGameUnPause?.Invoke();
    }

    public void Cancel()
    {
        if (confirmRestart.activeInHierarchy)
            confirmRestart.SetActive(false);
        else if (confirmReturnToMenu.activeInHierarchy)
            confirmReturnToMenu.SetActive(false);
    }
}
