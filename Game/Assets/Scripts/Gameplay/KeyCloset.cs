using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Interactive))]
public class KeyCloset : MonoBehaviour
{
    private Light2D _keyLight;
    public void Initialize()
    {
        GetComponent<Interactive>().isInteractive = true;
        GetComponent<Interactive>().SetListener(TakeKey);
        _keyLight.enabled = true;
        _keyLight = GetComponentInChildren<Light2D>();
    }
    private void TakeKey()
    {
        PlayerController.Instance.BossSpiderKeyNumber++;
        _keyLight.enabled = false;
        GetComponent<Interactive>().isInteractive = false;
        GetComponent<Interactive>().RemoveListener();
    }
}
