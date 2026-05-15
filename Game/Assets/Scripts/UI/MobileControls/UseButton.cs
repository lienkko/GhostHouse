using UnityEngine;
using UnityEngine.EventSystems;

public class UseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isHolding;

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
    }
    public void Hide()
    {
        isHolding = false;
        gameObject.SetActive(false);
    }
}
