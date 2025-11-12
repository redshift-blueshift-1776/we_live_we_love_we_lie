using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string tooltipMessage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.Instance.ShowTooltip(tooltipMessage);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.HideTooltip();
    }
}