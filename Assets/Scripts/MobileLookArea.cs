using UnityEngine;
using UnityEngine.EventSystems;

public class MobileLookArea : MonoBehaviour,
    IDragHandler,
    IPointerUpHandler
{
    public float sensitivity = 0.2f;

    public void OnDrag(PointerEventData eventData)
    {
        MobileSuperCheat.Instance.lookX =
            eventData.delta.x * sensitivity;

        MobileSuperCheat.Instance.lookY =
            eventData.delta.y * sensitivity;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MobileSuperCheat.Instance.lookX = 0;
        MobileSuperCheat.Instance.lookY = 0;
    }
}