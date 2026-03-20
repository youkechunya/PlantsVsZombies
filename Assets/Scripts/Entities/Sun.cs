using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sun : MonoBehaviour
{
    Game gamePlay;
    [SerializeField] private bool mouseFocus;

    private void Start()
    {
        gamePlay = new Game();
        gamePlay.GamePlay.Enable();
        gamePlay.GamePlay.Collect.performed += CollectCheck;
    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        if (LevelManager.Instance.autoCollector)
            StartCoroutine(AutoCollector());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Cursor")) mouseFocus = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Cursor")) mouseFocus = false;
    }

    IEnumerator AutoCollector()
    {
        yield return new WaitForSeconds(0.1f);
        Collect();
    }

    public void CollectCheck(InputAction.CallbackContext ctx)
    {
        if (mouseFocus)
        {
            Collect();
        }
    }

    private void Collect()
    {
        AudioManager.Instance.sfxPool.PlaySFX("points");
        transform.DOMove(SunManager.Instance.BankCollectorPos, 1f).OnComplete(() =>
        {
            SunManager.Instance.Sun += 25;
            GameEvents.OnReturnToPool(gameObject.name, gameObject);
        });
        transform.DOScale(new Vector3(0, 0, 0), 1f);
    }

    /// <summary>
    /// 开始产出动画 - 从植物位置抛物线飞出
    /// </summary>
    public void StartSpawn(Vector3 plantPosition, Vector3 targetPosition)
    {
        transform.position = plantPosition;

        // 计算抛物线中点（上方偏移）
        Vector3 midPoint = (plantPosition + targetPosition) / 2;
        float yOffset = Random.Range(0.2f, 0.5f);
        midPoint.y += yOffset;

        // 创建曲线路径
        Vector3[] path = new Vector3[] { plantPosition, midPoint, targetPosition };

        // 抛物线飞出
        transform.DOPath(path, 0.75f, PathType.CatmullRom)
            .SetEase(Ease.OutQuad);
    }
}
