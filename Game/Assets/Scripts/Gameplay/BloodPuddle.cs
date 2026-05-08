using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BloodPuddle : MonoBehaviour // ������ �����, ����������� ������
{
    [Header("��������� ����������")]
    [SerializeField] private float _slowSpeed = 2f;      // �������� � ����
    [SerializeField] private bool _resetOnExit = true;

    [Header("���� �����")]
    [SerializeField] private AudioClip _splashSound;      // ���� ����� �� �����

    private BoxCollider2D _collider;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;


        // ���������� ������
        player.ChangeSpeed(_slowSpeed);

        // ������ ����� � ����
        PlaySplashEffect(other.transform.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;


        // �������������� �������� ��� ������ �� ������
        if (_resetOnExit)
        {
            player.ReturnSpeedToNormal();
        }
    }

    private void PlaySplashEffect(Vector3 position)
    {
        // ������������ ����� ����� �� �����
        if (_splashSound != null)
        {
            AudioSource.PlayClipAtPoint(_splashSound, position, 0.5f);
        }
    }
}
