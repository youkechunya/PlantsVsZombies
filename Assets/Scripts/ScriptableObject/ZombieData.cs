using UnityEngine;

[CreateAssetMenu(fileName = "NewZombieData",menuName = "PVZ/ZombieData")]
public class ZombieData : ScriptableObject
{
    [Header("僵尸类型")]
    public ZombieType type;
    [Header("战力值")]
    public int powerValue;
    [Header("基础权重")]
    public int baseWeight;
    [Header("预制体")]
    public GameObject prefab;

    /// <summary>
    /// 动态获取权重
    /// </summary>
    /// <param name="wave"></param>
    /// <returns></returns>
    public int GetWeight(int wave)
    {
        // 以每一大波前共 20 小波为准归一化
        float t = Mathf.Min(wave / 20f, 1f);
        return type switch
        {
            ZombieType.Regular => Mathf.RoundToInt(baseWeight * (1 - t * 0.3f)),
            ZombieType.ConeHead => Mathf.RoundToInt(baseWeight * (1 + t * 0.5f)),
            ZombieType.BucketHead => Mathf.RoundToInt(baseWeight * (1 + t * 0.8f)),
            _ => baseWeight
        };
    }
}
