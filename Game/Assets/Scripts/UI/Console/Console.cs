using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public static Console Instance;

    public bool IsConsoleOpened { get; private set; }

    [SerializeField] private GameObject _consoleWindow;

    [Space(10)]
    [SerializeField] private Button _closeButton;

    private void Awake()
    {
        Instance = this;
        IsConsoleOpened = false;
        _closeButton.onClick.AddListener(CloseConsole);
    }

    private bool CanOpenConsole() { return Input.GetKeyDown(KeyCode.BackQuote) && !IsConsoleOpened && !Pause.IsPaused; }
    private bool CanCloseConsole() { return (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Escape)) && IsConsoleOpened; }
    private void Update()
    {
        if (CanOpenConsole())
        {
            OpenConsole();
        }
        else if (CanCloseConsole())
        {
            CloseConsole();
        }
    }

    

    private void OpenConsole()
    {
        Cursor.lockState = CursorLockMode.None;
        GameManager.Instance.BlockPlayer(true);
        StartCoroutine(SwitchIsConsoleOpened(true));
        _consoleWindow.SetActive(true);
    }

    private void CloseConsole()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.BlockPlayer(false);
        StartCoroutine(SwitchIsConsoleOpened(false));
        _consoleWindow.SetActive(false);
    }
    private IEnumerator SwitchIsConsoleOpened(bool state)
    {
        yield return null;
        IsConsoleOpened = state;
    }




}
