using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 图鉴界面（用来管理相关界面的显示）
/// </summary>
public class AlmanacBoard : MonoBehaviour
{
    // 主菜单
    [SerializeField] private GameObject mainMenu;
    // 植物界面
    [SerializeField] private GameObject plantView;

    [Header("植物图鉴")]
    // 第一个展示的植物
    [SerializeField] private GameObject firstPlant;
    // 植物信息框（显示文本的区域）
    [SerializeField] private GameObject plantContent;
    // 植物草坪
    [SerializeField] private Image plantGroundSprite;
    // 存放植物预制体的根
    [SerializeField] private GameObject plantGameObject;
    // 植物名
    [SerializeField] private TMP_Text plantName;
    // 植物描述
    [SerializeField] private TMP_Text plantDescription;
    // 植物数据
    [SerializeField] private TMP_Text plantData;
    // 植物其他数据
    [SerializeField] private TMP_Text plantExtraData;
    // 植物对应的宝开语）
    [SerializeField] private TMP_Text plantBaokaiLanguage;
    [Space(10)]

    [Header("僵尸图鉴")]
    // 僵尸图鉴（与植物图鉴同理，这里不进行展开）
    [SerializeField] private GameObject zombieView;

    // 已进行过展示的植物（相当于图鉴对象池）
    private readonly Dictionary<string, GameObject> showPlants = new();

    private void Awake()
    {
        showPlants.Add("PeaShooter", firstPlant);
    }

    public void ReturnMenu()
    {
        mainMenu.SetActive(true);
        plantView.SetActive(false);
        zombieView.SetActive(false);
    }

    public void ShowPlantView()
    {
        mainMenu.SetActive(false);
        plantView.SetActive(true);
    }

    public void ShowZombieView()
    {
        mainMenu.SetActive(false);
        zombieView.SetActive(true);
    }

    /// <summary>
    /// 设置当前展示植物
    /// </summary>
    /// <param name="info"></param>
    public void SetPlantView(AlmanacInformation info)
    {
        // 获取草坪
        plantGroundSprite.sprite = info.groundSprite;
        // 如果之前已经点击展示过
        if (showPlants.TryGetValue(info.plant.name, out GameObject plant))
        {
            // 将其他植物不可见
            for(int i = 0; i < plantGameObject.transform.childCount; i++)
            {
                plantGameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
            // 展示点击植物
            plant.SetActive(true);
        }
        else
        {
            // 先将展示的植物都不可见
            for (int i = 0; i < plantGameObject.transform.childCount; i++)
            {
                plantGameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
            // 生成对应植物
            GameObject obj = Instantiate(info.plant, plantGameObject.transform);
            // 设置位置
            obj.transform.localPosition = new Vector3(0, 0, 0);
            // 加入对象池
            showPlants.Add(obj.name.Replace("(Clone)","").Trim(), obj);
        }
        // 获取信息
        plantName.SetText(info.plantName);
        plantDescription.SetText(info.plantDescription);
        plantData.SetText(info.plantData);
        plantBaokaiLanguage.SetText(info.baokaiLanguage);
        plantExtraData.SetText(info.plantExtraData);

        // 刷新布局，不然字体的位置会错乱
        ForceRebuildLayout(plantDescription.GetComponent<RectTransform>());
        ForceRebuildLayout(plantData.GetComponent<RectTransform>());
        ForceRebuildLayout(plantBaokaiLanguage.GetComponent<RectTransform>());
        ForceRebuildLayout(plantExtraData.GetComponent<RectTransform>());

        // 设置滚动框的大小
        float descriptionHeight = plantDescription.GetComponent<RectTransform>().rect.height;
        float dataHeight = plantData.GetComponent<RectTransform>().rect.height;
        float baokaiLanguageHeight = plantBaokaiLanguage.GetComponent<RectTransform>().rect.height;
        float extraHeight = plantExtraData.GetComponent<RectTransform>().rect.height;
        float plantContentHeight = descriptionHeight + dataHeight + baokaiLanguageHeight + extraHeight + plantContent.GetComponent<VerticalLayoutGroup>().spacing * 3;
        plantContent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, plantContentHeight);
    }

    // 封装强制刷新布局的方法，复用性更高
    private void ForceRebuildLayout(RectTransform rectTransform)
    {
        // 强制重建布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        // 兼容某些版本的布局刷新延迟问题
        Canvas.ForceUpdateCanvases();
    }
}
