using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Sign : MonoBehaviour, IInteractive
{
    [SerializeField] private GameObject _textSign;
    private BoxCollider2D _collider;
    public static bool IsSignOpened { get; private set; } = false;
    public string HintText { get; } = "Show";
    public KeyCode KeyToInteract { get; } = KeyCode.E;
    public bool IsInteractive { get; private set; } = true;
    public void Interact()
    {
        ShowSign();
    }
    public bool CanInteract()
    {
        return GameManager.CanUseKeyboard && IsInteractive;
    }
    private void Awake()
    {
        IsInteractive = true;
        _collider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if (IsSignOpened && ControlsManager.Instance.IsInteracting)
        {
            HideSign();
        }
    }
    private void ShowSign()
    {
        _collider.enabled = false;
        IsSignOpened = true;
        _textSign.SetActive(true);
        GameManager.Instance.BlockPlayer(true);
        Inventory.Instance.HideActiveItem();
        ControlsManager.Instance.HideAllControls();
        ControlsManager.Instance.ShowInteractButton("Close");
    }
    private void HideSign()
    {
        _collider.enabled = true;
        IsSignOpened = false;
        _textSign.SetActive(false);
        GameManager.Instance.BlockPlayer(false);
        Inventory.Instance.ShowActiveItem();
        ControlsManager.Instance.ShowInteractButton(HintText);
        ControlsManager.Instance.ShowJoystick();
        ControlsManager.Instance.ShowCrouchButton();
    }
}
