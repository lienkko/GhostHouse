using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Console : MonoBehaviour
{
    public static Console Instance;

    public bool IsConsoleOpened { get; private set; }


    [SerializeField] private GameObject _consoleWindow;


    private void Awake()
    {
        IsConsoleOpened = false;
        Instance = this;
    }

    private bool CanOpenConsole() { return (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Slash)) && !IsConsoleOpened && !Pause.IsPaused; }
    private bool CanCloseConsole() { return (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Escape)) && IsConsoleOpened; }
    private void Update()
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

    private void OpenConsole()
    {
        GameManager.Instance.BlockPlayer(true);
        StartCoroutine(SwitchIsConsoleOpened(true));

        if (Input.GetKeyDown(KeyCode.Slash))
            _consoleWindow.transform.GetComponentInChildren<CommandLine>().isSlash = true;

        _consoleWindow.SetActive(true);
    }

    private void CloseConsole()
    {
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
