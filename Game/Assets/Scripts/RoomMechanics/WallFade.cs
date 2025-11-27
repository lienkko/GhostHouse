using UnityEngine;
using UnityEngine.Tilemaps;

public class WallFade : MonoBehaviour
{
    private Tilemap _wallTilemap;

    public float FadedAlpha = 0.5f;
    public float FadeDuration = 0.5f;

    private Color _originalColor;
    private float _targetAlpha;
    private float _fadeSpeed;

    void Start()
    {
        _wallTilemap = GetComponent<Tilemap>();

        if (_wallTilemap == null)
        {
            Debug.LogError("WallFade: Скрипт должен быть прикреплен к объекту с компонентом Tilemap!");
            enabled = false;
            return;
        }

        _originalColor = _wallTilemap.color;
        _targetAlpha = _originalColor.a;

        if (FadeDuration <= 0) FadeDuration = 0.01f;
        _fadeSpeed = 1.0f / FadeDuration;
    }

    void Update()
    {
        Color currentColor = _wallTilemap.color;

        currentColor.a = Mathf.MoveTowards(
            currentColor.a,
            _targetAlpha,
            _fadeSpeed * Time.deltaTime
        );

        _wallTilemap.color = currentColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _targetAlpha = FadedAlpha;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _targetAlpha = _originalColor.a;
        }
    }
}