using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;


public class CommandLine : MonoBehaviour
{
    private const string HELP_TEXT = "Доступные команды: \n" +
                    "/startgame - начинает игру\n" +
                    "/godmode (1/0) - включает/выключает режим бога\n" +
                    "/summon_wraith - призывает wraith (недоступна в стартовой комнате)\n" +
                    "/open_safe - открывает закрытый сейф в комнате\n" +
                    "/restartgame - перезапускает игру\n" +
                    "/nextroom - перемещает в следующую комнату\n" +
                    "/prevroom - перемещает в предыдущую комнату\n" +
                    "/room_lights (1/0) - включает/выключает свет в комнате";

    [SerializeField] private TMP_Text _commandsField;
    private TMP_InputField _inputField;

    private List<string> _lastCommands;
    private int _choosenLastCommandIndex;
    private bool _choosingLastCommand = true;
    private string _currentLine = "";

    

    [HideInInspector] public bool isSlash = false;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();

        _inputField.onValueChanged.AddListener(OnLineChanged);
        _inputField.onSubmit.AddListener(EnterCommand);

        _lastCommands = new List<string>();
        
    }

    private void OnLineChanged(string line)
    {
        StartCoroutine(WorkWithLine(line));
    }

    private IEnumerator WorkWithLine(string line)
    {
        yield return null;
        _currentLine = line;
        if (_lastCommands.Count > 0 && line != "/" + _lastCommands[_choosenLastCommandIndex])
        {
            _choosingLastCommand = false;
            _choosenLastCommandIndex = _lastCommands.Count - 1;
        }
        else if (_lastCommands.Count > 0 && line == "/" + _lastCommands[_choosenLastCommandIndex])
        {
            _choosenLastCommandIndex -= 1;
            if (_choosenLastCommandIndex < 0)
                _choosenLastCommandIndex = _lastCommands.Count - 1;
        }
    }


    private void OnEnable()
    {
        _choosingLastCommand = true;
        _choosenLastCommandIndex = _lastCommands.Count - 1;
        
        StartCoroutine(ActivateCommandLine());

        if (isSlash)
        {
            _inputField.text = "/";
            isSlash = false;
        }
        else
            _inputField.text = "";
        _inputField.caretPosition = _inputField.text.Length;
    }

    IEnumerator ActivateCommandLine()
    {
        yield return null;
        _inputField.ActivateInputField();
    }


    private void Update()
    {
        if (string.IsNullOrWhiteSpace(_currentLine))
        {
            _choosingLastCommand = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab) && _choosingLastCommand && _lastCommands.Count > 0)
        {
            
            _inputField.text = "/" + _lastCommands[_choosenLastCommandIndex];
            _inputField.caretPosition = _inputField.text.Length;
        }
    }

    

    private void EnterCommand(string line)
    {
        _inputField.text = "";
        _inputField.ActivateInputField();

        line = line.Trim();
        if (string.IsNullOrWhiteSpace(line))
            return;
        if (!line.StartsWith("/"))
        {
            PrintOnConsole($"\"{line}\" не является командой");
            return;
        }

        Execute(line[1..]);
    }

    private void ReloadPlayer()
    {
        PlayerController.Instance.gameObject.SetActive(false);
        PlayerController.Instance.gameObject.SetActive(true);
    }


    private void Execute(string line)
    {
        var commandAndParameters = GetCommandAndParameter(line);
        Safe safe = FindAnyObjectByType<Safe>();
        switch (commandAndParameters[0])
        {
            case "help":
                if (commandAndParameters.Length > 1)
                {
                    PrintOnConsole("help пока выводит только доступные команды");
                    return;
                }
                PrintOnConsole(HELP_TEXT);
                _lastCommands.Add(commandAndParameters[0]);
                break;
            case "startgame":
                ExecuteStartGame(commandAndParameters);
                break;
            case "godmode":
                ExecuteGodMode(commandAndParameters);
                break;
            case "summon_wraith":
                ExecuteSummonWraith(commandAndParameters);
                break;
            case "open_safe":
                ExecuteOpenSafe(safe, commandAndParameters);
                break;
            case "restartgame":
                ExecuteRestartGame(commandAndParameters);
                break;
            case "nextroom":
                ExecuteNextRoom(safe, commandAndParameters);
                break;
            case "prevroom":
                ExecutePrevRoom(safe, commandAndParameters);
                break;
            case "room_lights":
                ExecuteRoomLights(commandAndParameters);
                break;
            default:
                PrintOnConsole($"\"/{line}\" не является командой");
                break;
        }
        _choosenLastCommandIndex = _lastCommands.Count - 1;
    }

    

    private string[] GetCommandAndParameter(string line)
    {
        var matches = Regex.Matches(line, @"\S+");
        var result = new List<string>(matches.Count);

        foreach (Match match in matches)
            result.Add(match.Value);

        return result.ToArray();
    }

    private void PrintOnConsole(string line)
    {
        _commandsField.text += line + "\n";
    }

    private void ExecuteStartGame(string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            PrintOnConsole("startgame не требует параметров");
            return;
        }
        if (FindAnyObjectByType<RoomData>().name != "StartRoom")
        {
            PrintOnConsole("startgame может вызываться только в стартовой комнате");
            return;
        }
        if (!Ghost.Instance.InteractiveInstance.isInteractive)
        {
            PrintOnConsole("Игра уже началась");
            return;
        }
        GameManager.Instance.StartGame();
        PrintOnConsole("Игра началась");
        ReloadPlayer();
        _lastCommands.Add(commandAndParameters[0]);
    }

    private void ExecuteGodMode(string[] commandAndParameters)
    {
        if (commandAndParameters.Length != 2)
        {
            PrintOnConsole("Некорректные параметры для godmode");
            return;
        }
        if (commandAndParameters[1] == "1")
        {
            PlayerController.Instance.IsGodMode = true;
            PrintOnConsole("Режим бога включен");
        }
        else if (commandAndParameters[1] == "0")
        {
            PlayerController.Instance.IsGodMode = false;
            PrintOnConsole("Режим бога выключен");
        }
        else
            PrintOnConsole("Некорректный параметр для godmode");
        _lastCommands.Add(commandAndParameters[0]);
    }

    private void ExecuteSummonWraith(string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            PrintOnConsole("Некорректные параметры для summon_wraith");
            return;
        }
        if (RoomsManager.Instance.CurrentRoom.name == "StartRoom")
        {
            PrintOnConsole("В стартовой комнате нельзя вызвать wraith");
            return;
        }

        GameManager.Instance.SummonWraith();
        PrintOnConsole("Призрак вызван");
        _lastCommands.Add(commandAndParameters[0]);
    }

    private void ExecuteOpenSafe(Safe safe, string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            PrintOnConsole("Некорректные параметры для open_safe");
            return;
        }
        if (!safe)
        {
            PrintOnConsole("В комнате не найден закрытый сейф");
            return;
        }
        safe.OpenSafe();
        PrintOnConsole("Сейф открыт");
        ReloadPlayer();
        _lastCommands.Add(commandAndParameters[0]);
    }

    private void ExecuteRestartGame(string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            PrintOnConsole("Некорректные параметры для restartgame");
            return;
        }
        GameManager.Instance.ReloadGame();
    }

    private void ExecuteNextRoom(Safe safe, string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            PrintOnConsole("Некорректные параметры для nextroom");
            return;
        }
        if (WraithHandler.Instance.IsWraithSummoned)
        {
            PrintOnConsole("Во время полета wraith нельзя использовать nextroom");
            return;
        }
        if (RoomsManager.Instance.CurrentRoom.name == "StartRoom")
        {
            PrintOnConsole("nextroom не может вызываться в стартовой комнате");
            return;
        }
        if (!PlayerController.Instance.gameObject.activeSelf)
        {
            PrintOnConsole("nextroom не может вызываться когда игрок спрятан");
            return;
        }
        if (safe)
            safe.OpenSafe();
        RoomsManager.Instance.CurrentRoom.GetComponent<RoomData>().NextRoomDoor.ActivateDoor();
        _lastCommands.Add(commandAndParameters[0]);
    }

    private void ExecutePrevRoom(Safe safe, string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            PrintOnConsole("Некорректные параметры для prevroom");
            return;
        }
        if (WraithHandler.Instance.IsWraithSummoned)
        {
            PrintOnConsole("Во время полета wraith нельзя использовать prevroom");
            return;
        }
        if (RoomsManager.Instance.CurrentRoom.name == "StartRoom")
        {
            PrintOnConsole("prevroom не может вызываться в стартовой комнате");
            return;
        }
        if (!PlayerController.Instance.gameObject.activeSelf)
        {
            PrintOnConsole("prevroom не может вызываться когда игрок спрятан");
            return;
        }
        if (safe && safe.IsInPuzzle)
            safe.ClosePuzzle();
        RoomsManager.Instance.CurrentRoom.GetComponent<RoomData>().PreviousRoomDoor.ActivateDoor();
        _lastCommands.Add(commandAndParameters[0]);
    }

    private void ExecuteRoomLights(string[] commandAndParameters)
    {
        if (commandAndParameters.Length != 2)
        {
            PrintOnConsole("Некорректные параметры для room_lights");
            return;
        }
        if (RoomsManager.Instance.CurrentRoom.name == "StartRoom")
        {
            PrintOnConsole("В стартовой комнате нельзя переключать свет");
            return;
        }
        if (commandAndParameters[1] == "1")
        {
            GameManager.Instance.TurnOffLights(false);
            PrintOnConsole("Свет в комнате включен");
        }
        else if (commandAndParameters[1] == "0")
        {
            GameManager.Instance.TurnOffLights(true);
            PrintOnConsole("Свет в комнате выключен");
        }
        else
            PrintOnConsole("Некорректный параметр для room_lights");
        _lastCommands.Add(commandAndParameters[0]);
    }
}
