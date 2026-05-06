using UnityEngine;
using UnityEngine.UI;

public enum PipeType
{
    Empty,
    End,
    Straight,
    Corner,
    Tee
}

public enum PipeDirection
{
    Up,
    Right,
    Down,
    Left
}

[RequireComponent(typeof(Button))]
public class PipeTile : MonoBehaviour
{
    private const int UpMask = 1;
    private const int RightMask = 2;
    private const int DownMask = 4;
    private const int LeftMask = 8;

    [Header("Links")]
    [SerializeField] private Button _button;
    [SerializeField] private RectTransform _pipeVisual;
    [SerializeField] private Image _pipeImage;

    [Header("Sprites")]
    [SerializeField] private Sprite _endSprite;
    [SerializeField] private Sprite _straightSprite;
    [SerializeField] private Sprite _cornerSprite;
    [SerializeField] private Sprite _teeSprite;

    [Header("Tile Settings")]
    [SerializeField] private PipeType _pipeType = PipeType.Empty;
    [SerializeField][Range(0, 3)] private int _startRotation;
    [SerializeField] private bool _rotatable = true;

    private int _currentRotation;
    private RotationPuzzle _owner;

    public PipeType Type => _pipeType;
    public bool IsEmpty => _pipeType == PipeType.Empty;
    public int CurrentRotation => _currentRotation;

    private void Awake()
    {
        CacheLinks();

        if (_button != null)
        {
            _button.onClick.RemoveListener(OnTileClicked);
            _button.onClick.AddListener(OnTileClicked);
        }

        ResetTile();
    }

    private void Start()
    {
        if (_owner == null)
            _owner = GetComponentInParent<RotationPuzzle>();
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(OnTileClicked);
    }

    public void Bind(RotationPuzzle owner)
    {
        _owner = owner;
    }

    public void Configure(PipeType pipeType, int startRotation, bool rotatable)
    {
        _pipeType = pipeType;
        _startRotation = NormalizeRotation(startRotation);
        _rotatable = rotatable;
        ResetTile();
    }

    public void ResetTile()
    {
        CacheLinks();
        _currentRotation = NormalizeRotation(_startRotation);
        RefreshVisual();
    }

    public void Rotate90()
    {
        if (!_rotatable || IsEmpty)
            return;

        _currentRotation = NormalizeRotation(_currentRotation + 1);
        RefreshVisual();

        if (_owner != null)
            _owner.CheckPuzzle();
    }

    public bool IsOpen(PipeDirection direction)
    {
        int mask = GetRotatedMask();

        switch (direction)
        {
            case PipeDirection.Up:
                return (mask & UpMask) != 0;

            case PipeDirection.Right:
                return (mask & RightMask) != 0;

            case PipeDirection.Down:
                return (mask & DownMask) != 0;

            case PipeDirection.Left:
                return (mask & LeftMask) != 0;

            default:
                return false;
        }
    }

    private void OnTileClicked()
    {
        Rotate90();
    }

    private void RefreshVisual()
    {
        if (_pipeImage != null)
        {
            _pipeImage.sprite = GetSpriteForType();
            _pipeImage.enabled = _pipeType != PipeType.Empty;
        }

        if (_pipeVisual != null)
            _pipeVisual.localRotation = Quaternion.Euler(0f, 0f, -90f * _currentRotation);

        if (_button != null)
            _button.interactable = _rotatable && !IsEmpty;
    }

    private Sprite GetSpriteForType()
    {
        switch (_pipeType)
        {
            case PipeType.End:
                return _endSprite;

            case PipeType.Straight:
                return _straightSprite;

            case PipeType.Corner:
                return _cornerSprite;

            case PipeType.Tee:
                return _teeSprite;

            default:
                return null;
        }
    }

    private int GetBaseMask()
    {
        switch (_pipeType)
        {
            case PipeType.End:
                return RightMask;

            case PipeType.Straight:
                return UpMask | DownMask;

            case PipeType.Corner:
                return DownMask | LeftMask;

            case PipeType.Tee:
                return RightMask | DownMask | LeftMask;

            default:
                return 0;
        }
    }

    private int GetRotatedMask()
    {
        int mask = GetBaseMask();

        for (int i = 0; i < _currentRotation; i++)
            mask = RotateMaskClockwise(mask);

        return mask;
    }

    private int RotateMaskClockwise(int mask)
    {
        int rotated = 0;

        if ((mask & UpMask) != 0)
            rotated |= RightMask;

        if ((mask & RightMask) != 0)
            rotated |= DownMask;

        if ((mask & DownMask) != 0)
            rotated |= LeftMask;

        if ((mask & LeftMask) != 0)
            rotated |= UpMask;

        return rotated;
    }

    private int NormalizeRotation(int rotation)
    {
        rotation %= 4;

        if (rotation < 0)
            rotation += 4;

        return rotation;
    }

    private void CacheLinks()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if (_pipeVisual == null)
        {
            Transform visual = transform.Find("PipeVisual");
            if (visual != null)
                _pipeVisual = visual as RectTransform;
        }

        if (_pipeVisual == null)
            _pipeVisual = transform as RectTransform;

        if (_pipeImage == null && _pipeVisual != null)
            _pipeImage = _pipeVisual.GetComponent<Image>();

        if (_pipeImage == null)
            _pipeImage = GetComponent<Image>();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CacheLinks();
        _currentRotation = NormalizeRotation(_startRotation);
        RefreshVisual();
    }
#endif
}
