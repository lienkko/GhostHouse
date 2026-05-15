using UnityEngine;
using UnityEngine.EventSystems;

public class RiddleVariantsSlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int slotIndex;
    public void OnPointerDown(PointerEventData eventData)
    {
        RiddlesBossManager.Instance.AnswerWithButton(slotIndex);
    }
}
