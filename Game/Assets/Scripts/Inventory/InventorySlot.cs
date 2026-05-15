using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int slotIndex;
    public void OnPointerDown(PointerEventData eventData)
    {
        Inventory.Instance.ChangeActiveSlot(slotIndex);
    }
}
