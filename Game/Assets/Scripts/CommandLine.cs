using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class CommandLine : MonoBehaviour
{
    private TMP_InputField _inputField;
    private GameManager _gm;

    private List<string> _lastCommands;
    private int _choosenLastCommandIndex;
    private bool _choosingLastCommand = true;
    private string _currentLine = "";

    [SerializeField] private TMP_Text _commandsField;

    public bool isSlash = false;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
        _inputField.onValueChanged.AddListener(WorkWithLine);
        _lastCommands = new List<string>();
        _gm = FindAnyObjectByType<GameManager>();
        _inputField.onSubmit.AddListener(EnterCommand);
    }

    private void WorkWithLine(string line)
    {
        StartCoroutine(WorkWithLineCor(line));
    }

    private IEnumerator WorkWithLineCor(string line)
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
        StartCoroutine(ActivateInputField());
        if (isSlash)
        {
            _inputField.text = "/";
            isSlash = false;
        }
        else
            _inputField.text = "";
        _inputField.caretPosition = _inputField.text.Length;
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

    IEnumerator ActivateInputField(){
        yield return null;
        _inputField.ActivateInputField();
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
        _gm.playerController.gameObject.SetActive(false);
        _gm.playerController.gameObject.SetActive(true);
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
                PrintOnConsole("Доступные команды: \n" +
                    "/startgame - начинает игру\n" +
                    "/godmode (1/0) - включает/выключает режим бога\n" +
                    "/summon_wraith - призывает wraith (недоступна в стартовой комнате)\n" +
                    "/open_safe - открывает закрытый сейф в комнате\n" +
                    "/restartgame - перезапускает игру\n" +
                    "/nextroom - перемещает в следующую комнату\n" +
                    "/prevroom - перемещает в предыдущую комнату");
                _lastCommands.Add(commandAndParameters[0]);
                break;
            case "startgame":
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
                    var ghost = FindAnyObjectByType<Ghost>();
                    if (!ghost.GetComponent<Interactive>().isInteractive)
                    {
                        PrintOnConsole("Игра уже началась");
                        return;
                    }
                    ghost.StartTheGame(_gm.playerController.gameObject);
                    PrintOnConsole("Игра началась");
                    ReloadPlayer();
                    _lastCommands.Add(commandAndParameters[0]);
                    break;
                }
            case "godmode":
                if (commandAndParameters.Length != 2)
                {
                    PrintOnConsole("Некорректные параметры для godmode");
                    return;
                }
                if (commandAndParameters[1] == "1")
                {
                    _gm.playerController.IsGodMode = true;
                    PrintOnConsole("Режим бога включен");
                }
                else if (commandAndParameters[1] == "0")
                {
                    _gm.playerController.IsGodMode = false;
                    PrintOnConsole("Режим бога выключен");
                }
                else
                    PrintOnConsole("Некорректный параметр для godmode");
                _lastCommands.Add(commandAndParameters[0]);
                break;
            case "summon_wraith":
                if (commandAndParameters.Length > 1)
                {
                    PrintOnConsole("Некорректные параметры для summon_wraith");
                    return;
                }
                if (FindAnyObjectByType<RoomData>().name == "StartRoom")
                {
                    PrintOnConsole("В стартовой комнате нельзя вызвать wraith");
                    return;
                }

                _gm.SummonWraith(FindAnyObjectByType<RoomData>().gameObject);
                PrintOnConsole("Призрак вызван");
                _lastCommands.Add(commandAndParameters[0]);
                break;
            case "open_safe":
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
                    break;
                }
            case "restartgame":
                if (commandAndParameters.Length > 1)
                {
                    PrintOnConsole("Некорректные параметры для restartgame");
                    return;
                }
                _gm.ReloadGame();
                break;
            case "nextroom":
                {
                    if (commandAndParameters.Length > 1)
                    {
                        PrintOnConsole("Некорректные параметры для nextroom");
                        return;
                    }
                    if (_gm._wraith.isMoving)
                    {
                        PrintOnConsole("Во время полета wraith нельзя использовать nextroom");
                        return;
                    }
                    if (FindAnyObjectByType<RoomData>().name == "StartRoom")
                    {
                        PrintOnConsole("nextroom не может вызываться в стартовой комнате");
                        return;
                    }
                    if (!_gm.playerController.gameObject.activeSelf)
                    {
                        PrintOnConsole("nextroom не может вызываться когда игрок спрятан");
                        return;
                    }
                    if (safe)
                        safe.OpenSafe();
                    _gm.CurrentNextRoomDoor.ActivateDoor(_gm.playerController.gameObject);
                    _lastCommands.Add(commandAndParameters[0]);
                    break;
                }
            case "prevroom":
                if (commandAndParameters.Length > 1)
                {
                    PrintOnConsole("Некорректные параметры для prevroom");
                    return;
                }
                if (_gm._wraith.isMoving)
                {
                    PrintOnConsole("Во время полета wraith нельзя использовать prevroom");
                    return;
                }
                if (FindAnyObjectByType<RoomData>().name == "StartRoom")
                {
                    PrintOnConsole("prevroom не может вызываться в стартовой комнате");
                    return;
                }
                if (!_gm.playerController.gameObject.activeSelf)
                {
                    PrintOnConsole("prevroom не может вызываться когда игрок спрятан");
                    return;
                }
                if (safe && safe.isInPuzzle)
                    safe.ClosePuzzle();
                _gm.CurrentPreviousRoomDoor.ActivateDoor(_gm.playerController.gameObject);
                _lastCommands.Add(commandAndParameters[0]);
                break;
            default:
                PrintOnConsole($"\"/{line}\" не является командой");
                break;
        }
        _choosenLastCommandIndex = _lastCommands.Count - 1;
    }

    private string[] GetCommandAndParameter(string line)
    {
        var matches = Regex.Matches(line, @"\S+"); // каждое слово — последовательность непробельных символов
        var result = new List<string>(matches.Count);

        foreach (Match match in matches)
            result.Add(match.Value);

        return result.ToArray();
    }

    private void PrintOnConsole(string line)
    {
        _commandsField.text += line + "\n";
    }
}
