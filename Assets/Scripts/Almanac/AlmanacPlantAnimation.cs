using UnityEngine;

/// <summary>
/// 这脚本真正有作用的地方是灰烬植物的动画机
/// </summary>
public class AlmanacPlantAnimation : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // 检查 Tag 标签
        if (CompareTag("Almanac"))
        {
            // 查看 Animator 里是否有 almanac 参数
            if (HasParameter("almanac"))
                anim.SetTrigger("almanac");
            else
                anim.SetTrigger("idle");
        }
    }

    /// <summary>
    /// 查看当前物体动画机里是否含有所需参数
    /// </summary>
    /// <param name="paramName"></param>
    /// <returns></returns>
    private bool HasParameter(string paramName)
    {
        // 从动画机中含有的所有参数中查找
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}
