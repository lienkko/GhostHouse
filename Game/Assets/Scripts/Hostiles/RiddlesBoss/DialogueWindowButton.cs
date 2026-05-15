using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueWindowButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        RiddlesBossManager.Instance.SkipDialogue();
    }
}
