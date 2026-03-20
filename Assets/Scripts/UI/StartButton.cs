using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public RectTransform selectedBoard;

    public GameObject startLight;

    public GameObject startText;

    public GameObject lawnMower;

    public void OnPointerEnter(PointerEventData eventData)
    {
        startLight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        startLight.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 将所有选中植物的不可选中遮罩启用
        for (int i = 0; i < SeedManager.Instance.seedList.Count; i++)
        {
            SeedManager.Instance.seedList[i].GetComponent<Seed>().disable.SetActive(true);
        }
        StartCoroutine(StartGameIEnumerator());
    }

    IEnumerator StartGameIEnumerator()
    {
        // 记录游戏状态
        LevelManager.Instance.IsStart = true;
        selectedBoard.DOAnchorPos(new Vector2(-304, -1130), 0.3f);
        yield return new WaitForSeconds(0.5f);
        // 移动相机
        yield return GlobalManager.Instance.mainCamera.transform.DOMoveX(0, 1f)
            .SetEase(Ease.InQuad)
            .WaitForCompletion();
        yield return new WaitForSeconds(1.5f);
        // 生成小推车
        float y = 1.6f;
        for (int i = 0; i < 5; i++)
        {
            GameObject lawnMower = Instantiate(this.lawnMower, new Vector2(-3.7f, y), Quaternion.identity);
            y -= 1f;
            lawnMower.transform.DOMoveX(-3.18f, 0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        startText.SetActive(true);
        yield return new WaitForSeconds(2f);
        // 将所有物体返回对象池
        ObjectPool.Instance.ReturnAll();
        GlobalManager.Instance.InTheGame = true;
        // 播放BGM
        GameEvents.OnBGMPlay(LevelManager.Instance.levelConfig.bgm);
        // LevelManager.Instance.OriginPlant();
        LevelManager.Instance.shovelBank.SetActive(true);
        // 初始化阳光
        GameEvents.OnSunManagerInitial?.Invoke();
        yield return new WaitForSeconds(18f);
        LevelManager.Instance.ZombieSpawner();
    }
}
