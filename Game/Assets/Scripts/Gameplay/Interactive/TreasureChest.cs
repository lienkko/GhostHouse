using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class TreasureChest : MonoBehaviour, IInteractive
{
    private readonly string _puzzleName = "Prefabs/Puzzles/PuzzleNumbers";
    private GameObject _puzzle;
    private SpriteRenderer _spriteRenderer;

    public static bool IsInPuzzle { get; private set; } = false;
    public KeyCode KeyToInteract { get; } = KeyCode.E;
    public string HintText { get; } = "Open chest - E";
    public bool IsInteractive { get; private set; } = true;


    [Header("Chest Settings")]
    [SerializeField] private Sprite _openedChestSprite;
    [SerializeField] private GameObject _batteryPrefab;
    [SerializeField] private GameObject _bigBobPrefab;
    public void Interact()
    {
        OpenPuzzle();
    }
    public bool CanInteract()
    {
        return GameManager.CanUseKeyboard && IsInteractive;
    }
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        IsInteractive = true;
        Pause.OnResume += ChestOnResume;
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

    private void CreatePuzzle()
    {
        GameObject prefab = Resources.Load<GameObject>(_puzzleName);
        _puzzle = Instantiate(prefab, transform);
        _puzzle.transform.SetParent(gameObject.transform);
        _puzzle.transform.Find("Canvas/CompleteButton").GetComponent<Button>().onClick.AddListener(OpenChest);
        _puzzle.SetActive(true);
        _puzzle.GetComponent<NumberPuzzle>().SetupPuzzle();
    }

    public void OpenPuzzle()
    {
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(SwitchIsInPuzzle(true));

        GameManager.Instance.BlockPlayer(true);
        if (PlayerHand.Instance.ActiveItem)
        {
            PlayerHand.Instance.ActiveItem.HideItem();
        }

        if (_puzzle)
        {
            _puzzle.SetActive(true);
            _puzzle.GetComponent<NumberPuzzle>().SetupPuzzle();
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
            PlayerHand.Instance.ActiveItem.ShowItem();
        }
    }

    private IEnumerator SwitchIsInPuzzle(bool state)
    {
        yield return null;
        IsInteractive = !state;
        IsInPuzzle = state;
        GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);
    }

    private void ChestOnResume()
    {
        if (IsInPuzzle)
        {
            GameManager.Instance.BlockPlayer(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OpenChest()
    {
        if (_puzzle)
        {
            ClosePuzzle();
            Destroy(_puzzle);
        }
        else
            GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);
        _spriteRenderer.sprite = _openedChestSprite;
        IsInteractive = false;
        GiveReward();
        foreach (var col in GetComponents<BoxCollider2D>())
        {
            col.enabled = false;
        }
        Destroy(this);
    }

    private void GiveReward()
    {
        if (Random.Range(0, 11) > 6)
            Instantiate(_bigBobPrefab, transform.position, Quaternion.identity, RoomsManager.Instance.CurrentRoom.transform);
        Instantiate(_batteryPrefab, transform.position, Quaternion.identity, RoomsManager.Instance.CurrentRoom.transform);
    }
}