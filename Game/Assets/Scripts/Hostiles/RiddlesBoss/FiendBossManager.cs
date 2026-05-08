using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RiddlesBossManager : MonoBehaviour, IInteractive
{
    private enum FiendStates
    {
        PlayerLost = -3,
        Rage = -2,
        Anger = -1,
        Calm = 0,
        Upset = 1,
        Gutted = 2,
        PlayerWon = 3
    }
    private struct FormattedRiddle
    {
        public string Riddle;
        public string RightAnswer;
        public string[] VariantsOfAnswer;
    }
    private Dictionary<FiendStates, string> _fiendStates = new()
    { {FiendStates.Rage, "Rage"},{FiendStates.Anger,"Anger"}, {FiendStates.Calm, "Calm"}, {FiendStates.Upset,"Upset"}, {FiendStates.Gutted,"Gutted"}, {FiendStates.PlayerLost,"Killing"}, {FiendStates.PlayerWon,"Passing"}};
    private readonly string[][] _riddles =
    {
    new string[] { "Что становится больше, если от него отнять часть?", "Яма", "Яма", "Долг", "Тень", "Очередь" },
    new string[] { "Чем больше берёшь, тем больше оставляешь. Что это?", "Следы", "Следы", "Долги", "Воспоминания", "Ошибки" },
    new string[] { "Что может путешествовать по миру, оставаясь в одном углу?", "Марка", "Марка", "Компас", "Тень", "Карта" },
    new string[] { "Что всегда идёт вперёд, но никогда не движется назад?", "Время", "Время", "Река", "Солнце", "Возраст" },
    new string[] { "У кого есть ключи, но нет замков?", "Пианист", "Пианист", "Сторож", "Программист", "Капитан" },
    new string[] { "Что можно держать только после того, как отдашь?", "Слово", "Слово", "Обещание", "Долг", "Победу" },
    new string[] { "Чем суше становится предмет, тем больше влаги он впитывает. Что это?", "Полотенце", "Полотенце", "Губка", "Песок", "Бумага" },
    new string[] { "Что принадлежит тебе, но другие используют это чаще?", "Имя", "Имя", "Телефон", "Голос", "Время" },
    new string[] { "Что можно сломать, даже не прикасаясь?", "Тишина", "Обещание", "Тишина", "Сердце", "Репутацию" },
    new string[] { "Что имеет города, но не имеет домов; леса, но не деревьев; реки, но не воды?", "Карта", "Карта", "Глобус", "Сон", "Игра" },

    new string[] { "Что поднимается вверх, но никогда не опускается?", "Возраст", "Возраст", "Дым", "Температура", "Рейтинг" },
    new string[] { "Чем больше у тебя этого, тем меньше ты видишь. Что это?", "Темнота", "Темнота", "Страх", "Сонливость", "Туман" },
    new string[] { "Что может заполнить комнату, не занимая места?", "Свет", "Свет", "Запах", "Звук", "Тепло" },
    new string[] { "Что всегда перед тобой, но ты не можешь этого увидеть?", "Будущее", "Будущее", "Воздух", "Следующий шаг", "Судьба" },
    new string[] { "Что имеет много зубов, но не может кусаться?", "Расчёска", "Расчёска", "Пила", "Молния", "Шестерёнка" },
    new string[] { "Чем больше ты бежишь от этого, тем быстрее оно догоняет.", "Страх", "Время", "Усталость", "Старость", "Страх" },
    new string[] { "Что можно увидеть один раз в минуте, два раза в моменте и ни разу в тысяче лет?", "Букву «м»", "Букву «м»", "Букву «о»", "Букву «т»", "Букву «и»" },
    new string[] { "Что ломается, если назвать его?", "Тишина", "Тишина", "Секрет", "Сон", "Концентрация" },
    new string[] { "Что имеет руки, но не может хлопать?", "Часы", "Часы", "Манекен", "Статуя", "Робот" },
    new string[] { "Что всегда падает, но никогда не разбивается?", "Ночь", "Ночь", "Снег", "Дождь", "Тень" },

    new string[] { "Что становится легче, если его разделить?", "Горе", "Горе", "Ноша", "Ответственность", "Работа" },
    new string[] { "Что можно услышать, но нельзя увидеть или потрогать?", "Эхо", "Эхо", "Ветер", "Мысль", "Голос" },
    new string[] { "Что имеет начало, но не имеет конца?", "Луч", "Кольцо", "Луч", "Число", "Круг" },
    new string[] { "Чем больше ты кормишь это, тем сильнее оно становится. Но дай ему воды — и оно умрёт.", "Огонь", "Огонь", "Жадность", "Гнев", "Растение" },
    new string[] { "Что всегда отвечает, но никогда не задаёт вопросов?", "Эхо", "Эхо", "Книга", "Учитель", "Интернет" },
    new string[] { "Что можно открыть, не прикасаясь руками?", "Сердце", "Разговор", "Сердце", "Счёт", "Дверь" },
    new string[] { "Что растёт вниз?", "Корни", "Корни", "Сталактиты", "Тень", "Якорь" },
    new string[] { "Что может быть острым, но не является ножом?", "Ум", "Ум", "Взгляд", "Язык", "Слух" },
    new string[] { "Что нельзя удержать дольше нескольких секунд, даже если оно твоё?", "Дыхание", "Дыхание", "Внимание", "Молчание", "Время" },
    new string[] { "Что идёт, не двигая ногами?", "Время", "Время", "Дождь", "Часы", "Речь" },

    new string[] { "Что может быть тяжёлым и лёгким одновременно?", "Решение", "Решение", "Воздух", "Сердце", "Сон" },
    new string[] { "Что теряет голову утром и возвращает вечером?", "Подушка", "Подушка", "Кровать", "Монета", "Зубная щётка" },
    new string[] { "Что невозможно удержать, если произнести его название?", "Тишина", "Тишина", "Секрет", "Молчание", "Внимание" },
    new string[] { "Что всегда рядом, но его нельзя коснуться?", "Горизонт", "Горизонт", "Тень", "Прошлое", "Воздух" },
    new string[] { "Что имеет шею, но не имеет головы?", "Бутылка", "Бутылка", "Гитара", "Лампа", "Ваза" },
    new string[] { "Что можно поймать, но нельзя бросить?", "Простуду", "Простуду", "Идею", "Взгляд", "Момент" },
    new string[] { "Что становится короче, когда растёт?", "Свеча", "Свеча", "Тень", "Волосы", "Верёвка" },
    new string[] { "Что всегда возвращается, даже если его прогоняют?", "Мысли", "Мысли", "Эхо", "Холод", "Ночь" },
    new string[] { "Что не задаёт вопросов, но требует ответов?", "Экзамен", "Экзамен", "Жизнь", "Взгляд", "Совесть" },
    new string[] { "Что может быть закрыто, даже если у него нет двери?", "Тема", "Вопрос", "Тема", "Глаза", "Дело" },

    new string[] { "Что может быть твоим, даже если ты этого никогда не видел?", "Будущее", "Будущее", "Наследство", "Голос", "Судьба" },
    new string[] { "Что можно потерять за секунду, а возвращать годами?", "Доверие", "Доверие", "Деньги", "Репутацию", "Дружбу" },
    new string[] { "Что имеет язык, но не умеет говорить?", "Ботинок", "Ботинок", "Колокол", "Пламя", "Волна" },
    new string[] { "Что приходит без приглашения и уходит без предупреждения?", "Удача", "Сон", "Удача", "Вдохновение", "Дождь" },
    new string[] { "Что нельзя увидеть, пока оно не исчезнет?", "Воздух", "Здоровье", "Воздух", "Время", "Молодость" },
    new string[] { "Что чем быстрее бежит, тем труднее догнать?", "Время", "Время", "Мысль", "Тень", "Слух" },
    new string[] { "Что может соединять людей, даже если его нельзя увидеть?", "Доверие", "Доверие", "Интернет", "Память", "Голос" },
    new string[] { "Что можно открыть и закрыть, не используя замок?", "Глаза", "Глаза", "Разговор", "Сделку", "Книгу" },
    new string[] { "Что никогда не врёт, но может обманывать?", "Зеркало", "Зеркало", "Тень", "Время", "Память" },
    new string[] { "Что исчезает в тот момент, когда пытаешься этим поделиться?", "Одиночество", "Одиночество", "Секрет", "Тишина", "Момент" }
};

    public KeyCode KeyToInteract { get; } = KeyCode.E;
    public bool IsInteractive { get; private set; } = true;
    public string HintText { get; } = "Talk - E";

    [SerializeField] private GameObject _frontBarrier;

    [SerializeField] private Transform _playerBindPoint;

    [SerializeField] private Animator _fiendAnimator;

    [SerializeField] private GameObject _dialogueWindow;
    [SerializeField] private TextMeshProUGUI _dialogueTextField;
    [SerializeField] private TextMeshProUGUI _riddleTextField;
    [SerializeField] private TextMeshProUGUI[] _variantsTextFields;
    private readonly int NumOfVariants = 4;

    [SerializeField] private bool _isWaitingForAnswer = false;
    [SerializeField] private bool _isWaitingForSkip = false;
    [SerializeField] private bool _isPlaying = false;
    [SerializeField] private bool _isGameOver = false;
    private int _rightAnswer;

    private FiendStates _fiendState = FiendStates.Calm;

    private void Awake()
    {
        UpdateAnimatorState();
    }
    private void Update()
    {
        if (!_isPlaying) return;

        if (_isPlaying || _isGameOver)
            GameManager.Instance.BlockPlayer(true);
        if (_fiendState == FiendStates.PlayerWon) FinalDialogue();
        else if (_fiendState == FiendStates.PlayerLost) EndGame();

        if (Pause.IsPaused) return;

        if (!_isWaitingForSkip && !_isWaitingForAnswer)
        {
            SetRiddle();
            _isWaitingForAnswer = true;
        }

        if (_isWaitingForSkip && Input.GetKeyDown(KeyCode.Space))
        {
            _isWaitingForSkip = false;
            if (_isGameOver)
                EndGame();
            return;
        }
        if (_isWaitingForAnswer)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeStateWithAnswer(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeStateWithAnswer(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeStateWithAnswer(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ChangeStateWithAnswer(4);
            }
        }
    }
    public void Interact()
    {
        BindPlayer(true);
        FirstDialogue();
        _isPlaying = true;
    }
    public bool CanInteract() { return GameManager.CanUseKeyboard && IsInteractive; }

    private void BindPlayer(bool isStart)
    {
        if (isStart)
            PlayerController.Instance.transform.position = _playerBindPoint.position;
        GameManager.Instance.BlockPlayer(isStart);
    }
    private void FirstDialogue()
    {
        _dialogueWindow.SetActive(true);
        _dialogueTextField.text = "Приветствую тебя!" + "\n" + "Я Fiend" + "\n" + " Чтобы пройти дальше, тебе нужно верно ответить на мои вопросы!";
        _isWaitingForSkip = true;
    }
    private void FinalDialogue()
    {
        _isGameOver = true;
        _dialogueTextField.text = "Ну что же..." + "\n" + "Ты умный смертный" + "\n" + "Можешь проходить дальше";
        _isWaitingForSkip = true;
    }
    private void RightAnswerDialogue()
    {
        _riddleTextField.text = null;
        foreach (var f in _variantsTextFields)
        {
            f.text = null;
        }
        _dialogueTextField.text = "Верно((((";
        _isWaitingForSkip = true;
    }
    private void WrongAnswerDialogue()
    {
        _riddleTextField.text = null;
        foreach (var f in _variantsTextFields)
        {
            f.text = null;
        }
        _dialogueTextField.text = "Не верно!";
        _isWaitingForSkip = true;
    }
    private void EndGame()
    {
        if (_fiendState == FiendStates.PlayerLost)
            PlayerController.Instance.InflictDamage(100);
        BindPlayer(false);
        _frontBarrier.SetActive(false);
        _isPlaying = false;
        _isGameOver = false;
        _dialogueWindow.SetActive(false);
        UpdateAnimatorState();
        Destroy(gameObject);
    }


    private FormattedRiddle ChooseRiddle()
    {
        int riddleIndex = Random.Range(0, _riddles.Length);
        string[] rid = _riddles[riddleIndex];
        return new FormattedRiddle { Riddle = rid[0], RightAnswer = rid[1], VariantsOfAnswer = rid[2..6] };
    }
    void Shuffle(string[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = Random.Range(i, array.Length);
            (array[randomIndex], array[i]) = (array[i], array[randomIndex]);
        }
    }
    private void SetRiddle()
    {
        _dialogueTextField.text = null;
        var rid = ChooseRiddle();
        _riddleTextField.text = rid.Riddle;
        var randVariants = rid.VariantsOfAnswer;
        Shuffle(randVariants);
        for (int i = 0; i < NumOfVariants; i++)
        {
            _variantsTextFields[i].text = $"{i + 1}. {rid.VariantsOfAnswer[i]}";
        }
        _rightAnswer = System.Array.IndexOf(randVariants, rid.RightAnswer) + 1;
    }
    private void ChangeStateWithAnswer(int answer)
    {
        if (answer == _rightAnswer)
        {
            _fiendState += 1;
            RightAnswerDialogue();
        }
        else
        {
            _fiendState -= 1;
            WrongAnswerDialogue();
        }
        _isWaitingForAnswer = false;
        UpdateAnimatorState();
    }
    private void UpdateAnimatorState()
    {
        foreach (var st in _fiendStates.Keys)
        {
            if (st == _fiendState)
                _fiendAnimator.SetBool(_fiendStates[st], true);
            else
                _fiendAnimator.SetBool(_fiendStates[st], false);
        }
    }
}
