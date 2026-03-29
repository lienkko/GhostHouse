using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(IInteractive))]
public class KeyCloset : MonoBehaviour
{
    private Light2D _keyLight;
    public void Initialize()
    {
        IInteractive closetInteractive = GetComponent<IInteractive>();
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
        GetComponent<IInteractive>().isInteractive = false;
        GetComponent<IInteractive>().RemoveListener();
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
