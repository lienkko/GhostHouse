using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private GameObject _settingsWindow;
    [SerializeField] private GameObject _pauseWindow;

    public delegate void GamePause();
    public static event GamePause OnPause;
    public static event GamePause OnResume;

    private bool _inSettings = false;
    public static bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        _resumeButton.onClick.AddListener(ResumeGame);
        _settingsButton.onClick.AddListener(Settings);
        _menuButton.onClick.AddListener(ToMenu);
    }

    void Update()
    {
        if (_inSettings && Input.GetKeyDown(KeyCode.Escape))
        {
            Settings();
            return;
        }
        if (!Console.Instance.IsConsoleOpened && Input.GetKeyDown(KeyCode.Escape) && !Safe.IsInPuzzle)
        {
            if (!IsPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        _pauseWindow.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        GameManager.Instance.BlockPlayer(true);
        OnPause?.Invoke();
    }
    private void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        _pauseWindow.SetActive(false);
        if (!PlayerController.Instance.IsDead)
            Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.BlockPlayer(false);
        OnResume?.Invoke();
    }

    private void Settings()
    {
        _inSettings = !_inSettings;
        _settingsWindow.SetActive(_inSettings);
        _pauseWindow.SetActive(!_inSettings);
    }

    private void ToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("main menu");
    }
}
