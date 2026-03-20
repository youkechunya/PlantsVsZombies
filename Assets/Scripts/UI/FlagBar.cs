using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagBar : MonoBehaviour
{
    public RectTransform flagPart;
    private Slider slider;

    public List<GameObject> flags = new();

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void OnFlagPartChanged()
    {
        float flagPos = Mathf.Lerp(75f, -75f, slider.value);
        flagPart.anchoredPosition = new Vector3(flagPos, 16.8f, 0);
    }
}
