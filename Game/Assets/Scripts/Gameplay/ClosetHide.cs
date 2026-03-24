using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetHide : HideSpot
{
    [SerializeField] private Sprite _topSideSprite;
    [SerializeField] private Sprite _botSideSprite;
    [SerializeField] private Sprite _faceSprite;
    [SerializeField] private Sprite _backSprite;
    public override void Initialize()
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
                transform.position += new Vector3(0.05f, 0, 0);
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
}
