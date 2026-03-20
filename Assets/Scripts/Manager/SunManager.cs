using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    public static SunManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        sun = 0;
        numberOfSunshineProduced = 0;
        sunText = GameObject.Find("Sun").GetComponent<TMP_Text>();
    }

    private int sun;
    public int Sun
    {
        get => sun;
        set
        {
            sun = value;
            sunText.text = sun.ToString();
        }
    }

    public Vector3 BankCollectorPos;
    public TMP_Text sunText;

    [SerializeField] private GameObject sunObj;

    private bool naturalSun;
    /// <summary>
    /// 自然阳光产出
    /// </summary>
    public bool NaturalSun
    {
        get => naturalSun;
        set
        {
            naturalSun = value;
            if (naturalSun)
            {
                StartCoroutine(NaturalSunDrop());
            }
        }
    }

    /// <summary>
    /// 本关已生产阳光数
    /// </summary>
    private int numberOfSunshineProduced;
    /// <summary>
    /// 下个自然阳光产生间隔
    /// </summary>
    private float nextSunDuration;

    private void OnEnable()
    {
        GameEvents.OnSunManagerInitial += SunInitial;
    }

    private void OnDisable()
    {
        GameEvents.OnSunManagerInitial -= SunInitial;
    }

    private void SunInitial()
    {
        UpdateBankCollectorPosition();
        NaturalSun = true;
        Sun = Convert.ToInt32(sunText.text);
    }

    private void UpdateBankCollectorPosition()
    {
        BankCollectorPos = GameObject.Find("BankPosition").transform.position;
    }

    IEnumerator NaturalSunDrop()
    {
        while (NaturalSun)
        {
            CalculateNextSunDuration();
            yield return new WaitForSeconds(nextSunDuration);
            float x = UnityEngine.Random.Range(-3.0f, 4.5f);
            GameObject sun = ObjectPool.Instance.GetFromPool(sunObj.name);
            sun.transform.position = new Vector2(x, 3.5f);
            float y = UnityEngine.Random.Range(1.7f, -2.3f);
            sun.transform.DOMoveY(y, 5f);
            numberOfSunshineProduced++;
        }
    }

    private void CalculateNextSunDuration()
    {
        int a = Mathf.Clamp(10 * numberOfSunshineProduced + 425, 0, 950);
        int b = UnityEngine.Random.Range(0, 274);
        nextSunDuration = (a + b) / 100;
    }
}
