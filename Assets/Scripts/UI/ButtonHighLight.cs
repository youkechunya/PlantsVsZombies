using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighLight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite origin;
    public Sprite highlight;

    Image image;

    public bool needSound = true;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = highlight;
        if (needSound)
            AudioManager.Instance.sfxPool.PlaySFX("bleep");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = origin;
    }
}
