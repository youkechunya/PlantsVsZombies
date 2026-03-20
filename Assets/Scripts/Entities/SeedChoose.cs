using UnityEngine;
using UnityEngine.EventSystems;

public class SeedChoose : MonoBehaviour, IPointerClickHandler
{
    public GameObject disableMask;
    private bool disable;
    public bool Disable
    {
        get => disable;
        set => disable = value;
    }

    /// <summary>
    /// 游戏时可进行种植的种子
    /// </summary>
    public GameObject seed;

    public void OnPointerClick(PointerEventData eventData)
    {
        SeedManager.Instance.ChooseSeed(this);
    }
}
