using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private GameManager _gm;
    private Interactive _hideSpotInteractive;
    private Interactive _doorInteractive;
    private Interactive _safeInteractive;
    private bool _canInteract = true;

    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    private void Awake()
    {
        _gm = FindAnyObjectByType<GameManager>();
    }

    private bool CanHide() { return _canInteract && _hideSpotInteractive && _hideSpotInteractive.isInteractive;}
    private bool CanOpenSafe() { return _canInteract && _safeInteractive && _safeInteractive.isInteractive; }
    private bool CanOpedDoor() { return _canInteract && _doorInteractive &&  _doorInteractive.isInteractive; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && CanHide())
        {
            _hideSpotInteractive.Interact(gameObject);
            return;
        }
        if (Input.GetKeyDown(KeyCode.X) && CanOpenSafe())
        {
            _safeInteractive.Interact(gameObject);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && CanOpedDoor())
        {
            _doorInteractive.Interact(gameObject);
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var interactive = collision.GetComponent<Interactive>();
        if (interactive && interactive.isInteractive)
        {
            if (collision.GetComponent<DoorController>())
            {
                _doorInteractive = interactive;
                if (_doorInteractive.isInteractive)
                    _gm.OpenDoorText.gameObject.SetActive(true);
                else
                    _gm.LockedText.gameObject.SetActive(true);
            }
            if (collision.GetComponent<Safe>())
            {

                _gm.OpenDoorText.gameObject.SetActive(false);
                _gm.OpenSafeText.gameObject.SetActive(true);
                _safeInteractive = interactive;
            }
            if (collision.GetComponent<HideSpot>())
            {
                _gm.OpenDoorText.gameObject.SetActive(false);
                _gm.OpenSafeText.gameObject.SetActive(false);
                _gm.HideText.gameObject.SetActive(true);
                _hideSpotInteractive = interactive;
            }
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<HideSpot>())
        {
            _gm.HideText.gameObject.SetActive(false);
            _hideSpotInteractive = null;
            if(_safeInteractive)
            {
                _gm.OpenSafeText.gameObject.SetActive(true);
            }
            else if(_doorInteractive)
            {
                _gm.HideText.gameObject.SetActive(true);
            }
        }
        if(collision.GetComponent<Safe>())
        {
            _gm.OpenSafeText.gameObject.SetActive(false);
            _safeInteractive = null;
            if (_doorInteractive)
            {
                _gm.HideText.gameObject.SetActive(true);
            }
        }
        if (collision.GetComponent<DoorController>())
        {
            _gm.OpenDoorText.gameObject.SetActive(false);
            _doorInteractive = null;
        }
    }
}
