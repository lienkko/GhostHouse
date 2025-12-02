using System.Collections;
using UnityEngine;

public class HideSpot : MonoBehaviour
{
    private bool _isHidingSomeone = false;
    private Vector3 _unhidePos;

    [SerializeField] private Sprite _topSideSprite;
    [SerializeField] private Sprite _botSideSprite;
    [SerializeField] private Sprite _faceSprite;
    [SerializeField] private Sprite _backSprite;



    public void Initialize()
    {
        GetComponent<Interactive>().isInteractive = true;
        GetComponent<Interactive>().SetListener(Hide);
        switch (tag)
        {
            case "TopLeftPoint":
                GetComponent<SpriteRenderer>().sprite = _topSideSprite;
                transform.position += new Vector3(0.05f, 0, 0);
                break;
            case "TopRightPoint":
                GetComponent<SpriteRenderer>().sprite = _topSideSprite;
                GetComponent<SpriteRenderer>().flipX = true;
                transform.position += new Vector3(-0.05f, 0, 0);
                break;
            case "BotLeftPoint":
                GetComponent<SpriteRenderer>().sprite = _botSideSprite;
                transform.position += new Vector3(0.05f,0,0);
                break;
            case "BotRightPoint":
                GetComponent<SpriteRenderer>().sprite = _botSideSprite;
                GetComponent<SpriteRenderer>().flipX = true;
                transform.position += new Vector3(-0.05f, 0, 0);
                break;
            case "TopPoint":
                GetComponent<SpriteRenderer>().sprite = _faceSprite; break;
            case "BotPoint":
                GetComponent<SpriteRenderer>().sprite = _backSprite;
                transform.position += new Vector3(0, 0.05f, 0);
                break;
        }
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
    }

    private void Unhide()
    {
        StartCoroutine(SwitchIsHidingSomeone(false));
        PlayerController.Instance.transform.position = _unhidePos;
        RoomsManager.Instance.CurrentRoom.transform.Find("Lights").gameObject.SetActive(true);
        PlayerController.Instance.transform.gameObject.SetActive(true);
    }

    private IEnumerator SwitchIsHidingSomeone(bool state)
    {
        yield return null;
        _isHidingSomeone = state;
    }
}
