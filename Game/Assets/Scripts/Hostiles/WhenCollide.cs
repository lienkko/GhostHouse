using UnityEngine;

public class WhenCollide : MonoBehaviour
{
    public int damage;
    public delegate void WhenCollideDelegate(Collider2D collider);
    public event WhenCollideDelegate OnPlayerCollide;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<PlayerController>())
        {
            collider.gameObject.GetComponent<PlayerController>().InflictDamage(damage);
            OnPlayerCollide?.Invoke(collider);
        }
    }
}
