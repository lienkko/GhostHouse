using TMPro;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance { get; private set; }

    private Interactive _interactiveObj;

    private Interactive _swappingInteractive;

    [HideInInspector] public bool Hints;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (_interactiveObj)
        {
            if (Input.GetKeyDown(_interactiveObj.KeyToInteract))
            {
                _interactiveObj.Interact();
                return;
            }
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        var interactive = collision.GetComponent<Interactive>();
        if (interactive && interactive.IsInteractive)
        {
            _interactiveObj = interactive;
            if (Hints)
            {
                GameManager.Instance.GameUIFields.OpenSafeText.SetActive(true);
                GameManager.Instance.GameUIFields.OpenSafeText.GetComponent<TextMeshPro>().text = _interactiveObj.HintText;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var interactive = collision.GetComponent<Interactive>();
        if (interactive)
        {
            GameManager.Instance.GameUIFields.OpenSafeText.SetActive(false);
            GameManager.Instance.GameUIFields.OpenSafeText.GetComponent<TextMeshPro>().text = "Interact";
            _interactiveObj = null;
        }
    }
}
