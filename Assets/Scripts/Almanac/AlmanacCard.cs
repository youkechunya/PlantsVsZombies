using UnityEngine;

/// <summary>
/// 图鉴卡片（挂载在植物卡片的背景上）
/// </summary>
public class AlmanacCard : MonoBehaviour
{
    // 卡片信息
    [SerializeField] AlmanacInformation info;
    // 图鉴
    private AlmanacBoard board;

    private void Awake()
    {
        // 获取图鉴
        //（第一个parent：对应植物/僵尸名）
        //（第二个parent：Content）
        //（第三个parent：Viewport）
        //（第四个parent：Scroll View）
        //（第五个parent：PlantView/ZombieView）
        //（第六个parent：AlmanacBoard）
        board = transform.parent.parent.parent.parent.parent.parent.GetComponent<AlmanacBoard>();
    }

    public void OnClick()
    {
        board.SetPlantView(info);
    }
}
