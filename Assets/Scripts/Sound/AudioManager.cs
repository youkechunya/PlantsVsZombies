using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 音乐大小
    /// </summary>
    [Range(0, 1)][SerializeField] private float bgmVolume = 1f;
    /// <summary>
    /// 音效大小
    /// </summary>
    [Range(0, 1)][SerializeField] private float sfxVolume = 1f;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    // 滑块
    [SerializeField] private RectTransform bgmKnob;
    [SerializeField] private RectTransform sfxKnob;

    public BGMPlayer bgm;
    public SFXPool sfxPool;

    public float GetSFXVolume() => sfxVolume;

    public float GetBGMVolume() => bgmVolume;

    /// <summary>
    /// 当音乐滑块被滑动时
    /// </summary>
    public void OnBGMSoundChanged()
    {
        bgmVolume = bgmSlider.value;
        float knobPos = Mathf.Lerp(-68f, 68f, bgmSlider.value);
        bgmKnob.anchoredPosition = new Vector3(knobPos, 1);
        bgm.SetVolume(bgmVolume);
    }

    /// <summary>
    /// 当音效滑块被滑动时
    /// </summary>
    public void OnSFXSoundChanged()
    {
        sfxVolume = sfxSlider.value;
        float knobPos = Mathf.Lerp(-68f, 68f, sfxSlider.value);
        sfxKnob.anchoredPosition = new Vector3(knobPos, 1);
    }
}
