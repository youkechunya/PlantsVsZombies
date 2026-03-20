using System;
using UnityEngine;

[Serializable]
/// <summary>
/// 防具数据
/// </summary>
public class ArmorData
{
    [Header("防具")] public GameObject armor;
    [Header("防具掉落特效")] public ParticleSystem drop;
    [Tooltip("防具总血量")] public int maxHP;
    [Header("外观阶段")] public Sprite[] appearances;
    [Header("受击音效")] public AudioClip[] hitSounds;
    [Header("防护角度范围")] public Vector2 defenseAngleRange = new(135, 225);
    /// <summary>
    /// 改变外观的临界血量
    /// </summary>
    public int[] criticalHps;
    /// <summary>
    /// 当前血量
    /// </summary>
    public int currentHP;

    /// <summary>
    /// 初始化防具
    /// </summary>
    public void Initialize()
    {
        if (maxHP == 0) Debug.LogError("谁家皇帝的防具");
        if (appearances == null) Debug.LogWarning("这个防具不会损坏（）");
        if (hitSounds == null) Debug.LogWarning("这个防具没有受击音效（）");

        currentHP = maxHP;
        // 获取外观数量
        int length = appearances.Length;
        criticalHps = new int[length];
        // 初始化临界血量
        for (int i = 0; i < length - 1; i++)
        {
            criticalHps[i] = maxHP * (length - 1 - i) / 3;
        }

        armor.GetComponent<SpriteRenderer>().sprite = appearances[0];
        armor.SetActive(true);
    }

    /// <summary>
    /// 是否在可防御范围内
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public bool IsInDefenseRange(float angle) => angle > defenseAngleRange.x && angle < defenseAngleRange.y;

    /// <summary>
    /// 获取当前血量下的外观
    /// </summary>
    /// <returns></returns>
    public Sprite GetCurrentAppearance()
    {
        if (currentHP <= 0 || appearances == null) return null;

        // 从受损最严重的外观往回检查
        for (int i = criticalHps.Length - 1; i >= 0; i--)
        {
            if (currentHP < criticalHps[i])
                return appearances[i + 1];  // 受损外观索引比临界值索引+1
        }
        return appearances[0];  // 满血外观
    }
}
