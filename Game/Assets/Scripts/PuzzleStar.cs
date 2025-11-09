using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleStar : MonoBehaviour
{
    //[SerializeField] private Transform _redDot1;
    [SerializeField] private RectTransform[] _dots = new RectTransform[8];
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
    }

    public void MoveDot(int path)
    {
        int from = path / 10;
        int to = path % 10;
        if (_dots[from])
        {
            _dots[to] = _dots[from];
            _dots[from] = null;
            _dots[to].anchoredPosition = _dotsPos[to];
        }
    }
}
