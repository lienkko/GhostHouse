using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChest : MonoBehaviour
{
    // Изменили список имен: теперь тут только твоя новая головоломка
    private readonly string[] _puzzleNames = new string[] { "Numbers" };
    public static bool IsInPuzzle { get; private set; } = false;

    private GameObject _puzzle;
    private Interactive _interactiveComp;
    private SpriteRenderer _spriteRenderer;
    private bool _isPlayerNearby = false;

    [Header("Chest Settings")]
    [SerializeField] private Sprite _openedChestSprite;
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private int _coinCount = 5;
    [SerializeField] private float _spawnForce = 5f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _interactiveComp = GetComponent<Interactive>();

        _interactiveComp.SetListener(OpenPuzzle);
        _interactiveComp.isInteractive = true;

        Pause.OnResume += ChestOnResume;
    }

    private void Start()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnDeath += ClosePuzzle;
    }

    private void Update()
    {
        if (_isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !IsInPuzzle)
        {
            OpenPuzzle();
        }

        if (CanClosePuzzle())
        {
            ClosePuzzle();
        }
    }

    private bool CanClosePuzzle() =>
        !Pause.IsPaused && IsInPuzzle && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) && !Console.Instance.IsConsoleOpened;

    private void CreatePuzzle()
    {
        // Путь теперь будет: "Prefabs/Puzzles/PuzzleNumbers"
        string puzzleName = $"Prefabs/Puzzles/Puzzle{_puzzleNames[0]}";
        GameObject prefab = Resources.Load<GameObject>(puzzleName);

        if (prefab == null)
        {
            Debug.LogError($"Не найден префаб по пути: {puzzleName}");
            return;
        }

        _puzzle = Instantiate(prefab, transform);

        // ВАЖНО: Ищем кнопку завершения в твоем префабе и вешаем на неё открытие сундука
        // Убедись, что в префабе путь именно такой: Canvas/CompleteButton
        Transform completeBtn = _puzzle.transform.Find("Canvas/CompleteButton");
        if (completeBtn != null)
        {
            completeBtn.GetComponent<Button>().onClick.AddListener(OpenChest);
        }
    }

    public void OpenPuzzle()
    {
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(SwitchIsInPuzzle(true));
        GameManager.Instance.BlockPlayer(true);

        if (_puzzle)
        {
            _puzzle.SetActive(true);
            // Если в скрипте NumberPuzzle есть метод SetupPuzzle, вызываем его для новой игры
            _puzzle.GetComponent<NumberPuzzle>()?.SetupPuzzle();
            return;
        }
        CreatePuzzle();
    }

    public void ClosePuzzle()
    {
        if (PlayerController.Instance != null && PlayerController.Instance.IsAlive)
            Cursor.lockState = CursorLockMode.Locked;

        if (_puzzle)
        {
            StartCoroutine(SwitchIsInPuzzle(false));
            _puzzle.SetActive(false);
        }
        GameManager.Instance.BlockPlayer(false);
    }

    private IEnumerator SwitchIsInPuzzle(bool state)
    {
        yield return null;
        if (_interactiveComp != null) _interactiveComp.isInteractive = !state;
        IsInPuzzle = state;
    }

    private void ChestOnResume()
    {
        if (IsInPuzzle)
        {
            GameManager.Instance.BlockPlayer(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OpenChest()
    {
        if (_puzzle)
        {
            ClosePuzzle();
            Destroy(_puzzle);
        }

        if (_openedChestSprite != null)
            _spriteRenderer.sprite = _openedChestSprite;

        SpawnCoins();

        _interactiveComp.RemoveListener();
        _interactiveComp.isInteractive = false;

        if (GameManager.Instance.GameUIFields.OpenSafeText != null)
            GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);

        // Чтобы скрипт сундука не удалился раньше, чем сработают монеты, 
        // мы просто выключаем его, а не удаляем объект
        this.enabled = false;
    }

    private void SpawnCoins()
    {
        if (_coinPrefab == null) return;

        for (int i = 0; i < _coinCount; i++)
        {
            GameObject coin = Instantiate(_coinPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 force = new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 2f)).normalized * _spawnForce;
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _interactiveComp.isInteractive)
        {
            _isPlayerNearby = true;
            if (GameManager.Instance.GameUIFields.OpenSafeText != null)
                GameManager.Instance.GameUIFields.OpenSafeText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerNearby = false;
            if (GameManager.Instance.GameUIFields.OpenSafeText != null)
                GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);
        }
    }
}