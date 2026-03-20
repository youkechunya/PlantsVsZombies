using UnityEngine;

/// <summary>
/// 用于管理 Sprite 的碰撞
/// </summary>
public class CursorTrigger : MonoBehaviour
{
    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
