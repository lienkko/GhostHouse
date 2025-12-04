using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameUIFieldsGetter GameUIFields { get; private set; }

    

    [SerializeField] private AudioClip _blinkLightsSound;
    public AudioSource GMAudioSource { get; private set; }


    [SerializeField]private bool _testMode;
    public bool IsConsoleOpened { get; private set; } = false;

    private bool _inGame = false;
    private readonly int[,] _resolutions = {{800, 600}, { 1280, 960}};
    private int _currentResolution;


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

        InitializePrefs();
    }

    private void Start()
    {
        if (_testMode)
        {
            GameUIFields = FindAnyObjectByType<GameUIFieldsGetter>();
            return;
        }
        SceneManager.LoadScene("main menu");
    }

    private void SetWindowSize()
    {
        int choosedResolution = PlayerPrefs.GetInt("Resolution");
        if (PlayerPrefs.GetInt("DisplayMode") == 0 && !Screen.fullScreen)
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.ExclusiveFullScreen);
        else if (PlayerPrefs.GetInt("DisplayMode") == 1 && (Screen.fullScreen || choosedResolution != _currentResolution))
        {
            int width = _resolutions[choosedResolution, 0];
            int height = _resolutions[choosedResolution, 1];
            Screen.SetResolution(width, height, false);
            _currentResolution = PlayerPrefs.GetInt("Resolution");
        }
    }

    private void InitializePrefs()
    {
        if (!PlayerPrefs.HasKey("Launched"))
        {
            PlayerPrefs.SetInt("DisplayMode", 0);
            PlayerPrefs.SetInt("Resolution", 0);
            PlayerPrefs.SetInt("Hints", 1);
            PlayerPrefs.SetFloat("Volume", 1);
            PlayerPrefs.SetInt("Launched", 1);
            PlayerPrefs.Save();
        }
        _currentResolution = PlayerPrefs.GetInt("Resolution");
        SetWindowSize();
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");

    }

    private void InitializeGame()
    {
        GameUIFields = FindAnyObjectByType<GameUIFieldsGetter>();

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
        SetWindowSize();
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        if (_inGame)
            PlayerInteract.Instance.Hints = PlayerPrefs.GetInt("Hints") == 1;
        
    }
    
    
    private void PauseGame()
    {
        GMAudioSource.Pause();
    }
    private void ResumeGame()
    {
        GMAudioSource.UnPause();
    }

    private bool CanOpenConsole() { return (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Slash)) && !IsConsoleOpened && !Pause.IsPaused; }
    private bool CanCloseConsole() { return (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Escape)) && IsConsoleOpened; }

    private void Update()
    {
        if (_inGame)
        {
            if (CanOpenConsole())
            {
                Cursor.lockState = CursorLockMode.None;
                OpenConsole();
            }
            else if (CanCloseConsole())
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
        GameUIFields.ButtonRestartGame.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
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

        while (countOfBlinks < 12)
        {
            for(int i = 0; i < numOfLights; i++)
            {
                lightsTransform.GetChild(i).GetComponent<Light2D>().intensity = Random.Range(0.3f,1.1f);
            }
            countOfBlinks++;
            yield return new WaitForSeconds(0.1f);
        }

        TurnOffLights(turnOff,lightsTransform);
    }
    
    public void TurnOffLights(bool state, Transform lightsTransform=null)
    {
        if (!lightsTransform)
            lightsTransform = RoomsManager.Instance.CurrentRoom.transform.Find("Lights");
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
        PlayerController.Instance.OnDeath -= Death;
        PlayerController.Instance.OnDamage -= ChangeHp;
        IsConsoleOpened = false;
        SceneManager.LoadScene("LoadMainScene");
    }
}
