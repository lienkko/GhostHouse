using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Safe : MonoBehaviour
{
    private GameManager _gm;

    private string[] _puzzleNames = new string[] { "Circles", "Star" };
    public bool isInPuzzle = false;
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
        GetComponent<Interactive>().SetListener(OpenPuzzle);
        GetComponent<Interactive>().isInteractive = true;
        PlayerController.OnDeath += ClosePuzzle;
    }

    private void Update()
    {
        if (isInPuzzle && Input.GetKeyDown(KeyCode.Escape))
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            _playerController = null;
        }
    }

    private void ShowOpenText(bool state)
    {
        _gm.OpenSafeText.gameObject.SetActive(state);
    }

    private void OpenPuzzle(GameObject player)
    {
        Cursor.lockState = CursorLockMode.None;
        _playerController = player.GetComponent<PlayerController>();
        ShowOpenText(false);
        isInPuzzle = true;
        _playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        _playerController.CanWalk = false;
        _playerController.GetComponent<Interact>().CanInteract = false;
        if (_puzzle)
        {
            _puzzle.SetActive(true);
            return;
        }
        string puzzleName = $"Prefabs/Puzzles/Puzzle{_puzzleNames[Random.Range(0, 2)]}";
        _puzzle = Instantiate<GameObject>(Resources.Load<GameObject>(puzzleName));
        _puzzle.transform.SetParent(gameObject.transform);
        _puzzleButton = _puzzle.transform.Find("Canvas/CompleteButton").GetComponent<Button>();
        _puzzleButton.onClick.AddListener(OpenSafe);
    }

    public void ClosePuzzle()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ShowOpenText(true);
        isInPuzzle = false;
        _puzzle.SetActive(false);
        if (_playerController)
        {
            _playerController.CanWalk = true;
            _playerController.GetComponent<Interact>().CanInteract = true;
        }
    }

    public void OpenSafe()
    {
        GetComponent<Interactive>().isInteractive = false;
        GetComponent<Interactive>().RemoveListener();
        if (_puzzle)
        {
            ClosePuzzle();
            _puzzleButton.onClick.RemoveAllListeners();
            Destroy(_puzzle);
        }
        ShowOpenText(false);
        _doorToOpen.UnlockDoor();
        Destroy(this);
    }

}
