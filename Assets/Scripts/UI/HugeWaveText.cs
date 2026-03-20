using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HugeWaveText : MonoBehaviour
{
    Image image;

    [SerializeField] private Sprite hugewave;
    [SerializeField] private Sprite finalwave;

    public ZombieSpawner spawner;

    private int flagIndex = 0;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        flagIndex++;
        if (spawner.wave != spawner.levelConfig.totalWave)
        {
            image.sprite = hugewave;
            image.SetNativeSize();
            image.transform.localScale = new Vector3(4, 4, 4);
            AudioManager.Instance.sfxPool.PlaySFX("hugewave");
            image.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 3f);
            yield return new WaitForSeconds(6.4f);
            spawner.flagBar.flags[spawner.levelConfig.flagWave - flagIndex].GetComponent<Animator>().SetTrigger("trigger");
            gameObject.SetActive(false);
        }
        else
        {
            image.sprite = hugewave;
            image.SetNativeSize();
            image.transform.localScale = new Vector3(4, 4, 4);
            AudioManager.Instance.sfxPool.PlaySFX("hugewave");
            image.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 3f);
            yield return new WaitForSeconds(6.4f);
            image.sprite = finalwave;
            image.SetNativeSize();
            image.transform.localScale = new Vector3(4, 4, 4);
            AudioManager.Instance.sfxPool.PlaySFX("finalwave");
            image.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1.5f);
            yield return new WaitForSeconds(2.5f);
            spawner.flagBar.flags[spawner.levelConfig.flagWave - flagIndex].GetComponent<Animator>().SetTrigger("trigger");
            gameObject.SetActive(false);
        }
    }
}
