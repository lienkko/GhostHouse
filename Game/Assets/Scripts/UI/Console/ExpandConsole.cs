using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExpandConsole : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform _consoleWindow;
    [SerializeField] private Texture2D _nwseCursor;
    private Vector2 _offset;
    private bool _mouseIsInCorner;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(_nwseCursor, new Vector2(11, 11), CursorMode.Auto);
        _mouseIsInCorner = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _mouseIsInCorner = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _consoleWindow.parent as RectTransform,
            eventData.position,
            null,
            out Vector2 mouse);
        _offset = new Vector2(
            mouse.x - _consoleWindow.anchoredPosition.x - _consoleWindow.rect.width,
            _consoleWindow.anchoredPosition.y - mouse.y - _consoleWindow.rect.height);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Cursor.SetCursor(_nwseCursor, new Vector2(11,11), CursorMode.Auto);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _consoleWindow.parent as RectTransform,
            eventData.position,
            null,
            out Vector2 mouse);
        Rect canvasRect = _consoleWindow.parent.GetComponent<RectTransform>().rect;
        RectTransform _canvasRectT = _consoleWindow.parent as RectTransform;

        float width = Mathf.Clamp(mouse.x - _consoleWindow.anchoredPosition.x, 400, 
            canvasRect.width);
        float height = Mathf.Clamp(_consoleWindow.anchoredPosition.y - mouse.y,300,
            canvasRect.height);

        _consoleWindow.sizeDelta = new Vector2(width,
            height) - _offset;
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_mouseIsInCorner)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
