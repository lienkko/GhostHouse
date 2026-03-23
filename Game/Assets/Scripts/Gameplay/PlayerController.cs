using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }


    public bool IsGodMode = false;
    [HideInInspector] public bool CanWalk = true;


    public delegate void OnDeathDelegate();
    public event OnDeathDelegate OnDeath;

    public delegate void ChangeHpDelegate(int damage, int hp);
    public event ChangeHpDelegate OnChangeHp;

    public float CurrentSpeed { get; private set; } = 4f;
    public Vector2 MoveDir { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsAlive { get; private set; } = true;
    public int HealthPoints { get; private set; } = 40;
    public float LastHorizontalVector { get; private set; }
    public Vector3 DeltaMove { get; private set; } = Vector3.zero;


    private Rigidbody2D _playerRB;
    private Vector3 _lastPos;
    private float _walkSpeedValue = 4;


    private void Awake()
    {
        Instance = this;
        _lastPos = transform.position;
        _playerRB = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        DeltaMove = transform.position - _lastPos;
        _lastPos = transform.position;
        if (CanWalk)
            InputMovement();
        if (HealthPoints == 0)
            Die();
    }

    private void FixedUpdate()
    {

        if (CanWalk)
            Move();
    }

    public float ChangeSpeed(float value)
    {
        if (value < 4 || value > 8)
            return -1;
        _walkSpeedValue = value;
        return _walkSpeedValue;

    }

    private void InputMovement()
    {

        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");
        IsCrouching = Input.GetKey(KeyCode.LeftShift);
        if (moveH != 0)
            LastHorizontalVector = moveH;
        MoveDir = new Vector2(moveH, moveV).normalized;
    }

    private void Move()
    {
        CurrentSpeed = IsCrouching ? _walkSpeedValue * 0.5f : _walkSpeedValue;
        _playerRB.velocity = MoveDir * CurrentSpeed;
    }

    public void InflictDamage(int dmg)
    {
        if (IsGodMode)
        {
            return;
        }
        if (dmg > 0)
        {
            HealthPoints -= dmg;
        }
        if (HealthPoints <= 0)
        {
            HealthPoints = 0;
        }
        OnChangeHp?.Invoke(-dmg, HealthPoints);
    }

    public void Heal(int hp)
    {
        if (IsGodMode)
        {
            return;
        }
        if (hp > 0)
        {
            HealthPoints += hp;
        }
        if (HealthPoints >= 100)
        {
            HealthPoints = 100;
        }
        OnChangeHp?.Invoke(hp, HealthPoints);

    }

    public void ReloadPlayer()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void Die()
    {
        IsAlive = false;
        OnDeath?.Invoke();

        gameObject.SetActive(false);
    }
}
