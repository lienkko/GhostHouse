using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{

    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField, Range(0.1f, 0.6f)] private float handleSizeFactor = 0.35f;
    [SerializeField, Range(0.01f, 0.5f)] private float deadZone = 0.1f;

    private Vector2 inputVector;
    private Vector2 direction;
    private bool isPressed;
    private int activePointerId = -1;
    private Camera activeEventCamera;

    public Vector2 InputDirection
    {
        get { return inputVector; }
    }

    public Vector2 Direction
    {
        get { return direction; }
    }

    public void Hide()
    {
        direction = Vector2.zero;
        gameObject.SetActive(false);
    }
    private void Awake()
    {
        UpdateHandleSize();
    }

    private void OnValidate()
    {
        if (background == null || handle == null)
        {
            return;
        }

        UpdateHandleSize();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        activePointerId = eventData.pointerId;
        activeEventCamera = eventData.pressEventCamera;
        UpdateFromScreenPosition(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPressed || eventData.pointerId != activePointerId)
        {
            return;
        }

        UpdateFromScreenPosition(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
        {
            return;
        }

        isPressed = false;
        activePointerId = -1;
        activeEventCamera = null;
        inputVector = Vector2.zero;
        direction = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (!isPressed)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.fingerId == activePointerId)
                {
                    UpdateFromScreenPosition(touch.position);
                    return;
                }
            }
        }
        else if (activePointerId == -1 && Input.GetMouseButton(0))
        {
            UpdateFromScreenPosition(Input.mousePosition);
        }
    }

    private void UpdateFromScreenPosition(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            screenPosition,
            activeEventCamera,
            out Vector2 position
        );

        Vector2 backgroundSize = background.rect.size;
        if (backgroundSize.x <= 0f || backgroundSize.y <= 0f)
        {
            return;
        }

        position.x /= backgroundSize.x;
        position.y /= backgroundSize.y;

        inputVector = new Vector2(position.x * 2, position.y * 2);

        if (inputVector.sqrMagnitude < deadZone * deadZone)
        {
            direction = Vector2.zero;
            inputVector = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            return;
        }

        inputVector = inputVector.normalized;
        direction = SnapToEightDirections(inputVector);
        inputVector = direction;

        float radius = Mathf.Min(backgroundSize.x, backgroundSize.y) * 0.5f;
        float handleRadius = Mathf.Min(handle.sizeDelta.x, handle.sizeDelta.y) * 0.5f;
        float distance = Mathf.Max(0f, radius - handleRadius);

        handle.anchoredPosition = inputVector * distance;
    }

    private void UpdateHandleSize()
    {
        Vector2 backgroundSize = background.rect.size;
        float size = Mathf.Min(backgroundSize.x, backgroundSize.y) * handleSizeFactor;
        handle.sizeDelta = new Vector2(size, size);
    }

    private static Vector2 SnapToEightDirections(Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        if (angle < 0f)
        {
            angle += 360f;
        }

        if (angle >= 337.5f || angle < 22.5f) return Vector2.right;
        if (angle < 67.5f) return new Vector2(1f, 1f).normalized;
        if (angle < 112.5f) return Vector2.up;
        if (angle < 157.5f) return new Vector2(-1f, 1f).normalized;
        if (angle < 202.5f) return Vector2.left;
        if (angle < 247.5f) return new Vector2(-1f, -1f).normalized;
        if (angle < 292.5f) return Vector2.down;
        return new Vector2(1f, -1f).normalized;
    }
}
