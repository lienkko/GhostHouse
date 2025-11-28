using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool _isSneaking;
    private bool _isCrouching;
    private Rigidbody2D _playerRB;
    private Vector2 _moveDir;
    private int _healthPoints;

    public float MoveSpeed;
    public Vector2 MoveDir { get => _moveDir; }
    public bool IsCrouching { get => _isCrouching; }

    public delegate void NoArgs();
    public static event NoArgs OnDeath;

    public delegate void DamageDelegate(int damage,int hp);
    public static event DamageDelegate OnDamage;


    
    
    
    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        MoveSpeed = 5f;
        _healthPoints = 100;
    }
    private void Update()
    {
        InputMovement();
        if (_healthPoints == 0)
            Die();
    }

    private void FixedUpdate()
    {
        Move(); 
    }

    private void InputMovement()
    {

        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");
        _isCrouching = Input.GetKey(KeyCode.C);
        _isSneaking = Input.GetKey(KeyCode.LeftShift);


        _moveDir = new Vector2(moveH, moveV).normalized;
    }

    private void Move()
    {
        if (_isCrouching)
            MoveSpeed = 1.5f;
        else if (_isSneaking)
            MoveSpeed = 2f;
        else
            MoveSpeed = 5f;
        _playerRB.velocity = _moveDir * MoveSpeed;
    }

    public void InflictDamage(int dmg)
    {
        if (dmg > 0)
        {
            _healthPoints -= dmg;
        }
        if (_healthPoints <= 0)
        {
            _healthPoints = 0;
        }
        OnDamage?.Invoke(dmg, _healthPoints);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }
}
