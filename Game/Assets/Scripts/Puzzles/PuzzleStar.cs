using UnityEngine;

using UnityEngine.UI;

public class PuzzleStar : MonoBehaviour
{
    [SerializeField] private RectTransform[] _dots = new RectTransform[8];
    [SerializeField] private GameObject[] _arrows = new GameObject[8];
    [SerializeField] private Sprite[] _buttonSprites = new Sprite[2];
    [SerializeField] private GameObject _completeButton;
    private Image _buttonImage;
    private Button _buttonB;


    private Vector3[] _dotsPos = new Vector3[8];


    private void Awake()
    {
        SetDotsPos();
        _buttonImage = _completeButton.GetComponent<Image>();
        _buttonB = _completeButton.GetComponent<Button>();
    }

    private void SetDotsPos()
    {
        _dotsPos[0] = new Vector3(-137, 137, 0);
        _dotsPos[1] = new Vector3(0, 195, 0);
        _dotsPos[2] = new Vector3(137, 137, 0);
        _dotsPos[3] = new Vector3(195, 0, 0);
        _dotsPos[4] = new Vector3(137, -137, 0);
        _dotsPos[5] = new Vector3(0, -195, 0);
        _dotsPos[6] = new Vector3(-137, -137, 0);
        _dotsPos[7] = new Vector3(-195, 0, 0);
    }

    private bool DotsOnRightPlaces() {
        return _dots[0].name.Contains("BlueDot") &&
            _dots[2].name.Contains("BlueDot") && 
            _dots[4].name.Contains("RedDot") &&
            _dots[6].name.Contains("RedDot");
    }

    private void EnableButton(bool state)
    {
        _buttonImage.sprite = _buttonSprites[state?1:0];
        _buttonB.enabled = state;
    }

    private void Update()
    {
        if (_dots[0] && _dots[2] && _dots[4] && _dots[6])
        {
            if (DotsOnRightPlaces())
            {
                EnableButton(true);
            }
            else if (_buttonB.enabled)
            {
                EnableButton(false);
            }
        }
        else if (_buttonB.enabled)
        {
            EnableButton(false);
        }

    }



    public void MoveDot(int path)
    {
        int from = path / 10;
        int to = path % 10;
        if (_dots[from] && !_dots[to])
        {
            _arrows[from].SetActive(false);
            _arrows[to].SetActive(true);
            _dots[to] = _dots[from];
            _dots[from] = null;
            _dots[to].anchoredPosition = _dotsPos[to];
        }
    }
}
