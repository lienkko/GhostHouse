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
    private int _targetCount = 10; // Всего 10 цифр

    private void Start()
    {
        SetupPuzzle();
    }

    public void SetupPuzzle()
    {
        _currentNumber = 1;
        _completeButton.SetActive(false);
        if (_statusText != null) _statusText.text = "Нажми: 1";

        // Создаем список цифр от 1 до 10 и перемешиваем
        List<int> numbers = new List<int>();
        for (int i = 1; i <= _targetCount; i++) numbers.Add(i);

        for (int i = 0; i < _numberButtons.Length; i++)
        {
            int index = i;
            int numberValue = numbers[Random.Range(0, numbers.Count)];
            numbers.Remove(numberValue);

            // Настраиваем текст на кнопке
            _numberButtons[i].GetComponentInChildren<Text>().text = numberValue.ToString();

            // Сбрасываем цвета и события
            _numberButtons[i].interactable = true;
            _numberButtons[i].GetComponent<Image>().color = Color.white;
            _numberButtons[i].onClick.RemoveAllListeners();

            // Добавляем логику нажатия
            _numberButtons[i].onClick.AddListener(() => OnClick(numberValue, _numberButtons[index]));
        }
    }

    private void OnClick(int val, Button btn)
    {
        if (val == _currentNumber)
        {
            btn.interactable = false;
            btn.GetComponent<Image>().color = Color.green; // Подсветим верную
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
            // Ошибка: можно добавить тряску или звук, но пока просто ничего не происходит
            Debug.Log("Не та цифра!");
        }
        if (_currentNumber > _targetCount)
        {
            _completeButton.SetActive(true);
            if (_statusText != null) _statusText.text = "ГОТОВО!";

            // ДОБАВЬ ВОТ ЭТО:
            // Мы ищем сундук на сцене и говорим ему открыться при нажатии
            _completeButton.GetComponent<Button>().onClick.AddListener(() => {
                FindObjectOfType<TreasureChest>().OpenChest();
            });
        }
    }
}