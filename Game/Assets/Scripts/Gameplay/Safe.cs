using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Safe : MonoBehaviour
{
    private GameManager _gm;

    private string[] _puzzleNames = new string[] { "Circles", "Star" };
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
        GetComponent<Interactive>().SetListener(OpenPuzzle);
        GetComponent<Interactive>().isInteractive = true;
    }

    private void Update()
    {
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
        _playerController = player.GetComponent<PlayerController>();
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
        _playerController.GetComponent<Interact>().CanInteract = false;
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
        _playerController.GetComponent<Interact>().CanInteract = true;
    }

    public void OpenSafe()
    {
        GetComponent<Interactive>().isInteractive = false;
        GetComponent<Interactive>().RemoveListener();
        ClosePuzzle();
        ShowOpenText(false);
        _puzzleButton.onClick.RemoveAllListeners();
        _doorToOpen.UnlockDoor();
        Destroy(_puzzle);
        Destroy(this);
    }

}
