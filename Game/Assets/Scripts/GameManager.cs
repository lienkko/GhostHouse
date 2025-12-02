using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameUIFieldsGetter GameUIFields { get; private set; }

    

    [SerializeField] private AudioClip _blinkLightsSound;
    public AudioSource GMAudioSource { get; private set; }

    private Pause _pauseMenu;

    public bool IsConsoleOpened { get; private set; } = false;
    private bool _inGame = false;
    



    private void Awake()
    {
        if (Instance!= null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        GMAudioSource = GetComponent<AudioSource>();

        SceneManager.activeSceneChanged += OnSceneChanged;
        Settings.OnSettingsChanged += UpdateSettings;
        Pause.OnPause += PauseGame;
        Pause.OnResume += ResumeGame;

        InitializeApp();
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
        Screen.fullScreen = PlayerPrefs.GetInt("DisplayMode") == 0;
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");

    }

    private void InitializeGame()
    {
        GameUIFields = FindAnyObjectByType<GameUIFieldsGetter>();
        _pauseMenu = FindAnyObjectByType<Pause>();

        Ghost.Instance.InteractiveInstance.SetListener(StartGame);
        PlayerController.Instance.OnDeath += Death;
        PlayerController.Instance.OnDamage += ChangeHp;
        Cursor.lockState = CursorLockMode.Locked;

        PlayerInteract.Instance.Hints = PlayerPrefs.GetInt("Hints") == 1;
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
        Screen.fullScreen = PlayerPrefs.GetInt("DisplayMode")==0;
        if (_inGame)
            PlayerInteract.Instance.Hints = PlayerPrefs.GetInt("Hints") == 1;
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
    }
    
    
    private void PauseGame()
    {
        GMAudioSource.Pause();
    }
    private void ResumeGame()
    {
        GMAudioSource.UnPause();
    }


    private void Update()
    {
        if (_inGame)
        {
            if ((Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Slash)) && !IsConsoleOpened)
            {
                Cursor.lockState = CursorLockMode.None;
                OpenConsole();
            }
            else if ((Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Escape)) && IsConsoleOpened)
            {
                Cursor.lockState = CursorLockMode.Locked;
                CloseConsole();
            }
            
        }
    }

    //InGame methods
    public void BlockPlayer(bool state)
    {
        if (state)
            PlayerController.Instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        PlayerController.Instance.CanWalk = !state;
        PlayerInteract.Instance.CanInteract = !state;
    }

    private void OpenConsole()
    {
        BlockPlayer(true);
        StartCoroutine(SwitchIsConsoleOpened(true));

        if (Input.GetKeyDown(KeyCode.Slash))
            GameUIFields.ConsoleWindow.transform.GetComponentInChildren<CommandLine>().isSlash = true;

        GameUIFields.ConsoleWindow.SetActive(true);
    }

    private void CloseConsole()
    {
        BlockPlayer(false);
        StartCoroutine(SwitchIsConsoleOpened(false));
        GameUIFields.ConsoleWindow.SetActive(false);
    }
    private IEnumerator SwitchIsConsoleOpened(bool state)
    {
        yield return null;
        IsConsoleOpened = state;
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
        Ghost.Instance.gameObject.SetActive(false);
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
