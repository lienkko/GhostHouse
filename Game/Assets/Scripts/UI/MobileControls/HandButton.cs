using UnityEngine;
using UnityEngine.EventSystems;

public class HandButton : MonoBehaviour, IPointerDownHandler
{
    public delegate void HandButtonPressed();
    public event HandButtonPressed OnHandButtonPressed;
    public void OnPointerDown(PointerEventData eventData)
    {
        OnHandButtonPressed?.Invoke();
    }
    public void SetListener(HandButtonPressed listener)
    {
        OnHandButtonPressed = listener;
    }
    public void ClearListener()
    {
        OnHandButtonPressed = null;
    }
    public void Hide()
    {
        ClearListener();
        gameObject.SetActive(false);
    }
}
