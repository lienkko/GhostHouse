using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Sign : MonoBehaviour, IInteractive
{
    [SerializeField] private GameObject _textSign;
    private BoxCollider2D _collider;
    public static bool IsSignOpened { get; private set; } = false;
    public string HintText { get; } = "Show sign = E";
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
        if (IsSignOpened && !Pause.IsPaused && Input.GetKeyDown(KeyCode.Escape))
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
    }
    private void HideSign()
    {
        _collider.enabled = true;
        IsSignOpened = false;
        _textSign.SetActive(false);
        GameManager.Instance.BlockPlayer(false);
        Inventory.Instance.ShowActiveItem();
    }
}
