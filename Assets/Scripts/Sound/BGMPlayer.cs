using UnityEngine;

/// <summary>
/// “Ù¿÷≤•∑≈∆˜
/// </summary>
public class BGMPlayer : MonoBehaviour
{
    /// <summary>
    /// ≤•∑≈‘¥
    /// </summary>
    [SerializeField] private AudioSource bgm;

    private void OnEnable()
    {
        GameEvents.OnGamePause += Pause;
        GameEvents.OnGameUnPause += UnPause;
        GameEvents.OnBGMPlay += PlayMusic;
        GameEvents.OnReturnToMenu += StopMusic;
    }

    private void OnDisable()
    {
        GameEvents.OnGamePause -= Pause;
        GameEvents.OnGameUnPause -= UnPause;
        GameEvents.OnBGMPlay -= PlayMusic;
        GameEvents.OnReturnToMenu -= StopMusic;
    }

    private void PlayMusic(AudioClip clip)
    {
        bgm.clip = clip;
        bgm.volume = AudioManager.Instance.GetBGMVolume();
        bgm.loop = true;
        bgm.Play();
    }

    /// <summary>
    /// Õ£÷π≤•∑≈
    /// </summary>
    private void StopMusic() => bgm.Stop();

    /// <summary>
    /// ‘›Õ£
    /// </summary>
    private void Pause() => bgm.Pause();

    /// <summary>
    /// ºÃ–¯≤•∑≈
    /// </summary>
    private void UnPause() => bgm.UnPause();

    /// <summary>
    /// …Ë÷√“Ù¡ø
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume) => bgm.volume = volume;
}
