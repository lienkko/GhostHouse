using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Safe : MonoBehaviour, IInteractive
{
    private readonly string[] _puzzleNames = new string[] { /*"Circles", "Star",*/ "ColorSequence" };
    public static bool IsInPuzzle { get; private set; } = false;
    public KeyCode KeyToInteract { get; } = KeyCode.E;
    public string HintText { get; } = "Open - E";
    public bool IsInteractive { get; private set; } = true;

    private GameObject _puzzle;
    private DoorController _doorToOpen;
    [SerializeField] private Sprite _rightLeftSafeSprite;
    [SerializeField] private Sprite _topSafeSprite;
    [SerializeField] private Sprite _botSafeSprite;

    [SerializeField] private Collider2D _borderCollider;

    // Interactive fields
    public void Interact()
    {
        OpenPuzzle();
    }
    public bool CanInteract()
    {
        return GameManager.CanUseKeyboard && IsInteractive;
    }
    // -------------------

    private void Awake()
    {
        Pause.OnResume += SafeOnResume;
    }


    private void Start()
    {
        PlayerController.Instance.OnDeath += ClosePuzzle;
    }

    private bool CanClosePuzzle() { return !Pause.IsPaused && IsInPuzzle && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) && !Console.Instance.IsConsoleOpened; }

    private void Update()
    {
        if (CanClosePuzzle())
        {
            ClosePuzzle();
            return;
        }
    }

    public void Initialize(string pointTag, DoorController door)
    {
        _doorToOpen = door;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        switch (pointTag)
        {
            case "TopPoint":
                spriteRenderer.sprite = _topSafeSprite;
                break;
            case "BotPoint":
                _borderCollider.offset = new Vector2(0, -0.12f);
                spriteRenderer.sortingOrder = 11;

                spriteRenderer.sprite = _botSafeSprite;
                break;
            case "RightPoint":
                spriteRenderer.sprite = _rightLeftSafeSprite;
                break;
            case "LeftPoint":
                {
                    spriteRenderer.flipX = true;
                    spriteRenderer.sprite = _rightLeftSafeSprite;
                    break;
                }
        }
    }

    private void CreatePuzzle()
    {
        string puzzleName = $"Prefabs/Puzzles/Puzzle{_puzzleNames[Random.Range(0, _puzzleNames.Length)]}";
        _puzzle = Instantiate<GameObject>(Resources.Load<GameObject>(puzzleName));
        _puzzle.transform.SetParent(gameObject.transform);
        _puzzle.transform.Find("Canvas/CompleteButton").GetComponent<Button>().onClick.AddListener(OpenSafe);
    }

    private void OpenPuzzle()
    {
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(SwitchIsInPuzzle(true));

        GameManager.Instance.BlockPlayer(true);
        Inventory.Instance.HideActiveItem();

        if (_puzzle)
        {
            _puzzle.SetActive(true);
            return;
        }
        CreatePuzzle();
    }

    public void ClosePuzzle()
    {
        if (PlayerController.Instance.IsAlive)
            Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.GameUIFields.HintFieldText.SetActive(false);
        if (_puzzle)
        {
            StartCoroutine(SwitchIsInPuzzle(false));
            _puzzle.SetActive(false);
        }
        GameManager.Instance.BlockPlayer(false);
        Inventory.Instance.ShowActiveItem();
    }

    private IEnumerator SwitchIsInPuzzle(bool state)
    {

        yield return null;
        IsInteractive = !state;
        IsInPuzzle = state;
        GameManager.Instance.GameUIFields.HintFieldText.SetActive(!state);
    }

    private void SafeOnResume()
    {
        if (IsInPuzzle)
        {
            GameManager.Instance.BlockPlayer(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OpenSafe()
    {
        if (_puzzle)
        {
            ClosePuzzle();
            Destroy(_puzzle);
        }
        else
            GameManager.Instance.GameUIFields.HintFieldText.SetActive(false);
        IsInteractive = false;
        _doorToOpen.UnlockDoor();
        Destroy(this);
    }

}
