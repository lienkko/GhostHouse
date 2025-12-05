using UnityEngine;
using UnityEngine.EventSystems;

public class ConsoleDrag : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] private RectTransform _consoleWindow;
    [SerializeField] private Canvas _consoleCanvas;

    private Vector2 _offset;
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_consoleCanvas.transform as RectTransform, eventData.position, _consoleCanvas.worldCamera, out Vector2 pos);
        _offset = pos - _consoleWindow.anchoredPosition;
    }


    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_consoleCanvas.transform as RectTransform, eventData.position, _consoleCanvas.worldCamera, out Vector2 pos);
        float minX = _offset.x - _consoleWindow.rect.width/2;
        float maxX = _consoleCanvas.GetComponent<RectTransform>().rect.width - _consoleWindow.rect.width/2 + _offset.x;
        float minY = _offset.y - _consoleWindow.rect.height / 2;
        float maxY = _consoleCanvas.GetComponent<RectTransform>().rect.height - (_consoleWindow.rect.height-_offset.y);

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        _consoleWindow.anchoredPosition = pos-_offset;
    }
}
