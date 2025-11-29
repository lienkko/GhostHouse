using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    enum MD {Up, Down};
    private MD _moveDirection = MD.Down;
    private float _time = 0;
    public delegate void StartGameDelegate();
    public event StartGameDelegate OnStartGame;

    [SerializeField] private float _speed = 1;
    [SerializeField] private Transform _point1;
    [SerializeField] private Transform _point2;
    [SerializeField] private Transform _ghost;
    [SerializeField] private Sprite _spriteDown;
    [SerializeField] private Sprite _spriteUp;

    private void Awake()
    {
        GetComponent<Interactive>().SetListener(StartTheGame);
        GetComponent<Interactive>().isInteractive = true;
    }

    private void FixedUpdate()
    {
        if (_moveDirection == MD.Down)
        {
            _ghost.position = Vector3.Lerp(_point1.position, _point2.position, _time);
            _time += Time.deltaTime * _speed;
            if (_time >= 1)
            {
                _ghost.GetComponent<SpriteRenderer>().sprite = _spriteUp;
                _moveDirection = MD.Up;
            }
        }
        else
        {
            _ghost.position = Vector3.Lerp(_point1.position, _point2.position, _time);
            _time -= Time.deltaTime * _speed;
            if (_time <= 0)
            {
                _ghost.GetComponent<SpriteRenderer>().sprite = _spriteDown;
                _moveDirection = MD.Down;
            }
        }
    }

    private void StartTheGame(GameObject obj)
    {
        GetComponent<Interactive>().isInteractive = false;
        OnStartGame?.Invoke();
    }
}
