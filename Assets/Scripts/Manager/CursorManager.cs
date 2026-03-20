using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 鼠标管理器
/// </summary>
public class CursorManager : MonoBehaviour
{
    // 事件系统
    private EventSystem eventSystem;
    // 事件数据
    private PointerEventData pointerData;
    // 图片组件
    Image image;
    // 默认外观
    [SerializeField] private Sprite defaultSprite;
    // 悬浮外观
    [SerializeField] private Sprite hoverSprite;

    private void Awake()
    {
        image = GetComponent<Image>();
        eventSystem = EventSystem.current;
        pointerData = new PointerEventData(eventSystem);
    }

    private void Update()
    {
        // 将系统鼠标隐藏
        Cursor.visible = false;
        // 鼠标偏移
        Vector3 mousePosition = Input.mousePosition + new Vector3(58.5f * transform.localScale.x, -95.5f * transform.localScale.x);
        transform.position = mousePosition;
        // 悬浮检测
        DetectHover();
    }

    private void DetectHover()
    {
        // 更新射线检测的起点位置
        pointerData.position = Input.mousePosition;

        var results = new System.Collections.Generic.List<RaycastResult>();
        // 射线检测（传入 pointerData 并将结果输出到 results）
        eventSystem.RaycastAll(pointerData, results);

        bool foundButton = false;

        foreach (var result in results)
        {
            // 跳过自己
            if (result.gameObject == gameObject) continue;

            // 检测是否是按钮
            if (result.gameObject.CompareTag("Button") ||
                result.gameObject.GetComponent<Button>() != null)
            {
                foundButton = true;
                break;
            }
        }

        // 切换光标样式
        image.sprite = foundButton ? hoverSprite : defaultSprite;
    }
}
