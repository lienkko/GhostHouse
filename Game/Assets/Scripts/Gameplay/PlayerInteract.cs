using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance {get; private set;}

    private Interactive _hideSpotInteractive;
    private Interactive _doorInteractive;
    private Interactive _safeInteractive;
    private Interactive _ghostInteractive;

    [HideInInspector] public bool CanInteract;

    [HideInInspector] public bool Hints;

    private void Awake()
    {
        Instance = this;
        CanInteract = true;
    }


    private bool CanHide() { return CanInteract && _hideSpotInteractive && _hideSpotInteractive.isInteractive;}
    private bool CanOpenSafe() { return CanInteract && _safeInteractive && _safeInteractive.isInteractive; }
    private bool CanOpedDoor() { return CanInteract && _doorInteractive &&  _doorInteractive.isInteractive; }
    private bool CanStartGame() { return CanInteract && _ghostInteractive && _ghostInteractive.isInteractive;  }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && CanHide())
        {
            _hideSpotInteractive.Interact();
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && CanOpenSafe())
        {
            _safeInteractive.Interact();
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && CanOpedDoor())
        {
            _doorInteractive.Interact();
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && CanStartGame())
        {
            _ghostInteractive.Interact();
            GameManager.Instance.GameUIFields.StartGameText.SetActive(false);
            return;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var interactive = collision.GetComponent<Interactive>();
        if (interactive)
        {
            if (collision.GetComponent<HideSpot>() && interactive.isInteractive)
            {
                _hideSpotInteractive = interactive;
                if (Hints)
                    GameManager.Instance.GameUIFields.HideText.SetActive(true);
                return;
            }
            if (collision.GetComponent<Safe>() && interactive.isInteractive)
            {
                _safeInteractive = interactive;
                if (Hints)
                    GameManager.Instance.GameUIFields.OpenSafeText.SetActive(true);
                return;
            }
            if (collision.GetComponent<DoorController>() && interactive.isInteractive)
            {                
                _doorInteractive = interactive;
                if (Hints)
                    GameManager.Instance.GameUIFields.OpenDoorText.SetActive(true);
                return;
            }
            if (collision.GetComponent<DoorController>() && !interactive.isInteractive)
            {
                GameManager.Instance.GameUIFields.LockedImage.SetActive(true);
                return;
            }
            if (collision.GetComponent<Ghost>() && interactive.isInteractive)
            {
                _ghostInteractive = interactive;
                if (Hints)
                    GameManager.Instance.GameUIFields.StartGameText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var interactive = collision.GetComponent<Interactive>();
        if (interactive)
        {
            if (collision.GetComponent<HideSpot>())
            {
                GameManager.Instance.GameUIFields.HideText.SetActive(false);
                _hideSpotInteractive = null;

            }
            if (collision.GetComponent<Safe>())
            {
                GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);
                _safeInteractive = null;

            }
            if (collision.GetComponent<DoorController>())
            {
                GameManager.Instance.GameUIFields.OpenDoorText.SetActive(false);
                GameManager.Instance.GameUIFields.LockedImage.SetActive(false);
                _doorInteractive = null;

            }
        }
        if (collision.GetComponent<Ghost>())
        {
            GameManager.Instance.GameUIFields.StartGameText.SetActive(false);
            _ghostInteractive = null;

        }
    }
}
