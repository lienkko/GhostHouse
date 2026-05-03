using UnityEngine;

public class BloodCleaner : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float _speed = 2.5f;
    [SerializeField] private float _killDistance = 1f;

    [Header("Обнаружение")]
    [SerializeField] private float _detectionRange = 30f;

    private bool _isActive = true;
    public bool IsWalking { get; private set; }
    public Vector3 DeltaMove { get; private set; } = Vector3.zero;

    void Start()
    {
        FollowPlayer();
    }

    void Update()
    {
        if (!_isActive) return;

        // Ничего не делает, пока открыта табличка с правилами
        if (Sign.IsSignOpened) return;

        float distance = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distance <= _killDistance)
        {
            GetComponent<Animator>().SetBool("IsAttacking", true);
            PlayerController.Instance.InflictDamage(100);
        }

        if (distance <= _detectionRange)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        IsWalking = true;
        Vector2 direction = (PlayerController.Instance.transform.position - transform.position).normalized;

        Vector2 newPosition = (Vector2)transform.position + direction * _speed * Time.deltaTime;
        transform.position = newPosition;
    }

    public void Activate()
    {
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
        Destroy(gameObject, 0.5f);
    }
}