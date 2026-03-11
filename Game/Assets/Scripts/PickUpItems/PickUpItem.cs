using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactive))]
public class PickUpItem : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Interactive>().isInteractive = true;
        GetComponent<Interactive>().SetListener(HideItem);
    }

    private void HideItem()
    {
        gameObject.SetActive(false);
    }
}
