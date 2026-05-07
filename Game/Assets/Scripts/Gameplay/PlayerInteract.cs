using TMPro;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance { get; private set; }

    private IInteractive _interactiveObj;

    private IInteractive _swappingInteractive;

    [HideInInspector] public bool Hints;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (_interactiveObj != null)
        {
            if (Input.GetKeyDown(_interactiveObj.KeyToInteract) && _interactiveObj.CanInteract())
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
                if (Hints)
                {
                    GameManager.Instance.GameUIFields.HintFieldText.SetActive(true);
                    GameManager.Instance.GameUIFields.HintFieldText.GetComponent<TextMeshProUGUI>().text = _interactiveObj.HintText;
                }
            }

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractive>(out _))
        {
            GameManager.Instance.GameUIFields.HintFieldText.SetActive(false);
            GameManager.Instance.GameUIFields.HintFieldText.GetComponent<TextMeshProUGUI>().text = "Interact";
            GameManager.Instance.GameUIFields.LockedImage.SetActive(false);
            _interactiveObj = null;
        }
    }
}
