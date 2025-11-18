using System;
using UnityEngine;
using UnityEngine.UI;


public class PuzzleCircles : MonoBehaviour
{
    private const float _circlesRadius = 167.2f;

    [SerializeField] private RectTransform[] _dotsTransforms = new RectTransform[12];
    [SerializeField] private GameObject _completeButton;
    [SerializeField] private Sprite[] _buttonSprites = new Sprite[2];
    private Image _buttonImage;
    private Button _buttonB;

    private int[,] _dotsInCircles = new [,] {{5,1,0,3,7,8},{6,2,1,4,8,9},{9,5,4,7,10,11}};
    private Vector2[] _centersDots = new Vector2[3] {new Vector2(-85,60), new Vector2(85,60), new Vector2(0,-88)};

    private bool _isMoving;
    private bool _isMovingLeft;

    private int _movingCircle;
    
    private float _angle = 0f;
    private float _dotsMoveSpeed = 200f;

    private string _rotateDirection;
    

    private void Awake()
    {
        _dotsTransforms[0].anchoredPosition = new Vector3(-170, 204, 0);
        _dotsTransforms[1].anchoredPosition = new Vector3(0, 204, 0);
        _dotsTransforms[2].anchoredPosition = new Vector3(170, 204, 0);
        _dotsTransforms[3].anchoredPosition = new Vector3(-254, 60, 0);
        _dotsTransforms[4].anchoredPosition = new Vector3(-85, 60, 0);
        _dotsTransforms[5].anchoredPosition = new Vector3(85, 60, 0);
        _dotsTransforms[6].anchoredPosition = new Vector3(254, 60, 0);
        _dotsTransforms[7].anchoredPosition = new Vector3(-170, -88, 0);
        _dotsTransforms[8].anchoredPosition = new Vector3(0, -88, 0);
        _dotsTransforms[9].anchoredPosition = new Vector3(170, -88, 0);
        _dotsTransforms[10].anchoredPosition = new Vector3(-85, -233, 0);
        _dotsTransforms[11].anchoredPosition = new Vector3(85, -233, 0);
        _buttonImage = _completeButton.GetComponent<Image>();
        _buttonB = _completeButton.GetComponent<Button>();
    }

    private void Update()
    {
        if ((_dotsTransforms[1].name.Contains("GreenDot")) && (_dotsTransforms[4].name.Contains("GreenDot")) && (_dotsTransforms[5].name.Contains("GreenDot")) && (_dotsTransforms[7].name.Contains("GreenDot")) && (_dotsTransforms[8].name.Contains("GreenDot")) && (_dotsTransforms[9].name.Contains("GreenDot")))
        {
            _buttonImage.sprite = _buttonSprites[1];
            _buttonB.enabled = true;
        }
        else if (_buttonB)
        {
            _buttonImage.sprite = _buttonSprites[0];
            _buttonB.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            RotateDots();
        }
    }


    private void RotateDots()
    {
        Vector2 center = _centersDots[_movingCircle];
        _angle += _dotsMoveSpeed * Time.deltaTime;
        if (_angle >= 60){_angle = 60;}
        int direction = _isMovingLeft ? 1 : -1;
        for (int dot = 0; dot < 6; dot++)
        {
            float angleRad = ((_angle * direction + 60 * dot) * (MathF.PI / 180));
            float x = center.x + Mathf.Cos(angleRad) * _circlesRadius;
            float y = center.y + Mathf.Sin(angleRad) * _circlesRadius;
            _dotsTransforms[_dotsInCircles[_movingCircle,dot]].anchoredPosition = new Vector3(x, y,0);
        }
        if (_angle == 60)
        {
            _isMoving = false;
            _angle = 0;
            MoveDotsInList();
        }
    }

    public void ArrowClicked(string d)
    {
        if (_isMoving)
            return;
        _rotateDirection = d;
        _isMoving = true;
        switch (d)
        {
            case "LR":
                {
                    _movingCircle = 0;
                    _isMovingLeft = false;
                    break;
                }
            case "LL":
                {
                    _movingCircle = 0;
                    _isMovingLeft = true;
                    break;
                }
            case "RL":
                {
                    _movingCircle = 1;
                    _isMovingLeft = true;
                    break;
                }
            case "RR":
                {
                    _movingCircle = 1;
                    _isMovingLeft = false;
                    break;
                }
            case "BR":
                {
                    _movingCircle = 2;
                    _isMovingLeft = false;
                    break;
                }
            case "BL":
                {
                    _movingCircle = 2;
                    _isMovingLeft = true;
                    break;
                }
        }
    }

    public void MoveDotsInList()
    {
        switch (_rotateDirection)
        {
            case "LR":
                {
                    RectTransform dot = _dotsTransforms[0];
                    _dotsTransforms[0] = _dotsTransforms[3];
                    _dotsTransforms[3] = _dotsTransforms[7];
                    _dotsTransforms[7] = _dotsTransforms[8];
                    _dotsTransforms[8] = _dotsTransforms[5];
                    _dotsTransforms[5] = _dotsTransforms[1];
                    _dotsTransforms[1] = dot;
                    break;
                }
            case "LL":
                {
                    RectTransform dot = _dotsTransforms[0];
                    _dotsTransforms[0] = _dotsTransforms[1];
                    _dotsTransforms[1] = _dotsTransforms[5];
                    _dotsTransforms[5] = _dotsTransforms[8];
                    _dotsTransforms[8] = _dotsTransforms[7];
                    _dotsTransforms[7] = _dotsTransforms[3];
                    _dotsTransforms[3] = dot;
                    break;
                }
            case "RL":
                {
                    RectTransform dot = _dotsTransforms[2];
                    _dotsTransforms[2] = _dotsTransforms[6];
                    _dotsTransforms[6] = _dotsTransforms[9];
                    _dotsTransforms[9] = _dotsTransforms[8];
                    _dotsTransforms[8] = _dotsTransforms[4];
                    _dotsTransforms[4] = _dotsTransforms[1];
                    _dotsTransforms[1] = dot;
                    break;
                }
            case "RR":
                {
                    RectTransform dot = _dotsTransforms[2];
                    _dotsTransforms[2] = _dotsTransforms[1];
                    _dotsTransforms[1] = _dotsTransforms[4];
                    _dotsTransforms[4] = _dotsTransforms[8];
                    _dotsTransforms[8] = _dotsTransforms[9];
                    _dotsTransforms[9] = _dotsTransforms[6];
                    _dotsTransforms[6] = dot;
                    break;
                }
            case "BR":
                {
                    RectTransform dot = _dotsTransforms[10];
                    _dotsTransforms[10] = _dotsTransforms[11];
                    _dotsTransforms[11] = _dotsTransforms[9];
                    _dotsTransforms[9] = _dotsTransforms[5];
                    _dotsTransforms[5] = _dotsTransforms[4];
                    _dotsTransforms[4] = _dotsTransforms[7];
                    _dotsTransforms[7] = dot;
                    break;
                }
            case "BL":
                {
                    RectTransform dot = _dotsTransforms[10];
                    _dotsTransforms[10] = _dotsTransforms[7];
                    _dotsTransforms[7] = _dotsTransforms[4];
                    _dotsTransforms[4] = _dotsTransforms[5];
                    _dotsTransforms[5] = _dotsTransforms[9];
                    _dotsTransforms[9] = _dotsTransforms[11];
                    _dotsTransforms[11] = dot;
                    break;
                }
        }
    }
}
