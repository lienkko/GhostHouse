using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public float MoveSpeed = 5f;
    public bool IsGodMode = false;
    [HideInInspector] public bool CanWalk = true;


    public delegate void OnDeathDelegate();
    public event OnDeathDelegate OnDeath;

    public delegate void DamageDelegate(int damage, int hp);
    public event DamageDelegate OnDamage;

    public Vector2 MoveDir { get; private set; }
    public bool IsCrouching { get; private set; }
    public int HealthPoints { get; private set; } = 100;
    

    private Rigidbody2D _playerRB;
    

    private void Awake()
    {
        Instance = this;

        _playerRB = GetComponent<Rigidbody2D>();
    }






    private void Update()
    {
        if(CanWalk)
            InputMovement();
        if (HealthPoints == 0)
            Die();
    }

    private void FixedUpdate()
    {
        if (CanWalk)
            Move(); 
    }

    private void InputMovement()
    {

        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");
        IsCrouching = Input.GetKey(KeyCode.LeftShift);

        MoveDir = new Vector2(moveH, moveV).normalized;
    }

    private void Move()
    {
        if (IsCrouching)
            MoveSpeed = 1.5f;
        else
            MoveSpeed = 5f;
        _playerRB.velocity = MoveDir * MoveSpeed;
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
        OnDamage?.Invoke(dmg, HealthPoints);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }
}
