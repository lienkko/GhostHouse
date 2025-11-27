using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    private HideSpot _nearestHideSpot;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_nearestHideSpot && _nearestHideSpot.canHide())
            {
                _nearestHideSpot.Hide(transform);
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        var hs = collision.gameObject.GetComponent<HideSpot>();
        if (hs)
            _nearestHideSpot = collision.gameObject.GetComponent<HideSpot>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _nearestHideSpot = null;
    }

    
}