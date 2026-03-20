using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ShovelMovement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform bankPosition;

    Game gamePlay;

    bool select;
    bool mouseFocus;

    private void OnEnable()
    {
        gamePlay = new Game();
        gamePlay.GamePlay.Enable();
        gamePlay.GamePlay.Select.performed += Selected;
        gamePlay.GamePlay.Cancel.performed += Cancel;
    }

    private void Update()
    {
        if (select)
        {
            Vector3 mousePosition = Input.mousePosition + new Vector3(58f,62.5f); 
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePosition);
            targetPos.z = 0;
            transform.position = targetPos;
        }
    }
    private void OnDisable()
    {
        if (gamePlay != null)
        {
            gamePlay.GamePlay.Select.performed -= Selected;
            gamePlay.GamePlay.Cancel.performed -= Cancel;
        }
    }

    private void Selected(InputAction.CallbackContext ctx)
    {
        if (!mouseFocus)
            return;

        select = !select;
        if (!select)
        {
            tag = "Button";
            transform.position = bankPosition.position;
        }
        else
        {
            tag = "Untagged";
            AudioManager.Instance.sfxPool.PlaySFX("shovel");
        }
    }

    private void Cancel(InputAction.CallbackContext ctx)
    {
        CancelSelect();
        AudioManager.Instance.sfxPool.PlaySFX("tap1");
    }

    public void CancelSelect()
    {
        select = false;
        tag = "Button";
        transform.position = bankPosition.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseFocus = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseFocus = false;
    }
}
