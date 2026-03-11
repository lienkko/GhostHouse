using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactive))]
public class Item : MonoBehaviour
{
    [SerializeField] private string Name;
    [SerializeField] private Sprite Icon;
    private void Awake()
    {
        GetComponent<Interactive>().isInteractive = true;
        GetComponent<Interactive>().SetListener(HideItem);
    }

    private void HideItem()
    {
        gameObject.SetActive(false);
    }

    public string GetName() { return Name; }
    public Sprite GetIcon() { return Icon; }
}
