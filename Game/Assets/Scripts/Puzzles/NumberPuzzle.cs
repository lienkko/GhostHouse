using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NumberPuzzle : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private Button[] _numberButtons;
    [SerializeField] private GameObject _completeButton;
    [SerializeField] private Text _statusText;

    private int _currentNumber = 1;
    private readonly int _targetCount = 10;

    private void Start()
    {
        SetupPuzzle();
    }

    public void SetupPuzzle()
    {
        _currentNumber = 1;
        _completeButton.SetActive(false);
        _statusText.text = "Нажми: 1";

        List<int> numbers = new();
        for (int i = 1; i <= _targetCount; i++) numbers.Add(i);

        for (int i = 0; i < _targetCount; i++)
        {
            int index = i;
            int numberValue = numbers[Random.Range(0, numbers.Count)];
            numbers.Remove(numberValue);

            _numberButtons[i].GetComponentInChildren<Text>().text = numberValue.ToString();

            _numberButtons[i].interactable = true;
            _numberButtons[i].GetComponent<Image>().color = Color.white;
            _numberButtons[i].onClick.RemoveAllListeners();

            _numberButtons[i].onClick.AddListener(() => OnClick(numberValue, _numberButtons[index]));
        }
    }

    private void OnClick(int val, Button btn)
    {
        if (val == _currentNumber)
        {
            btn.interactable = false;
            btn.GetComponent<Image>().color = Color.green;
            _currentNumber++;

            if (_currentNumber > _targetCount)
            {
                _completeButton.SetActive(true);
                if (_statusText != null) _statusText.text = "ГОТОВО!";
            }
            else
            {
                if (_statusText != null) _statusText.text = $"Нажми: {_currentNumber}";
            }
        }
        else
        {
            Debug.Log("Не та цифра!");
        }
    }
}