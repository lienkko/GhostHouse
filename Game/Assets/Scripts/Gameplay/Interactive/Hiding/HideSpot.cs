using System.Collections;
using UnityEngine;

public class HideSpot : MonoBehaviour, IInteractive
{
    private bool _isHidingSomeone = false;
    private Vector3 _unhidePos;
    public KeyCode KeyToInteract { get; } = KeyCode.E;
    public string HintText { get; } = "Hide - E";
    public bool IsInteractive { get; protected set; } = false;
    public void Interact()
    {
        Hide();
    }
    public bool CanInteract()
    {
        return GameManager.CanUseKeyboard && IsInteractive;
    }
    private void Update()
    {
        if (!Pause.IsPaused && _isHidingSomeone && Input.GetKeyDown(KeyCode.E) && !FindAnyObjectByType<CommandLine>())
            Unhide();
    }

    public void Hide()
    {
        _unhidePos = PlayerController.Instance.transform.position;
        PlayerController.Instance.transform.position = transform.position;
        RoomsManager.Instance.CurrentRoom.transform.Find("Lights").gameObject.SetActive(false);
        PlayerController.Instance.gameObject.SetActive(false);
        StartCoroutine(SwitchIsHidingSomeone(true));
        GameManager.Instance.BlockPlayer(true);
        Inventory.Instance.HideActiveItem();
    }

    private void Unhide()
    {
        StartCoroutine(SwitchIsHidingSomeone(false));
        PlayerController.Instance.transform.position = _unhidePos;
        RoomsManager.Instance.CurrentRoom.transform.Find("Lights").gameObject.SetActive(true);
        PlayerController.Instance.transform.gameObject.SetActive(true);
        GameManager.Instance.BlockPlayer(false);
        Inventory.Instance.ShowActiveItem();
    }
    public virtual void Initialize() { }
    private IEnumerator SwitchIsHidingSomeone(bool state)
    {
        yield return null;
        _isHidingSomeone = state;
    }
}
