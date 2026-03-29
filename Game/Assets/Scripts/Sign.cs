using UnityEngine;

[RequireComponent(typeof(IInteractive))]
public class Sign : MonoBehaviour
{
    [SerializeField] private GameObject _textSign;
    private BoxCollider2D _collider;
    public static bool IsSignOpened { get; private set; } = false;
    private void Awake()
    {
        GetComponent<IInteractive>().isInteractive = true;
        GetComponent<IInteractive>().SetListener(ShowSign);
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
    }
    private void HideSign()
    {
        _collider.enabled = true;
        IsSignOpened = false;
        _textSign.SetActive(false);
        GameManager.Instance.BlockPlayer(false);
    }
}
