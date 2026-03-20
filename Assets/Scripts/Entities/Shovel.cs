using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Shovel : MonoBehaviour
{
    Game gamePlay;
    // 是否获取焦点
    bool focus;
    // 获取到焦点的物体
    private BasePlant currentFocusPlant;

    [SerializeField] ShovelMovement shovel;

    [SerializeField] private LayerMask plantLayer;

    private void Start()
    {
        gamePlay = new Game();
        gamePlay.GamePlay.Enable();
        gamePlay.GamePlay.Remove.performed += Remove;
    }

    void Remove(InputAction.CallbackContext ctx)
    {
        if (!focus)
            return;

        if (currentFocusPlant == null)
            shovel.CancelSelect();
        else
        {
            AudioManager.Instance.sfxPool.PlaySFX("plant2");
            Vector3Int cellPosition = currentFocusPlant.cellPosition;
            LevelManager.Instance.UpdateGrid(cellPosition, 1);
            GameEvents.OnReturnToPool(currentFocusPlant.name, currentFocusPlant.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((plantLayer.value & (1 << collision.gameObject.layer)) == 0)
            return;

        if (!collision.TryGetComponent<BasePlant>(out var plant)) return;

        focus = true;

        // 切换焦点：先恢复旧的
        if (currentFocusPlant != null && currentFocusPlant != plant)
        {
            currentFocusPlant.UpdateLight(1f);
        }

        currentFocusPlant = plant;
        currentFocusPlant.UpdateLight(4f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((plantLayer.value & (1 << collision.gameObject.layer)) == 0)
            return;

        if (!collision.TryGetComponent<BasePlant>(out var plant)) return;

        focus = false;

        if (currentFocusPlant == plant)
        {
            currentFocusPlant.UpdateLight(1f);
            currentFocusPlant = null;
        }
    }
}
