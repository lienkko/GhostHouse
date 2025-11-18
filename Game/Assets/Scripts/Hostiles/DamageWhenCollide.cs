using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageWhenCollide : MonoBehaviour
{
    public int damage;
    public delegate void DamageWhenCollideDelegate();
    public static event DamageWhenCollideDelegate OnCollide;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            collision.gameObject.GetComponent<PlayerController>().InflictDamage(damage);
            OnCollide?.Invoke();
        }
    }
}
