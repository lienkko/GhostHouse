using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BloodPuddle : MonoBehaviour // Лужицы крови, замедляющие игрока
{
    [Header("Параметры замедления")]
    [SerializeField] private float _slowSpeed = 2f;      // Скорость в луже
    [SerializeField] private bool _resetOnExit = true;

    [Header("Звук шагов")]
    [SerializeField] private AudioClip _splashSound;      // Звук шагов по крови

    private BoxCollider2D _collider;
    private bool _isPlayerInside = false;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        _isPlayerInside = true;

        // Замедление игрока
        player.ChangeSpeed(_slowSpeed);

        // Эффект входа в лужу
        PlaySplashEffect(other.transform.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        _isPlayerInside = false;

        // Восстановление скорости при выходе из лужицы
        if (_resetOnExit)
        {
            player.ReturnSpeedToNormal();
        }
    }

    private void PlaySplashEffect(Vector3 position)
    {
        // Проигрывание звука шагов по крови
        if (_splashSound != null)
        {
            AudioSource.PlayClipAtPoint(_splashSound, position, 0.5f);
        }
    }
}
