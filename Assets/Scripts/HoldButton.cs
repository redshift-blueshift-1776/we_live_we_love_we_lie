using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    public enum ButtonType
    {
        Left,
        Right,
        Forward,
        Backward,
        Jump,
        Sprint,
        Interact
    }

    public ButtonType buttonType;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Left:
                MobileSuperCheat.Instance.horizontal = -1;
                break;

            case ButtonType.Right:
                MobileSuperCheat.Instance.horizontal = 1;
                break;

            case ButtonType.Forward:
                MobileSuperCheat.Instance.vertical = 1;
                break;

            case ButtonType.Backward:
                MobileSuperCheat.Instance.vertical = -1;
                break;

            case ButtonType.Jump:
                MobileSuperCheat.Instance.jumpPressed = true;
                break;

            case ButtonType.Sprint:
                MobileSuperCheat.Instance.sprintHeld = true;
                break;

            case ButtonType.Interact:
                MobileSuperCheat.Instance.interactPressed = true;
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Left:
            case ButtonType.Right:
                MobileSuperCheat.Instance.horizontal = 0;
                break;

            case ButtonType.Forward:
            case ButtonType.Backward:
                MobileSuperCheat.Instance.vertical = 0;
                break;

            case ButtonType.Sprint:
                MobileSuperCheat.Instance.sprintHeld = false;
                break;
        }
    }
}