using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class UpdateFrameManager : MonoBehaviour
{
    public static UpdateFrameManager instance;

    const float fpsMeasurePeriod = 0.5f;    //FPS测量间隔
    private float m_FpsNextPeriod = 0;  //FPS下一段的间隔
    public int FPS = 60;//限帧
    [SerializeField] private TMP_Text fpsText;

    [DllImport("kernel32.dll")]
    private static extern uint SetThreadExecutionState(uint esFlags);

    private const uint ES_CONTINUOUS = 0x80000000;
    private const uint ES_SYSTEM_REQUIRED = 0x00000001;
    private const uint ES_DISPLAY_REQUIRED = 0x00000002;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = FPS;
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod; //Time.realtimeSinceStartup获取游戏开始到当前的时间，增加一个测量间隔，计算出下一次帧率计算是要在什么时候

        // 阻止系统进入省电模式
        SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
    }

    private void Update()
    {
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)    //当前时间超过了下一次的计算时间
        {
            m_FpsNextPeriod += fpsMeasurePeriod;    //在增加下一次的间隔
            fpsText.text = ((int)(1 / Time.unscaledDeltaTime)).ToString();
        }
    }

    void OnDestroy()
    {
        // 恢复系统默认电源策略
        SetThreadExecutionState(ES_CONTINUOUS);
    }
}
