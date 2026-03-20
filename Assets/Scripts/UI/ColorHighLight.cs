using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorHighLight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color originColor;
    public Color highlightColor;

    private SpriteRenderer spriteRenderer;
    private Image image;
    private TMP_Text text;

    private void Awake()
    {
        image = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        text = GetComponent<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null)
            image.color = highlightColor;
        if (spriteRenderer != null)
            spriteRenderer.color = highlightColor;
        if (text != null)
            text.color = highlightColor;
        AudioManager.Instance.sfxPool.PlaySFX("bleep");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null)
            image.color = originColor;
        if (spriteRenderer != null)
            spriteRenderer.color = originColor;
        if (text != null)
            text.color = originColor;
    }
}
