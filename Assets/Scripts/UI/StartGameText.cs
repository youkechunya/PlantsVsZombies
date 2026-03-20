using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartGameText : MonoBehaviour
{
    Image image;

    public Sprite ok;
    public Sprite ready;
    public Sprite startGame;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        AudioManager.Instance.sfxPool.PlaySFX("readysetplant");
        image.sprite = ok;
        image.SetNativeSize();
        transform.localScale = Vector3.one;
        float scale = 0.24f;
        while (scale < 0.32f)
        {
            scale += Time.deltaTime / 4f;
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        image.sprite = ready;
        image.SetNativeSize();
        scale = 0.36f;
        while(scale < 0.5f)
        {
            scale += Time.deltaTime / 5f;
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        yield return new WaitForSeconds(0.4f);
        image.sprite = startGame;
        image.SetNativeSize();
        transform.localScale = Vector3.one * 0.35f;
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
