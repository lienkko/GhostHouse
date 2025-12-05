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
        _offset = pos - new Vector2(_consoleWindow.anchoredPosition.x,_consoleWindow.anchoredPosition.y);
    }


    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_consoleCanvas.transform as RectTransform, eventData.position, _consoleCanvas.worldCamera, out Vector2 pos);

        float minX = _offset.x - _consoleCanvas.GetComponent<RectTransform>().rect.width / 2 - _consoleWindow.rect.width/2;
        float maxX = _consoleCanvas.GetComponent<RectTransform>().rect.width / 2 + _offset.x - _consoleWindow.rect.width/2;
        float minY = _consoleWindow.rect.height / 2 - _consoleCanvas.GetComponent<RectTransform>().rect.height/2;
        float maxY = _consoleCanvas.GetComponent<RectTransform>().rect.height / 2 + _offset.y;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        _consoleWindow.anchoredPosition = pos-_offset;
    }
}
