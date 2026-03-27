using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance { get; private set; }

    private Interactive _hideSpotInteractive;
    private Interactive _doorInteractive;
    private Interactive _safeInteractive;
    private Interactive _ghostInteractive;
    private Interactive _itemInteractive;
    private Interactive _keyClosetInteractive;
    private Interactive _signInteractive;

    private Interactive _swappingInteractive;

    [HideInInspector] public bool Hints;

    private void Awake()
    {
        Instance = this;
    }


    private bool CanHide() { return GameManager.Instance.CanUseKeyboard && _hideSpotInteractive && _hideSpotInteractive.isInteractive; }
    private bool CanOpenSafe() { return GameManager.Instance.CanUseKeyboard && _safeInteractive && _safeInteractive.isInteractive; }
    private bool CanOpedDoor() { return GameManager.Instance.CanUseKeyboard && _doorInteractive && _doorInteractive.isInteractive; }
    private bool CanStartGame() { return GameManager.Instance.CanUseKeyboard && _ghostInteractive && _ghostInteractive.isInteractive; }
    private bool CanPickUp() { return GameManager.Instance.CanUseKeyboard && _itemInteractive; }
    private bool CanTakeKey() { return GameManager.Instance.CanUseKeyboard && _keyClosetInteractive; }
    private bool CanReadSign() { return GameManager.Instance.CanUseKeyboard && _signInteractive; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && CanPickUp())
        {
            _swappingInteractive = _itemInteractive;
            if (GetComponent<Inventory>().PickUp(_itemInteractive.GetComponent<Item>()))
                _swappingInteractive.Interact();
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && CanHide())
        {
            _hideSpotInteractive.Interact();
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && CanTakeKey())
        {
            _keyClosetInteractive.Interact();
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && CanReadSign())
        {
            _signInteractive.Interact();
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


    private void OnTriggerStay2D(Collider2D collision)
    {
        var interactive = collision.GetComponent<Interactive>();
        if (interactive)
        {
            if (collision.GetComponent<Item>() && interactive.isInteractive)
            {
                _itemInteractive = interactive;
                if (Hints)
                    GameManager.Instance.GameUIFields.TakeItemText.SetActive(true);
                return;
            }
            if (collision.GetComponent<HideSpot>() && interactive.isInteractive)
            {
                _hideSpotInteractive = interactive;
                if (Hints)
                    GameManager.Instance.GameUIFields.HideText.SetActive(true);
                return;
            }
            if (collision.GetComponent<KeyCloset>() && interactive.isInteractive)
            {
                _keyClosetInteractive = interactive;
                if (Hints)
                    GameManager.Instance.GameUIFields.TakeKeyText.SetActive(true);
                return;
            }
            if (collision.GetComponent<Sign>() && interactive.isInteractive)
            {
                _signInteractive = interactive;
                if (Hints)
                    GameManager.Instance.GameUIFields.ReadSignText.SetActive(true);
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
            if (collision.GetComponent<Item>())
            {
                GameManager.Instance.GameUIFields.TakeItemText.SetActive(false);
                _itemInteractive = null;
            }
            if (collision.GetComponent<HideSpot>())
            {
                GameManager.Instance.GameUIFields.HideText.SetActive(false);
                _hideSpotInteractive = null;

            }
            if (collision.GetComponent<KeyCloset>())
            {
                GameManager.Instance.GameUIFields.TakeKeyText.SetActive(false);
                _keyClosetInteractive = null;

            }
            if (collision.GetComponent<Sign>())
            {
                GameManager.Instance.GameUIFields.ReadSignText.SetActive(false);
                _signInteractive = null;

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
