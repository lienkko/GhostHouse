using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private Text _healthPointsField;
    [SerializeField] private Ghost _ghost;
    [SerializeField] private AudioClip _blinkLightsSound;
    
    [SerializeField] private Slider loadingBar;

    [SerializeField] private GameObject _console;
    private bool _isConsoleOpened = false;

    private AudioSource _audioSource;
    private GameObject _currentRoom;
    private DoorController _startedDoor;
    public PlayerController playerController;
    private Interact _interactScript;
    public GameObject CurrentRoom { get=>_currentRoom; set=>_currentRoom = value; }

    public TextMeshProUGUI OpenDoorText;
    public TextMeshProUGUI OpenSafeText;
    public TextMeshProUGUI HideText;
    public TextMeshProUGUI StartGameText;
    public WraithHandler _wraith;

    public Image LockedText;

    public DoorController CurrentNextRoomDoor;
    public DoorController CurrentPreviousRoomDoor;

    private void Awake()
    {
        PlayerController.OnDeath += Death;
        PlayerController.OnDamage += ChangeHp;
        _ghost.OnStartGame += StartGame;
        _startedDoor = FindAnyObjectByType<DoorController>();
        _currentRoom = FindAnyObjectByType<RoomData>().gameObject;

        _audioSource = GetComponent<AudioSource>();
        playerController = FindAnyObjectByType<PlayerController>();
        _interactScript = FindAnyObjectByType<Interact>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {

        if ((Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Slash)) && !_isConsoleOpened)
        {
            playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerController.CanWalk = false;
            _interactScript.CanInteract = false;
            _isConsoleOpened = true;
            if (Input.GetKeyDown(KeyCode.Slash))
                _console.transform.GetComponentInChildren<CommandLine>().isSlash = true;
            _console.gameObject.SetActive(true);
        }
        else if((Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Escape)) && _isConsoleOpened)
        {
            playerController.CanWalk = true;
            _isConsoleOpened = false;
            _interactScript.CanInteract = true;
            _console.gameObject.SetActive(false);
        }
    }


    private void StartGame()
    {
        _startedDoor.GetComponent<Interactive>().isInteractive = true;
        StartCoroutine(BlinkLights(true));
    }

    private void Death()
    {
        _gameOverText.SetActive(true);
    }

    private void ChangeHp(int dmg, int hp)
    {
        _healthPointsField.text = hp.ToString();
    }

    private IEnumerator BlinkLights(bool turnOff = false)
    {
        int countOfBlinks = 0;
        Transform lightsTransform = _currentRoom.transform.Find("Lights");
        int numOfChilds = lightsTransform.childCount;
        _audioSource.PlayOneShot(_blinkLightsSound);
        _audioSource.loop = true;
        while (countOfBlinks < 18)
        {
            for(int i = 0; i < numOfChilds; i++)
            {
                lightsTransform.GetChild(i).GetComponent<Light2D>().intensity = Random.Range(0.3f,1.1f);
            }
            countOfBlinks++;
            yield return new WaitForSeconds(Time.deltaTime*10);
        }
        TurnOffLights(turnOff);
    }
    
    public void TurnOffLights(bool state)
    {
        Transform lightsTransform = _currentRoom.transform.Find("Lights");
        int numOfChilds = lightsTransform.childCount;
        for (int i = 0; i < numOfChilds; i++)
        {
            if (state)
                lightsTransform.GetChild(i).GetComponent<Light2D>().intensity = 0;
            else
                lightsTransform.GetChild(i).GetComponent<Light2D>().intensity = 0.9f;
        }
    }

    public void SummonWraith(GameObject currentRoom)
    {
        _currentRoom = currentRoom;
        StartCoroutine(BlinkLights());
        var wraithPoints = _currentRoom.transform.Find("WraithPoints");
        var doors = FindObjectsOfType<DoorController>();
        _wraith.StartWraith(wraithPoints.Find("StartPoint").position, wraithPoints.Find("EndPoint").position, doors);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene("ReloadMainScene");
    }
}
