using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Reflection;

public class PuzzleStarDot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Image _image;
    private RectTransform _rt;
    private float _realX;

    public bool _isMoving;

    private void Awake()
    {
        _isMoving = false;
        _image = GetComponent<Image>();
        _rt = GetComponent<RectTransform>();
        _realX = _rt.anchoredPosition.x;
    }

    private Vector3 GetPosOnLine(float x, float y)
    {
        y = 0.412f * x - 80.34f;
        return new Vector3(x, y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rt.anchoredPosition += eventData.delta;
        _realX += eventData.delta.x;
        Debug.Log(_realX);
        _rt.anchoredPosition = GetPosOnLine(_rt.anchoredPosition.x,_rt.anchoredPosition.y);
        if (_realX < -137f)
        {
            _rt.anchoredPosition = new Vector3(-137, -137, 0);
        }
        else if (_realX > 195f)
            _rt.anchoredPosition = new Vector3(195, 0, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _image.raycastTarget = true;
    }

}
