using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class CommandLine : MonoBehaviour
{
    private TMP_InputField _inputField;
    private PlayerController _playerController;
    private GameManager _gm;
    [SerializeField] private TMP_Text _commandsField;

    public bool isSlash = false;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
        _playerController = FindAnyObjectByType<PlayerController>();
        _gm = FindAnyObjectByType<GameManager>();
        _inputField.onSubmit.AddListener(EnterCommand);
    }

    private void OnEnable()
    {
        _inputField.ActivateInputField();
        if (isSlash)
        {
            _inputField.text = "/";
            _inputField.caretPosition = _inputField.text.Length;
            isSlash = false;
        }
        else
            _inputField.text = "";
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

    private void Execute(string line)
    {
        var commandAndParameters = GetCommandAndParameter(line);
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
                    "/open_safe - открывает закрытый сейф в комнате");
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
                    ghost.StartTheGame(_playerController.gameObject);
                    PrintOnConsole("Игра началась");
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
                    _playerController.IsGodMode = true;
                    PrintOnConsole("Режим бога включен");
                }
                else if (commandAndParameters[1] == "0")
                {
                    _playerController.IsGodMode = false;
                    PrintOnConsole("Режим бога выключен");
                }
                else
                    PrintOnConsole("Некорректный параметр для godmode");
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
                break;
            case "open_safe":
                if (commandAndParameters.Length > 1)
                {
                    PrintOnConsole("Некорректные параметры для open_safe");
                    return;
                }
                Safe safe = FindAnyObjectByType<Safe>();
                if (!safe)
                {
                    PrintOnConsole("В комнате не найден закрытый сейф");
                    return;
                }
                safe.OpenSafe();
                PrintOnConsole("Сейф открыт");
                break;
            default:
                PrintOnConsole($"\"/{line}\" не является командой");
                break;
        }

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
