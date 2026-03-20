using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPool : MonoBehaviour
{
    /// <summary>
    /// 音效池
    /// </summary>
    private readonly Queue<AudioSource> audioSourcePool = new();

    /// <summary>
    /// 音效库
    /// </summary>
    [SerializeField] private List<AudioClip> allClips = new();
    /// <summary>
    /// 音效字典（用于通过名字快速查找对应音效）
    /// </summary>
    private readonly Dictionary<string, AudioClip> audioDict = new();

    private void OnEnable()
    {
        GameEvents.OnGamePause += Pause;
        GameEvents.OnGameUnPause += UnPause;
        GameEvents.OnReturnToMenu += ReturnToMenu;
    }

    private void Start()
    {
        // 将所有音效存入字典
        for (int i = 0; i < allClips.Count; i++)
        {
            if (!audioDict.TryAdd(allClips[i].name, allClips[i]))
            {
                Debug.LogWarning($"重复音效名称: {allClips[i].name}");
            }
            //audioDict.Add(allClips[i].name, allClips[i]);
        }

        // 初始化音效池
        for (int i = 0; i < 20; i++)
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audioSourcePool.Enqueue(audio);
        }
    }

    private void OnDisable()
    {
        GameEvents.OnGamePause -= Pause;
        GameEvents.OnGameUnPause -= UnPause;
        GameEvents.OnReturnToMenu -= ReturnToMenu;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="pitchVariation"></param>
    public void PlaySFX(string clipName, float pitchVariation = 0)
    {
        // 如果当前可用音效池为空则直接跳过
        if (audioSourcePool.Count == 0)
            return;

        if (!audioDict.TryGetValue(clipName, out var clip))
        {
            Debug.LogError($"未找到音效: {clipName}");
            return;
        }
        // 从音效池中取出播放器
        AudioSource audio = audioSourcePool.Dequeue();
        audio.clip = clip;
        audio.volume = AudioManager.Instance.GetSFXVolume();

        // 如果该音效需要修改音调
        if (pitchVariation > 0)
        {
            // 随机修改音调
            audio.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        }
        audio.Play();
        // 等音效播放完就返回音效池
        StartCoroutine(ReturnToPool(audio, audio.clip.length));
    }

    /// <summary>
    /// 将播放器延迟返回音效池
    /// </summary>
    /// <param name="source"></param>
    /// <param name="returnDelay"></param>
    /// <returns></returns>
    private IEnumerator ReturnToPool(AudioSource source, float returnDelay)
    {
        yield return new WaitForSeconds(returnDelay);
        audioSourcePool.Enqueue(source);
    }

    private void Pause()
    {
        PlaySFX(SfxType.PAUSE);
    }

    private void UnPause()
    {
        PlaySFX(SfxType.BUTTON_CLICK);
    }

    private void ReturnToMenu()
    {
        PlaySFX(SfxType.GRAVEBUTTON);
    }
}
