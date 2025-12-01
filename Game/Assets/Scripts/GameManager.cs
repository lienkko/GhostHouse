using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameUIFieldsGetter GameUIFields { get; private set; }


    [SerializeField] private AudioClip _blinkLightsSound;
    public AudioSource GMAudioSource { get; private set; }

    private bool _isConsoleOpened = false;
    private bool _inGame = false;


    

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        GMAudioSource = GetComponent<AudioSource>();
        SceneManager.activeSceneChanged += OnSceneChanged;
        InitializeApp();
        Settings.OnSettingsChanged += UpdateSettings;
    }

    private void Start()
    {
        SceneManager.LoadScene("main menu");
    }
    private void InitializeApp()
    {
        PlayerPrefs.SetInt("DisplayMode", 0);
        if (!PlayerPrefs.HasKey("Launched"))
        {
            PlayerPrefs.SetInt("Resolution", 0);
            PlayerPrefs.SetInt("Hints", 1);
            PlayerPrefs.SetFloat("Volume", 1);
            PlayerPrefs.SetInt("Launched", 1);
            PlayerPrefs.Save();
        }


    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.name == "Game") 
        {
            _inGame = true;
            InitializeGame();
        }
        else 
        {
            _inGame = false;
        }
    }

    private void UpdateSettings()
    {
        Screen.fullScreen = PlayerPrefs.GetInt("DisplayMode")==1;
        if (_inGame)
            PlayerInteract.Instance.Hints = PlayerPrefs.GetInt("Hints") == 1;
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
    }
    

    private void InitializeGame()
    {
        GameUIFields = FindAnyObjectByType<GameUIFieldsGetter>();
        Ghost.Instance.InteractiveInstance.SetListener(StartGame);
        PlayerController.Instance.OnDeath += Death;
        PlayerController.Instance.OnDamage += ChangeHp;
        Cursor.lockState = CursorLockMode.Locked;


        Screen.fullScreen = PlayerPrefs.GetInt("DisplayMode") == 1;
        PlayerInteract.Instance.Hints = PlayerPrefs.GetInt("Hints") == 1;
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
    }



    private void Update()
    {
        if (_inGame)
        {
            if ((Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Slash)) && !_isConsoleOpened)
            {
                Cursor.lockState = CursorLockMode.None;
                OpenConsole();
            }
            else if ((Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Escape)) && _isConsoleOpened)
            {
                Cursor.lockState = CursorLockMode.Locked;
                CloseConsole();
            }
        }
    }



    //InGame methods
    private void OpenConsole()
    {
        PlayerController.Instance.CanWalk = false;
        PlayerController.Instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        PlayerInteract.Instance.CanInteract = false;

        _isConsoleOpened = true;

        if (Input.GetKeyDown(KeyCode.Slash))
            GameUIFields.ConsoleWindow.transform.GetComponentInChildren<CommandLine>().isSlash = true;

        GameUIFields.ConsoleWindow.SetActive(true);
    }

    private void CloseConsole()
    {
        PlayerController.Instance.CanWalk = true;
        _isConsoleOpened = false;
        PlayerInteract.Instance.CanInteract = true;
        GameUIFields.ConsoleWindow.SetActive(false);
    }
    public void StartGame()
    {
        if (Ghost.Instance.InteractiveInstance.isInteractive == false)
            return;
        Ghost.Instance.InteractiveInstance.isInteractive = false;
        RoomsManager.Instance.CurrentRoom.
            GetComponent<RoomData>().NextRoomDoor.
            GetComponent<Interactive>().isInteractive = true;
        StartCoroutine(BlinkLights(true));
    }

    private void Death()
    {
        GameUIFields.DeathText.SetActive(true);
    }

    private void ChangeHp(int dmg, int hp)
    {
        GameUIFields.HpField.text = hp.ToString();
    }

    private IEnumerator BlinkLights(bool turnOff = false)
    {
        int countOfBlinks = 0;
        Transform lightsTransform = RoomsManager.Instance.CurrentRoom.transform.Find("Lights");
        int numOfLights = lightsTransform.childCount;

        GMAudioSource.PlayOneShot(_blinkLightsSound);

        while (countOfBlinks < 18)
        {
            for(int i = 0; i < numOfLights; i++)
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
        Transform lightsTransform = RoomsManager.Instance.CurrentRoom.transform.Find("Lights");
        int numOfChilds = lightsTransform.childCount;
        for (int i = 0; i < numOfChilds; i++)
        {
            if (state)
                lightsTransform.GetChild(i).GetComponent<Light2D>().intensity = 0;
            else
                lightsTransform.GetChild(i).GetComponent<Light2D>().intensity = 0.9f;
        }
    }

    public void SummonWraith()
    {
        StartCoroutine(BlinkLights());
        var wraithPoints = RoomsManager.Instance.CurrentRoom.transform.Find("WraithPoints");

        WraithHandler.Instance.StartWraith(wraithPoints.Find("StartPoint").position, wraithPoints.Find("EndPoint").position);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene("LoadMainScene");
    }
}
