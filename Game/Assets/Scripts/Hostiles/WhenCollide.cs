using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WhenCollide : MonoBehaviour
{
    public int damage;
    public delegate void WhenCollideDelegate(Collision2D collision);
    public event WhenCollideDelegate OnPlayerCollide;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            collision.gameObject.GetComponent<PlayerController>().InflictDamage(damage);
            OnPlayerCollide?.Invoke(collision);
        }
    }
}
