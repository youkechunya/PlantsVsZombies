using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelConfig",menuName = "PVZ/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("背景音乐")]
    public AudioClip bgm;
    [Header("总波数")]
    public int totalWave;
    [Header("旗帜波数")]
    public int flagWave;
    [Header("僵尸移速倍率")]
    public float zombieSpeedMagnification = 0.2f;
    [Header("植物冷却倍率")]
    public float cooldownMagnification = 1f;
    [Header("出怪倍率")]
    public float spawnMagnification = 1;
    [Header("保护波数（即不受出怪倍率影响的波数）")]
    public int protectWaveNumber = 5;
    [Header("本关僵尸列表")]
    public List<ZombieData> zombieList;
    [Header("本关Boss")]
    public List<GameObject> bossList;
}
