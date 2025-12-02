using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Safe : MonoBehaviour
{
    private readonly string[] _puzzleNames = new string[] { "Circles", "Star" };


    public bool IsInPuzzle { get; private set; } = false;

    private GameObject _puzzle;
    private DoorController _doorToOpen;

    [SerializeField] private Sprite _rightLeftSafeSprite;
    [SerializeField] private Sprite _topSafeSprite;
    [SerializeField] private Sprite _botSafeSprite;


    private void Awake()
    {
        GetComponent<Interactive>().SetListener(OpenPuzzle);
        GetComponent<Interactive>().isInteractive = true;
        PlayerController.Instance.OnDeath += ClosePuzzle;
        Pause.OnResume += ShowCursorOnResume;
    }

    private void Update()
    {
        if (IsInPuzzle && Input.GetKeyDown(KeyCode.E))
        {
            ClosePuzzle();
            return;
        }
    }

    public void Initialize(string pointTag, DoorController door)
    {
        _doorToOpen = door;
        switch (tag)
        {
            case "TopPoint":
                GetComponent<SpriteRenderer>().sprite = _topSafeSprite;
                break;
            case "BotPoint":
                GetComponent<SpriteRenderer>().sprite = _botSafeSprite;
                break;
            case "RightPoint":
                GetComponent<SpriteRenderer>().sprite = _rightLeftSafeSprite;
                break;
            case "LeftPoint":
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                    GetComponent<SpriteRenderer>().sprite = _rightLeftSafeSprite;
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
        GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);

        StartCoroutine(SwitchIsInPuzzle(true));

        GameManager.Instance.BlockPlayer(true);

        if (_puzzle)
        {
            _puzzle.SetActive(true);
            return;
        }
        CreatePuzzle();
    }

    public void ClosePuzzle()
    {
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
        IsInPuzzle = state;
    }

    private void ShowCursorOnResume(){Cursor.lockState = CursorLockMode.None;}

    public void OpenSafe()
    {
        GetComponent<Interactive>().isInteractive = false;
        GetComponent<Interactive>().RemoveListener();
        if (_puzzle)
        {
            ClosePuzzle();
            Destroy(_puzzle);
        }
        else
            GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);
        _doorToOpen.UnlockDoor();
        Destroy(this);
    }

}
