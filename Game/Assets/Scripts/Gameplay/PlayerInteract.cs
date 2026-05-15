using TMPro;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance { get; private set; }

    private IInteractive _interactiveObj;


    [HideInInspector] public bool Hints;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (_interactiveObj != null)
        {
            if (ControlsManager.Instance.IsInteracting && _interactiveObj.CanInteract())
            {
                _interactiveObj.Interact();
                return;
            }
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractive>(out var interactive))
        {

            if ((interactive is DoorController dc) && (dc.isDoorLocked || !dc.IsInteractive))
            {
                GameManager.Instance.GameUIFields.LockedImage.SetActive(true);
            }
            else if (interactive.IsInteractive)
            {
                _interactiveObj = interactive;
                ControlsManager.Instance.ShowInteractButton(_interactiveObj.HintText);
            }

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractive>(out _))
        {
            GameManager.Instance.GameUIFields.LockedImage.SetActive(false);
            ControlsManager.Instance.HideInteractButton();
            _interactiveObj = null;
        }
    }
}
