using UnityEngine;

public class HideSpot : MonoBehaviour
{
    private Transform _hidingPlayer;
    private bool _isHidingSomeone = false;
    private GameManager _gm;


    [SerializeField] private Sprite _topSideSprite;
    [SerializeField] private Sprite _botSideSprite;
    [SerializeField] private Sprite _faceSprite;
    [SerializeField] private Sprite _backSprite;



    public void Initialize()
    {
        _gm = FindAnyObjectByType<GameManager>();
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
        if (_isHidingSomeone && Input.GetKeyDown(KeyCode.Space))
            Unhide();
    }

    public void Hide(GameObject player)
    {
        _hidingPlayer = player.transform;
        player.gameObject.SetActive(false);
        _isHidingSomeone = true;
    }

    private void Unhide()
    {
        _isHidingSomeone = false;
        _hidingPlayer.gameObject.SetActive(true);
        _hidingPlayer = null;
    }
}
