using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(BoxCollider2D))]
public class Web : MonoBehaviour
{
    private readonly float webSpeed = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (!player) return;
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.offset = new Vector2(0, 0);
        boxCollider.size = new Vector2(1.8f, 1.8f);
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

        player.ReturnSpeedToNormal();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.offset = new Vector2(0, 0);
        boxCollider.size = new Vector2(0.7f, 0.7f);
    }

    private bool TryBurn(PlayerController player)
    {
        if (PlayerHand.Instance == null) return false;

        var candle = PlayerHand.Instance.ActiveItem as CandleItem;
        if (candle == null) return false;

        var light = candle.GetComponent<Light2D>();
        if (light == null || !light.enabled) return false;

        player.ReturnSpeedToNormal();
        Destroy(gameObject);
        return true;
    }
}