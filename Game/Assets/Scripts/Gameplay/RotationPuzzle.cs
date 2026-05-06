using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RotationPuzzle : MonoBehaviour
{
    private const int GridSize = 5;
    private const int CellsCount = GridSize * GridSize;

    public enum PuzzlePattern
    {
        Pattern1,
        Pattern2,
        Pattern3
    }

    private static readonly string[][] Patterns =
    {
        new[]
        {
            "E,S,S,S,C",
            "C,S,S,S,C",
            "C,S,S,S,C",
            "C,S,S,S,C",
            "C,S,S,S,E"
        },
        new[]
        {
            "E,S,S,S,C",
            "C,S,S,C,S",
            "S,C,E,S,S",
            "S,C,S,C,S",
            "C,S,S,S,C"
        },
        new[]
        {
            "E,S,T,S,E",
            "C,S,T,S,E",
            "C,S,T,S,E",
            "E,S,T,S,C",
            "E,S,S,S,C"
        }
    };

    [Header("Grid")]
    [SerializeField] private Transform _gridRoot;
    [SerializeField] private PipeTile[] _tiles = new PipeTile[CellsCount];

    [Header("Pattern")]
    [SerializeField] private PuzzlePattern _selectedPattern = PuzzlePattern.Pattern1;
    [SerializeField] private bool _loadPatternOnStart = true;
    [SerializeField] private bool _shuffleOnStart = true;
    [SerializeField] [Range(1, 20)] private int _shuffleAttempts = 8;

    [Header("Input")]
    [SerializeField] private bool _manageCursorWhileActive = true;

    [Header("UI")]
    [SerializeField] private GameObject _completeButton;
    [SerializeField] private Sprite[] _buttonSprites = new Sprite[2];
    [SerializeField] private Text _statusText;
    [SerializeField] private Color _lockedButtonColor = new Color(0.65f, 0.65f, 0.65f, 0.7f);
    [SerializeField] private Color _readyButtonColor = Color.white;

    [Header("Completion")]
    [SerializeField] private UnityEvent _onPuzzleCompleted;

    private Button _completeButtonButton;
    private Image _completeButtonImage;
    private CursorLockMode _previousCursorLockMode;
    private bool _previousCursorVisible;
    private bool _storedCursorState;
    private bool _isSolved;

    private void Awake()
    {
        CacheUi();
        RegisterCompleteButton();
    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
            return;

        EnablePuzzleCursor();
    }

    private void Start()
    {
        CacheUi();
        RegisterCompleteButton();
        BindTiles();

        if (_loadPatternOnStart)
            SetupPuzzle();
        else
            CheckPuzzle();
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying || !_manageCursorWhileActive)
            return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
            return;

        RestoreCursorState();
    }

    private void OnDestroy()
    {
        if (_completeButtonButton != null)
            _completeButtonButton.onClick.RemoveListener(CompletePuzzle);
    }

    public void SetupPuzzle()
    {
        if (!EnsureTilesReady())
            return;

        ApplySelectedPattern();

        if (_shuffleOnStart)
            ShuffleTiles();

        CheckPuzzle();
    }

    public void CheckPuzzle()
    {
        _isSolved = IsSolved();
        UpdateCompleteButton(_isSolved);
        SetStatus(_isSolved ? "GO!" : "Assemble one pipe system");
    }

    public void LoadPattern(PuzzlePattern pattern)
    {
        _selectedPattern = pattern;
        SetupPuzzle();
    }

    public void CompletePuzzle()
    {
        CheckPuzzle();

        if (!_isSolved)
            return;

        _onPuzzleCompleted?.Invoke();
        ClosePuzzleAfterCompletion();
    }

    [ContextMenu("Load Selected Pattern")]
    private void LoadSelectedPatternFromContext()
    {
        if (!EnsureTilesReady())
            return;

        ApplySelectedPattern();
        CheckPuzzle();
    }

    [ContextMenu("Shuffle Current Pattern")]
    private void ShuffleCurrentPatternFromContext()
    {
        if (!EnsureTilesReady())
            return;

        ShuffleTiles();
        CheckPuzzle();
    }

    [ContextMenu("Collect Tiles From Grid")]
    private void CollectTilesFromGrid()
    {
        if (_gridRoot == null)
        {
            Transform gridTransform = transform.Find("Grid");
            if (gridTransform != null)
                _gridRoot = gridTransform;
        }

        List<PipeTile> collectedTiles = new List<PipeTile>(CellsCount);

        if (_gridRoot != null)
        {
            for (int i = 0; i < _gridRoot.childCount; i++)
            {
                PipeTile tile = _gridRoot.GetChild(i).GetComponent<PipeTile>();
                if (tile != null)
                    collectedTiles.Add(tile);
            }
        }
        else
        {
            collectedTiles.AddRange(GetComponentsInChildren<PipeTile>(true));
        }

        _tiles = collectedTiles.ToArray();
        Debug.Log($"RotationPuzzle: found {_tiles.Length} tiles.");
    }

    private void ApplySelectedPattern()
    {
        string[] rows = Patterns[(int)_selectedPattern];

        for (int row = 0; row < GridSize; row++)
        {
            string[] cells = rows[row].Split(',');

            for (int column = 0; column < GridSize; column++)
            {
                int index = row * GridSize + column;
                PipeType pipeType = ParsePipeType(cells[column]);

                _tiles[index].Bind(this);
                _tiles[index].Configure(pipeType, 0, pipeType != PipeType.Empty);
            }
        }
    }

    private void ShuffleTiles()
    {
        for (int attempt = 0; attempt < _shuffleAttempts; attempt++)
        {
            for (int i = 0; i < _tiles.Length; i++)
            {
                PipeTile tile = _tiles[i];
                int randomRotation = tile.IsEmpty ? 0 : Random.Range(0, 4);
                tile.Configure(tile.Type, randomRotation, !tile.IsEmpty);
                tile.Bind(this);
            }

            if (!IsSolved())
                return;
        }
    }

    private PipeType ParsePipeType(string token)
    {
        switch (token.Trim())
        {
            case "E":
                return PipeType.End;

            case "S":
                return PipeType.Straight;

            case "C":
                return PipeType.Corner;

            case "T":
                return PipeType.Tee;

            default:
                return PipeType.Empty;
        }
    }

    private bool IsSolved()
    {
        List<int> usedTiles = new List<int>(CellsCount);

        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] != null && !_tiles[i].IsEmpty)
                usedTiles.Add(i);
        }

        if (usedTiles.Count == 0)
            return false;

        for (int i = 0; i < usedTiles.Count; i++)
        {
            int index = usedTiles[i];

            for (int direction = 0; direction < 4; direction++)
            {
                PipeDirection pipeDirection = (PipeDirection)direction;

                if (!_tiles[index].IsOpen(pipeDirection))
                    continue;

                if (!TryGetNeighborIndex(index, pipeDirection, out int neighborIndex))
                    return false;

                if (_tiles[neighborIndex].IsEmpty)
                    return false;

                if (!_tiles[neighborIndex].IsOpen(GetOpposite(pipeDirection)))
                    return false;
            }
        }

        Queue<int> queue = new Queue<int>();
        HashSet<int> visited = new HashSet<int>();

        int startIndex = usedTiles[0];
        queue.Enqueue(startIndex);
        visited.Add(startIndex);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();

            for (int direction = 0; direction < 4; direction++)
            {
                PipeDirection pipeDirection = (PipeDirection)direction;

                if (!_tiles[current].IsOpen(pipeDirection))
                    continue;

                if (!TryGetNeighborIndex(current, pipeDirection, out int neighborIndex))
                    continue;

                if (_tiles[neighborIndex].IsEmpty)
                    continue;

                if (!_tiles[neighborIndex].IsOpen(GetOpposite(pipeDirection)))
                    continue;

                if (visited.Add(neighborIndex))
                    queue.Enqueue(neighborIndex);
            }
        }

        return visited.Count == usedTiles.Count;
    }

    private bool TryGetNeighborIndex(int index, PipeDirection direction, out int neighborIndex)
    {
        int row = index / GridSize;
        int column = index % GridSize;

        switch (direction)
        {
            case PipeDirection.Up:
                row--;
                break;

            case PipeDirection.Right:
                column++;
                break;

            case PipeDirection.Down:
                row++;
                break;

            case PipeDirection.Left:
                column--;
                break;
        }

        if (row < 0 || row >= GridSize || column < 0 || column >= GridSize)
        {
            neighborIndex = -1;
            return false;
        }

        neighborIndex = row * GridSize + column;
        return true;
    }

    private PipeDirection GetOpposite(PipeDirection direction)
    {
        switch (direction)
        {
            case PipeDirection.Up:
                return PipeDirection.Down;

            case PipeDirection.Right:
                return PipeDirection.Left;

            case PipeDirection.Down:
                return PipeDirection.Up;

            case PipeDirection.Left:
                return PipeDirection.Right;

            default:
                return PipeDirection.Up;
        }
    }

    private bool EnsureTilesReady()
    {
        CacheUi();
        RegisterCompleteButton();

        if (_gridRoot != null && (_tiles == null || _tiles.Length != CellsCount || HasMissingTiles()))
            CollectTilesFromGrid();

        if (_tiles == null || _tiles.Length != CellsCount)
        {
            Debug.LogError($"RotationPuzzle: need exactly {CellsCount} tiles.");
            UpdateCompleteButton(false);
            SetStatus($"Error: need {CellsCount} tiles");
            return false;
        }

        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] == null)
            {
                Debug.LogError($"RotationPuzzle: tile at index {i} is missing.");
                UpdateCompleteButton(false);
                SetStatus("Error: some tiles are missing");
                return false;
            }

            _tiles[i].Bind(this);
        }

        return true;
    }

    private bool HasMissingTiles()
    {
        if (_tiles == null)
            return true;

        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] == null)
                return true;
        }

        return false;
    }

    private void BindTiles()
    {
        if (_tiles == null)
            return;

        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] != null)
                _tiles[i].Bind(this);
        }
    }

    private void UpdateCompleteButton(bool solved)
    {
        if (_completeButtonButton != null)
            _completeButtonButton.interactable = solved;

        if (_completeButtonImage != null && _buttonSprites != null && _buttonSprites.Length >= 2)
            _completeButtonImage.sprite = _buttonSprites[solved ? 1 : 0];

        if (_completeButtonImage != null)
            _completeButtonImage.color = solved ? _readyButtonColor : _lockedButtonColor;
    }

    private void SetStatus(string message)
    {
        if (_statusText != null)
            _statusText.text = message;
    }

    private void CacheUi()
    {
        if (_completeButton != null)
        {
            if (_completeButtonButton == null)
                _completeButtonButton = _completeButton.GetComponent<Button>();

            if (_completeButtonImage == null)
                _completeButtonImage = _completeButton.GetComponent<Image>();
        }
    }

    private void RegisterCompleteButton()
    {
        if (_completeButtonButton == null)
            return;

        _completeButtonButton.onClick.RemoveListener(CompletePuzzle);
        _completeButtonButton.onClick.AddListener(CompletePuzzle);
    }

    private void ClosePuzzleAfterCompletion()
    {
        if (GameManager.Instance != null && PlayerController.Instance != null)
            GameManager.Instance.BlockPlayer(false);

        if (PlayerHand.Instance != null && PlayerHand.Instance.ActiveItem != null)
            PlayerHand.Instance.ActiveItem.Unhide();

        if (PlayerController.Instance != null && PlayerController.Instance.IsAlive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private void EnablePuzzleCursor()
    {
        if (!_manageCursorWhileActive)
            return;

        if (!_storedCursorState)
        {
            _previousCursorLockMode = Cursor.lockState;
            _previousCursorVisible = Cursor.visible;
            _storedCursorState = true;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void RestoreCursorState()
    {
        if (!_manageCursorWhileActive || !_storedCursorState)
            return;

        bool shouldKeepCursorAvailable =
            Pause.IsPaused ||
            (Console.Instance != null && Console.Instance.IsConsoleOpened) ||
            (PlayerController.Instance != null && !PlayerController.Instance.IsAlive);

        if (shouldKeepCursorAvailable)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = _previousCursorLockMode;
            Cursor.visible = _previousCursorVisible;
        }

        _storedCursorState = false;
    }
}
