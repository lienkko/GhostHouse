using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;


public class CommandLine : MonoBehaviour
{
    private const string HELP_TEXT = "��������� �������: \n" +
                    "startgame - �������� ����\n" +
                    "godmode (1/0) - ��������/��������� ����� ����\n" +
                    "summon_wraith - ��������� wraith (���������� � ��������� �������)\n" +
                    "open_safe - ��������� �������� ���� � �������\n" +
                    "restartgame - ������������� ����\n" +
                    "nextroom - ���������� � ��������� �������\n" +
                    "prevroom - ���������� � ���������� �������\n" +
                    "room_lights (1/0) - ��������/��������� ���� � �������\n" +
                    "speed (2 <= value <= 10) - ��������/��������� ���� � �������\n" +
                    "clear - ������� ���� ����";

    [SerializeField] private TMP_Text _commandsField;
    private TMP_InputField _inputField;

    private static List<string> _lastCommands = new List<string>();
    private int _choosenLastCommandIndex;
    private bool _choosingLastCommand = true;
    private string _currentLine = "";

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();

        _inputField.onValueChanged.AddListener(OnLineChanged);
        _inputField.onSubmit.AddListener(EnterCommand);
    }

    private void OnLineChanged(string line)
    {
        StartCoroutine(WorkWithLine(line));
    }

    private IEnumerator WorkWithLine(string line)
    {
        yield return null;
        _currentLine = line;
        if (_lastCommands.Count > 0 && line != _lastCommands[_choosenLastCommandIndex])
        {
            _choosingLastCommand = false;
            _choosenLastCommandIndex = _lastCommands.Count - 1;
        }
        else if (_lastCommands.Count > 0 && line == _lastCommands[_choosenLastCommandIndex])
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
        _inputField.text = "";
        _inputField.caretPosition = _inputField.text.Length;
    }



    private void Update()
    {
        _inputField.ActivateInputField();
        if (string.IsNullOrWhiteSpace(_currentLine))
        {
            _choosingLastCommand = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab) && _choosingLastCommand && _lastCommands.Count > 0)
        {

            _inputField.text = _lastCommands[_choosenLastCommandIndex];
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

        Execute(line);
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
                    Log("help ���� ������� ������ ��������� �������");
                    return;
                }
                Log(HELP_TEXT);
                AddToLastCommands(commandAndParameters[0]);
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
            case "speed":
                ExecuteSpeed(commandAndParameters);
                break;
            case "clear":
                ExecuteClear(commandAndParameters);
                break;
            default:
                Log($"\"{line}\" �� �������� ��������");
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

    private void Log(string line)
    {
        _commandsField.text += line + "\n";
    }

    private void AddToLastCommands(string command)
    {
        if (_lastCommands.Contains(command))
        {
            _lastCommands.Remove(command);
        }
        _lastCommands.Add(command);
    }

    private void ExecuteStartGame(string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            Log("startgame �� ������� ����������");
            return;
        }
        if (FindAnyObjectByType<RoomData>().name != "StartRoom")
        {
            Log("startgame ����� ���������� ������ � ��������� �������");
            return;
        }
        if (!Ghost.Instance.InteractiveInstance.isInteractive)
        {
            Log("���� ��� ��������");
            return;
        }
        GameManager.Instance.StartGame();
        Log("���� ��������");
        PlayerController.Instance.ReloadPlayer();
        AddToLastCommands(commandAndParameters[0]);
    }

    private void ExecuteGodMode(string[] commandAndParameters)
    {
        if (commandAndParameters.Length != 2)
        {
            Log("������������ ��������� ��� godmode");
            return;
        }
        if (commandAndParameters[1] == "1")
        {
            PlayerController.Instance.IsGodMode = true;
            Log("����� ���� �������");
        }
        else if (commandAndParameters[1] == "0")
        {
            PlayerController.Instance.IsGodMode = false;
            Log("����� ���� ��������");
        }
        else
            Log("������������ �������� ��� godmode");
        AddToLastCommands(commandAndParameters[0]);
    }

    private void ExecuteSummonWraith(string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            Log("������������ ��������� ��� summon_wraith");
            return;
        }
        if (RoomsManager.Instance.CurrentRoom.name == "StartRoom")
        {
            Log("� ��������� ������� ������ ������� wraith");
            return;
        }

        GameManager.Instance.SummonWraith();
        Log("������� ������");
        AddToLastCommands(commandAndParameters[0]);
    }

    private void ExecuteOpenSafe(Safe safe, string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            Log("������������ ��������� ��� open_safe");
            return;
        }
        if (!safe)
        {
            Log("� ������� �� ������ �������� ����");
            return;
        }
        safe.OpenSafe();
        Log("���� ������");
        if (PlayerController.Instance.gameObject.activeSelf)
            PlayerController.Instance.ReloadPlayer();
        AddToLastCommands(commandAndParameters[0]);
    }

    private void ExecuteRestartGame(string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            Log("������������ ��������� ��� restartgame");
            return;
        }
        GameManager.Instance.ReloadGame();
    }

    private void ExecuteNextRoom(Safe safe, string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            Log("������������ ��������� ��� nextroom");
            return;
        }
        if (WraithHandler.Instance.IsWraithSummoned)
        {
            Log("�� ����� ������ wraith ������ ������������ nextroom");
            return;
        }
        if (RoomsManager.Instance.CurrentRoom.name == "StartRoom")
        {
            Log("nextroom �� ����� ���������� � ��������� �������");
            return;
        }
        if (!PlayerController.Instance.gameObject.activeSelf)
        {
            Log("nextroom �� ����� ���������� ����� ����� �������");
            return;
        }
        if (safe)
            safe.OpenSafe();
        RoomsManager.Instance.CurrentRoom.GetComponent<RoomData>().NextRoomDoor.ActivateDoor();
        Log($"����� ��������� � ��������� �������");
        AddToLastCommands(commandAndParameters[0]);
    }

    private void ExecutePrevRoom(Safe safe, string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            Log("������������ ��������� ��� prevroom");
            return;
        }
        if (WraithHandler.Instance.IsWraithSummoned)
        {
            Log("�� ����� ������ wraith ������ ������������ prevroom");
            return;
        }
        if (RoomsManager.Instance.CurrentRoom.name == "StartRoom")
        {
            Log("prevroom �� ����� ���������� � ��������� �������");
            return;
        }
        if (!PlayerController.Instance.gameObject.activeSelf)
        {
            Log("prevroom �� ����� ���������� ����� ����� �������");
            return;
        }
        if (safe && Safe.IsInPuzzle)
            safe.ClosePuzzle();
        RoomsManager.Instance.CurrentRoom.GetComponent<RoomData>().PreviousRoomDoor.ActivateDoor();
        Log($"����� ��������� � ���������� �������");
        AddToLastCommands(commandAndParameters[0]);
    }

    private void ExecuteRoomLights(string[] commandAndParameters)
    {
        if (commandAndParameters.Length != 2)
        {
            Log("������������ ��������� ��� room_lights");
            return;
        }
        if (RoomsManager.Instance.CurrentRoom.name == "StartRoom")
        {
            Log("� ��������� ������� ������ ����������� ����");
            return;
        }
        if (commandAndParameters[1] == "1")
        {
            GameManager.Instance.TurnOffLights(false);
            Log("���� � ������� �������");
        }
        else if (commandAndParameters[1] == "0")
        {
            GameManager.Instance.TurnOffLights(true);
            Log("���� � ������� ��������");
        }
        else
            Log("������������ �������� ��� room_lights");
        AddToLastCommands(commandAndParameters[0]);
    }

    private void ExecuteSpeed(string[] commandAndParameters)
    {
        if (commandAndParameters.Length == 1)
        {
            Log($"��������: {PlayerController.Instance.CurrentSpeed}");
            return;
        }
        if (commandAndParameters.Length != 2)
        {
            Log("������������ ��������� ��� speed");
            return;
        }
        if (float.TryParse(commandAndParameters[1], out float speed))
        {
            if (PlayerController.Instance.ChangeNormalSpeed(speed) < 0)
                Log("�������� ������ ���� ����� 4 � 8");
            else
            {
                Log($"�������� �������� �� {speed}");
                AddToLastCommands(commandAndParameters[0]);
            }
        }
        else
            Log("������������ �������� ��� speed");
    }

    private void ExecuteClear(string[] commandAndParameters)
    {
        if (commandAndParameters.Length > 1)
        {
            Log("������������ ��������� ��� clear");
            return;
        }
        _commandsField.text = "";
        AddToLastCommands(commandAndParameters[0]);
    }
}
