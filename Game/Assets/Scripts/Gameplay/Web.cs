using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Web : MonoBehaviour
{
    [SerializeField] private float webSpeed = 1.1f;
    [SerializeField] private float normalSpeed = 4f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (!player) return;

        if (TryBurn(player)) return;

        player.ChangeSpeed(webSpeed);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (!player) return;

        TryBurn(player);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (!player) return;

        player.ChangeSpeed(normalSpeed);
    }

    private bool TryBurn(PlayerController player)
    {
        if (PlayerHand.Instance == null) return false;

        var candle = PlayerHand.Instance.ActiveItem as CandleItem;
        if (candle == null) return false;

        var light = candle.GetComponent<Light2D>();
        if (light == null || !light.enabled) return false;

        player.ChangeSpeed(normalSpeed);
        Destroy(gameObject);
        return true;
    }
}