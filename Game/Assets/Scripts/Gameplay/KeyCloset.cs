using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Interactive))]
public class KeyCloset : MonoBehaviour
{
    private Light2D _keyLight;
    public void Initialize()
    {
        Interactive closetInteractive = GetComponent<Interactive>();
        closetInteractive.isInteractive = true;
        closetInteractive.SetListener(TakeKey);
        _keyLight = GetComponentInChildren<Light2D>();
        _keyLight.enabled = true;
    }
    private void TakeKey()
    {
        if (SpiderBossManager.Instance)
            SpiderBossManager.Instance.AddKey();
        _keyLight.enabled = false;
        GetComponent<Interactive>().isInteractive = false;
        GetComponent<Interactive>().RemoveListener();
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
