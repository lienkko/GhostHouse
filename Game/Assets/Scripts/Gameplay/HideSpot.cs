using UnityEngine;

public class HideSpot : MonoBehaviour
{
    private Transform _hidingPlayer;
    private bool _isHidingSomeone;

    public bool canHide()
    {
        if (_isHidingSomeone)
            return false;
        return true;
    }

    private void Update()
    {
        if (_isHidingSomeone && Input.GetKeyDown(KeyCode.E))
            Unhide();
    }

    public void Hide(Transform player)
    {
        _isHidingSomeone = true;
        _hidingPlayer = player;
        player.gameObject.SetActive(false);
    }

    private void Unhide()
    {
        _isHidingSomeone = false;
        _hidingPlayer.gameObject.SetActive(true);
        _hidingPlayer = null;
    }
}
