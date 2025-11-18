using UnityEngine;

public class HideSpot : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHide playerHide = collision.gameObject.GetComponent<PlayerHide>();
            if (playerHide != null && !playerHide.isHidden)
            {
                playerHide.currentHideSpot = this;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHide playerHide = collision.gameObject.GetComponent<PlayerHide>();
            if (playerHide != null && !playerHide.isHidden)
            {
                playerHide.currentHideSpot = null;
            }
        }
    }
}