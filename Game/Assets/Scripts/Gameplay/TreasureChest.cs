using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Interactive))]
public class TreasureChest : MonoBehaviour
{
    private readonly string _puzzleName = "Prefabs/Puzzles/PuzzleNumbers";
    public static bool IsInPuzzle { get; private set; } = false;

    private GameObject _puzzle;
    private Interactive _interactiveComp;
    private SpriteRenderer _spriteRenderer;

    [Header("Chest Settings")]
    [SerializeField] private Sprite _openedChestSprite;
    private int _rewardValue;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _interactiveComp = GetComponent<Interactive>();
        _interactiveComp.SetListener(OpenPuzzle);
        _interactiveComp.isInteractive = true;

        Pause.OnResume += ChestOnResume;
    }

    private void Start()
    {
        PlayerController.Instance.OnDeath += ClosePuzzle;
        _rewardValue = Random.Range(20,41);
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
    }

    public void OpenPuzzle()
    {
        Cursor.lockState = CursorLockMode.None;
        
        StartCoroutine(SwitchIsInPuzzle(true));
        
        GameManager.Instance.BlockPlayer(true);

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
    }

    private IEnumerator SwitchIsInPuzzle(bool state)
    {
        yield return null;
        _interactiveComp.isInteractive = !state;
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
        _interactiveComp.RemoveListener();
        _interactiveComp.isInteractive = false;
        GiveReward();
        Destroy(this);
    }

    private void GiveReward()
    {
        return;
    }
}