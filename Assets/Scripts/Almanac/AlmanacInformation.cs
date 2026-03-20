using UnityEngine;

[CreateAssetMenu(fileName = "NewAlmanacPlant", menuName = "PVZ/AlmanacPlant")]
public class AlmanacInformation : ScriptableObject
{
    [Header("草坪图片")]
    public Sprite groundSprite;
    [Header("植物")]
    public GameObject plant;
    [Header("植物名称")]
    public string plantName;
    [Header("描述")]
    [TextArea(3,5)]
    public string plantDescription;
    [Header("数据")]
    [TextArea(2, 4)]
    public string plantData;
    [Header("额外数据")]
    [TextArea(2, 4)]
    public string plantExtraData;
    [Header("宝开语")]
    [TextArea(5, 8)]
    public string baokaiLanguage;
}
