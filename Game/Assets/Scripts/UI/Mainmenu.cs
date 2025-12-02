using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Mainmenu : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private GameObject _mainMenuWin;
    [SerializeField] private GameObject _settingsWin;

    private void Awake()
    {
        _startButton.onClick.AddListener(StartGame);
        _quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _settingsWin.SetActive(false);
            _mainMenuWin.SetActive(true);
        }
    }
    private void StartGame()
    {
        SceneManager.LoadScene("LoadMainScene");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}

