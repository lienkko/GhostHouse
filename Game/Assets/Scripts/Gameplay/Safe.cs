using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Safe : MonoBehaviour
{
    private GameManager _gm;

    private string[] _puzzleNames = new string[] { "Circles", "Star" };
    private bool _mayOpen = false;
    private bool _isInPuzzle = false;
    private GameObject _puzzle;
    private Button _puzzleButton;
    private PlayerController _playerController;
    private DoorController _doorToOpen;

    public Sprite RightLeftSafeSprite;
    public Sprite TopSafeSprite;
    public Sprite BotSafeSprite;


    private void Awake()
    {
        _gm = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        if (_mayOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenPuzzle();
            return;
        }
        if (_isInPuzzle && Input.GetKeyDown(KeyCode.Escape))
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
                GetComponent<SpriteRenderer>().sprite = TopSafeSprite;
                break;
            case "BotPoint":
                GetComponent<SpriteRenderer>().sprite = BotSafeSprite;
                break;
            case "RightPoint":
                GetComponent<SpriteRenderer>().sprite = RightLeftSafeSprite;
                break;
            case "LeftPoint":
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                    GetComponent<SpriteRenderer>().sprite = RightLeftSafeSprite;
                    break;
                }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ShowOpenText(true);
        _playerController = collision.GetComponent<PlayerController>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ShowOpenText(false);
        _playerController = null;
        ClosePuzzle();
    }

    private void ShowOpenText(bool state)
    {
        _gm.OpenText.gameObject.SetActive(state);
        _mayOpen = state;
    }

    private void OpenPuzzle()
    {
        ShowOpenText(false);
        _isInPuzzle = true;
        if (_puzzle)
        {
            _puzzle.SetActive(true);
            return;
        }
        string puzzleName = $"Prefabs/Puzzles/Puzzle{_puzzleNames[Random.Range(0, 2)]}";
        _playerController.MoveSpeed = 0;
        _playerController.enabled = false;
        _puzzle = Instantiate<GameObject>(Resources.Load<GameObject>(puzzleName));
        _puzzle.transform.SetParent(gameObject.transform);
        _puzzleButton = _puzzle.transform.Find("Canvas/CompleteButton").GetComponent<Button>();
        _puzzleButton.onClick.AddListener(OpenSafe);
    }

    private void ClosePuzzle()
    {
        ShowOpenText(true);
        _isInPuzzle = false;
        _puzzle.SetActive(false);
        _playerController.enabled = true;
    }

    public void OpenSafe()
    {
        ClosePuzzle();
        _puzzleButton.onClick.RemoveAllListeners();
        _doorToOpen.UnlockDoor();
        _gm.OpenText.gameObject.SetActive(false);
        Destroy(_puzzle);
        Destroy(this);
    }

}
