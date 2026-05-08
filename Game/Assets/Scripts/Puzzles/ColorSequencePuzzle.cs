using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSequencePuzzle : MonoBehaviour
{
    [SerializeField] private Button[] _tiles;
    [SerializeField] private GameObject _completeButton;
    [SerializeField] private int _sequenceLength = 5;
    [SerializeField] private int _roundsToSolve = 3;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _activeColor = Color.yellow;
    [SerializeField] private Color _wrongColor = Color.red;
    [SerializeField] private Color _completeColor = Color.green;
    [SerializeField] private float _flashTime = 0.45f;
    [SerializeField] private float _pauseTime = 0.2f;

    private readonly List<int> _sequence = new();
    private int _inputIndex;
    private int _round;
    private bool _canClick;
    private bool _isInitialized;

    private void Start()
    {
        if (_tiles == null || _tiles.Length == 0)
            BuildView();

        SetupButtons();
        _isInitialized = true;
        StartPuzzle();
    }

    private void OnEnable()
    {
        if (_isInitialized)
            StartPuzzle();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _canClick = false;
    }

    public void StartPuzzle()
    {
        StopAllCoroutines();
        _canClick = false;
        _round = 0;
        _completeButton.SetActive(false);
        PaintAll(_normalColor);
        StartRound();
    }

    private void SetupButtons()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            int index = i;
            _tiles[i].onClick.RemoveAllListeners();
            _tiles[i].onClick.AddListener(() => TileClicked(index));
        }
    }

    private void StartRound()
    {
        _sequence.Clear();
        _inputIndex = 0;

        for (int i = 0; i < Mathf.Max(1, _sequenceLength); i++)
            _sequence.Add(Random.Range(0, _tiles.Length));

        StartCoroutine(ShowSequence());
    }

    private void TileClicked(int index)
    {
        if (!_canClick)
            return;

        if (index != _sequence[_inputIndex])
        {
            StartCoroutine(WrongClick());
            return;
        }

        _inputIndex++;
        if (_inputIndex >= _sequence.Count)
        {
            StartCoroutine(RoundComplete());
            return;
        }

        StartCoroutine(FlashClick(index));
    }

    private IEnumerator ShowSequence()
    {
        _canClick = false;
        PaintAll(_normalColor);

        foreach (int index in _sequence)
        {
            Paint(index, _activeColor);
            yield return new WaitForSeconds(_flashTime);
            Paint(index, _normalColor);
            yield return new WaitForSeconds(_pauseTime);
        }

        _canClick = true;
    }

    private IEnumerator WrongClick()
    {
        _canClick = false;
        _inputIndex = 0;
        PaintAll(_wrongColor);
        yield return new WaitForSeconds(_flashTime);
        yield return ShowSequence();
    }

    private IEnumerator FlashClick(int index)
    {
        Paint(index, _activeColor);
        yield return new WaitForSeconds(_flashTime);
        Paint(index, _normalColor);
    }

    private IEnumerator RoundComplete()
    {
        _canClick = false;
        _round++;
        PaintAll(_completeColor);
        yield return new WaitForSeconds(_flashTime);

        if (_round >= Mathf.Max(1, _roundsToSolve))
        {
            _completeButton.SetActive(true);
            _completeButton.transform.SetAsLastSibling();
            yield break;
        }

        StartRound();
    }

    private void PaintAll(Color color)
    {
        for (int i = 0; i < _tiles.Length; i++)
            Paint(i, color);
    }

    private void Paint(int index, Color color)
    {
        _tiles[index].GetComponent<Image>().color = color;
    }

    private void BuildView()
    {
        Transform viewRoot = transform.Find("Canvas");
        if (viewRoot == null)
            viewRoot = transform;

        RectTransform panel = CreateRect("Panel", viewRoot, new Vector2(620, 380));
        panel.anchoredPosition = Vector2.zero;
        panel.SetAsFirstSibling();

        Image panelImage = panel.gameObject.AddComponent<Image>();
        panelImage.color = new Color(0.08f, 0.08f, 0.1f, 0.92f);
        panelImage.raycastTarget = false;

        _tiles = new Button[8];
        for (int i = 0; i < _tiles.Length; i++)
        {
            RectTransform tile = CreateRect($"Tile{i + 1}", panel, new Vector2(95, 95));
            tile.anchoredPosition = new Vector2(-225 + i % 4 * 150, i < 4 ? 85 : -45);
            Image image = tile.gameObject.AddComponent<Image>();
            image.color = _normalColor;
            _tiles[i] = tile.gameObject.AddComponent<Button>();
            _tiles[i].targetGraphic = image;
        }

        if (_completeButton == null)
            CreateCompleteButton(panel);
    }

    private void CreateCompleteButton(Transform parent)
    {
        RectTransform complete = CreateRect("CompleteButton", parent, new Vector2(220, 55));
        complete.anchoredPosition = new Vector2(0, -150);
        Image completeImage = complete.gameObject.AddComponent<Image>();
        completeImage.color = _completeColor;
        complete.gameObject.AddComponent<Button>().targetGraphic = completeImage;
        AddCompleteButtonText(complete);
        _completeButton = complete.gameObject;
    }

    private void AddCompleteButtonText(RectTransform complete)
    {
        Text text = CreateRect("Text", complete, complete.sizeDelta).gameObject.AddComponent<Text>();
        text.text = "\u0437\u0430\u0432\u0435\u0440\u0448\u0438\u0442\u044c";
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 28;
        text.raycastTarget = false;
    }

    private RectTransform CreateRect(string objectName, Transform parent, Vector2 size)
    {
        GameObject view = new(objectName, typeof(RectTransform));
        RectTransform rect = view.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        return rect;
    }
}
