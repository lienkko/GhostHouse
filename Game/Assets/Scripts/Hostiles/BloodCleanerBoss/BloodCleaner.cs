using UnityEngine;

public class BloodCleaner : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _killDistance = 1.5f;

    [Header("Обнаружение")]
    [SerializeField] private float _detectionRange = 15f;

    private bool _isActive = true;

    void Start()
    {
        FollowPlayer();
    }

    void Update()
    {
        if (!_isActive) return;

        float distance = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distance <= _killDistance)
        {
            PlayerController.Instance.InflictDamage(100);
        }

        if (distance <= _detectionRange)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
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