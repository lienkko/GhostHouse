using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PuzzleCircles : MonoBehaviour
{
    [SerializeField] private RectTransform[] _dots = new RectTransform[12];
    private Vector3[] _dotsPos;
    private int[,] _dotsInCircle; // Хранит номера точек, которые находятся в кажом кругу

    private void Awake()
    {
        _dotsPos = new Vector3[12];
        _dotsPos[0] = new Vector3(-170,204,0);
        _dotsPos[1] = new Vector3(0,204,0);
        _dotsPos[2] = new Vector3(170,204,0);
        _dotsPos[3] = new Vector3(-254,60,0);
        _dotsPos[4] = new Vector3(-85,60,0);
        _dotsPos[5] = new Vector3(85,60,0);
        _dotsPos[6] = new Vector3(254, 60, 0);
        _dotsPos[7] = new Vector3(-170,-88,0);
        _dotsPos[8] = new Vector3(0,-88,0);
        _dotsPos[9] = new Vector3(170,-88,0);
        _dotsPos[10] = new Vector3(-85,-233,0);
        _dotsPos[11] = new Vector3(85,-233,0);
        _dotsInCircle = new [,] {{0,1,5,8,7,3},{2,6,9,8,4,1},{10,7,4,5,9,11}};
        for (int i = 0; i<12;i++)
        {
            _dots[i].anchoredPosition = _dotsPos[i];
        }
    }

    private void MoveDots(int circ)
    {
        int dotIndex;
        for (int i = 0; i < 6;i++)
        {
            dotIndex = _dotsInCircle[circ, i]; //индекс точки которая должна двигаться
            _dots[dotIndex].anchoredPosition = _dotsPos[dotIndex];
        }
    }

    public void MoveCircle(string d)
    {
        int c = 0;
        switch (d)
        {
            case "LR":
                {
                    RectTransform dot = _dots[0];
                    _dots[0] = _dots[3];
                    _dots[3] = _dots[7];
                    _dots[7] = _dots[8];
                    _dots[8] = _dots[5];
                    _dots[5] = _dots[1];
                    _dots[1] = dot;
                    c = 0;
                    break;
                }
            case "LL":
                {
                    RectTransform dot = _dots[0];
                    _dots[0] = _dots[1];
                    _dots[1] = _dots[5];
                    _dots[5] = _dots[8];
                    _dots[8] = _dots[7];
                    _dots[7] = _dots[3];
                    _dots[3] = dot;
                    c = 0;
                    break;
                }
            case "RL":
                {
                    RectTransform dot = _dots[2];
                    _dots[2] = _dots[6];
                    _dots[6] = _dots[9];
                    _dots[9] = _dots[8];
                    _dots[8] = _dots[4];
                    _dots[4] = _dots[1];
                    _dots[1] = dot;
                    c = 1;
                    break;
                }
            case "RR":
                {
                    RectTransform dot = _dots[2];
                    _dots[2] = _dots[1];
                    _dots[1] = _dots[4];
                    _dots[4] = _dots[8];
                    _dots[8] = _dots[9];
                    _dots[9] = _dots[6];
                    _dots[6] = dot;
                    c = 1;
                    break;
                }
            case "BR":
                {
                    RectTransform dot = _dots[10];
                    _dots[10] = _dots[11];
                    _dots[11] = _dots[9];
                    _dots[9] = _dots[5];
                    _dots[5] = _dots[4];
                    _dots[4] = _dots[7];
                    _dots[7] = dot;
                    c = 2;
                    break;
                }
            case "BL":
                {
                    RectTransform dot = _dots[10];
                    _dots[10] = _dots[7];
                    _dots[7] = _dots[4];
                    _dots[4] = _dots[5];
                    _dots[5] = _dots[9];
                    _dots[9] = _dots[11];
                    _dots[11] = dot;
                    c = 2;
                    break;
                }
        }
        MoveDots(c);
        
    }
}
