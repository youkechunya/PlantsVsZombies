using UnityEngine;

public class PauseManager : MonoBehaviour
{
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

    private void Pause()
    {
        Time.timeScale = 0.0f;
    }

    private void UnPause()
    {
        Time.timeScale = 1.0f;
    }
}
