using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PuzzleStar : MonoBehaviour
{
    //[SerializeField] private Transform _redDot1;
    [SerializeField] private RectTransform[] _dots = new RectTransform[8];
    [SerializeField] private GameObject[] _arrows = new GameObject[8];
    [SerializeField] private Sprite[] _buttonSprites = new Sprite[2];
    [SerializeField] private GameObject _completeButton;
    private Image _buttonImage;
    private Button _buttonB;


    private Vector3[] _dotsPos = new Vector3[8];


    private void Awake()
    {
        _dotsPos[0] = new Vector3(-137, 137, 0);
        _dotsPos[1] = new Vector3(0, 195, 0);
        _dotsPos[2] = new Vector3(137, 137, 0);
        _dotsPos[3] = new Vector3(195, 0, 0);
        _dotsPos[4] = new Vector3(137, -137, 0);
        _dotsPos[5] = new Vector3(0, -195, 0);
        _dotsPos[6] = new Vector3(-137, -137, 0);
        _dotsPos[7] = new Vector3(-195, 0, 0);
        _buttonImage = _completeButton.GetComponent<Image>();
        _buttonB = _completeButton.GetComponent<Button>();
    }

    private void Update()
    {
        if (_dots[0] && _dots[2] && _dots[4] && _dots[6])
        {
            if ((_dots[0].name.Contains("BlueDot")) && (_dots[2].name.Contains("BlueDot")) && (_dots[4].name.Contains("RedDot")) && (_dots[6].name.Contains("RedDot")))
            {
                _buttonImage.sprite = _buttonSprites[1];
                _buttonB.enabled = true;
            }
            else
            {
                _buttonImage.sprite = _buttonSprites[0];
                _buttonB.enabled = false;
            }
        }
        else if (_buttonB.enabled)
        {
            _buttonImage.sprite = _buttonSprites[0];
            _buttonB.enabled = false;
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
