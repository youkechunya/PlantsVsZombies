using System;
using UnityEngine;

/// <summary>
/// 事件管理器
/// </summary>
public static class GameEvents
{
    /// <summary>
    /// 游戏暂停事件
    /// </summary>
    public static Action OnGamePause;

    /// <summary>
    /// 游戏继续事件
    /// </summary>
    public static Action OnGameUnPause;

    /// <summary>
    /// 音乐播放事件
    /// </summary>
    public static Action<AudioClip> OnBGMPlay;

    /// <summary>
    /// 返回大厅事件
    /// </summary>
    public static Action OnReturnToMenu;

    /// <summary>
    /// 选卡事件
    /// </summary>
    public static Action OnSelectedSeed;
    
    /// <summary>
    /// 返回对象池事件
    /// </summary>
    public static Action<string, GameObject> OnReturnToPool;

    /// <summary>
    /// 开始游戏后的阳光初始化
    /// </summary>
    public static Action OnSunManagerInitial;

    public static void ClearAll()
    {
        OnGamePause = null;
        OnGameUnPause = null;
        OnBGMPlay = null;
        OnReturnToMenu = null;
        OnSelectedSeed = null;
        OnReturnToMenu = null;
        OnSunManagerInitial = null;
    }
}
