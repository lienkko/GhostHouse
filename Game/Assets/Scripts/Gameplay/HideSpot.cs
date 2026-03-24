using System.Collections;
using UnityEngine;

public class HideSpot : MonoBehaviour
{
    private bool _isHidingSomeone = false;
    private Vector3 _unhidePos;
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
        if (PlayerHand.Instance.ActiveItem)
        {
            PlayerHand.Instance.ActiveItem.Hide();
        }
    }

    private void Unhide()
    {
        StartCoroutine(SwitchIsHidingSomeone(false));
        PlayerController.Instance.transform.position = _unhidePos;
        RoomsManager.Instance.CurrentRoom.transform.Find("Lights").gameObject.SetActive(true);
        PlayerController.Instance.transform.gameObject.SetActive(true);
        GameManager.Instance.BlockPlayer(false);
        if (PlayerHand.Instance.ActiveItem)
        {
            PlayerHand.Instance.ActiveItem.Unhide();
        }
    }
    public virtual void Initialize() { }
    private IEnumerator SwitchIsHidingSomeone(bool state)
    {
        yield return null;
        _isHidingSomeone = state;
    }
}
