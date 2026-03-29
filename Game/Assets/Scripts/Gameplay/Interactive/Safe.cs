using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Safe : MonoBehaviour, IInteractive
{
    private readonly string[] _puzzleNames = new string[] { "Circles", "Star" };
    public static bool IsInPuzzle { get; private set; } = false;

    private GameObject _puzzle;
    private DoorController _doorToOpen;
    [SerializeField] private Sprite _rightLeftSafeSprite;
    [SerializeField] private Sprite _topSafeSprite;
    [SerializeField] private Sprite _botSafeSprite;

    [SerializeField] private Collider2D _borderCollider;

    // Interactive fields
    public override void Interact()
    {
        OpenPuzzle();
    }
    public bool CanInteract()
    {
        return GameManager.Instance.CanUseKeyboard && IsInteractive;
    }
    // -------------------

    private void Awake()
    {
        IsInteractive = true;
        Pause.OnResume += SafeOnResume;
    }


    private void Start()
    {
        PlayerController.Instance.OnDeath += ClosePuzzle;
        HintField = GameManager.Instance.GameUIFields.OpenSafeText;
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
        string puzzleName = $"Prefabs/Puzzles/Puzzle{_puzzleNames[Random.Range(0, 2)]}";
        _puzzle = Instantiate<GameObject>(Resources.Load<GameObject>(puzzleName));
        _puzzle.transform.SetParent(gameObject.transform);
        _puzzle.transform.Find("Canvas/CompleteButton").GetComponent<Button>().onClick.AddListener(OpenSafe);
    }

    private void OpenPuzzle()
    {
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(SwitchIsInPuzzle(true));

        GameManager.Instance.BlockPlayer(true);
        if (PlayerHand.Instance.ActiveItem)
        {
            PlayerHand.Instance.ActiveItem.Hide();
        }

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
        GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);
        if (_puzzle)
        {
            StartCoroutine(SwitchIsInPuzzle(false));
            _puzzle.SetActive(false);
        }
        GameManager.Instance.BlockPlayer(false);
        if (PlayerHand.Instance.ActiveItem)
        {
            PlayerHand.Instance.ActiveItem.Unhide();
        }
    }

    private IEnumerator SwitchIsInPuzzle(bool state)
    {
        yield return null;
        IsInteractive = false;
        IsInPuzzle = state;
        GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);
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
            GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);
        IsInteractive = false;
        _doorToOpen.UnlockDoor();
        Destroy(this);
    }

}
