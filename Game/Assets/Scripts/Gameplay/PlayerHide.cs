using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    private HideSpot _nearestHideSpot;
    GameManager _gm;

    private void Awake()
    {
        _gm = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_nearestHideSpot && _nearestHideSpot.canHide())
            {
                _nearestHideSpot.Hide(transform);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hs = collision.gameObject.GetComponent<HideSpot>();
        if (hs && hs.canHide())
        {
            _nearestHideSpot = hs;
            _gm.OpenText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var hs = collision.gameObject.GetComponent<HideSpot>();
        if (hs && hs.enabled)
        {
            _nearestHideSpot = null;
            _gm.OpenText.gameObject.SetActive(false);
        }
    }

    
}