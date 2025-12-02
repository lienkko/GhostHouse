using UnityEngine;

public class Ghost : MonoBehaviour
{
    public static Ghost Instance { get; private set; }

    public Interactive InteractiveInstance { get; private set; }


    enum MD {Up, Down};


    [SerializeField] private float _speed = 1;
    [SerializeField] private Transform _point1;
    [SerializeField] private Transform _point2;
    [SerializeField] private Transform _ghost;
    [SerializeField] private Sprite _spriteDown;
    [SerializeField] private Sprite _spriteUp;

    private MD _moveDirection = MD.Down;
    private float _flightTime = 0;

    private void Awake()
    {
        Instance = this;
        InteractiveInstance = GetComponent<Interactive>();
        InteractiveInstance.isInteractive = true;
    }

    private void FixedUpdate()
    {
        if (_moveDirection == MD.Down)
        {
            _ghost.position = Vector3.Lerp(_point1.position, _point2.position, _flightTime);
            _flightTime += Time.deltaTime * _speed;
            if (_flightTime >= 1)
            {
                _ghost.GetComponent<SpriteRenderer>().sprite = _spriteUp;
                _moveDirection = MD.Up;
            }
        }
        else
        {
            _ghost.position = Vector3.Lerp(_point1.position, _point2.position, _flightTime);
            _flightTime -= Time.deltaTime * _speed;
            if (_flightTime <= 0)
            {
                _ghost.GetComponent<SpriteRenderer>().sprite = _spriteDown;
                _moveDirection = MD.Down;
            }
        }
    }
}
