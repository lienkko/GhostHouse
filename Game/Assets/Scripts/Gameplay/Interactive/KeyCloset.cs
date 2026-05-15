using UnityEngine;
using UnityEngine.Rendering.Universal;

public class KeyCloset : MonoBehaviour, IInteractive
{
    private Light2D _keyLight;
    public KeyCode KeyToInteract { get; } = KeyCode.E;
    public string HintText { get; } = "Take";
    public bool IsInteractive { get; private set; } = false;
    public void Interact()
    {
        TakeKey();
    }
    public bool CanInteract()
    {
        return GameManager.CanUseKeyboard && IsInteractive;
    }
    public void Initialize()
    {
        IsInteractive = true;
        _keyLight = GetComponentInChildren<Light2D>();
        _keyLight.enabled = true;
    }
    private void TakeKey()
    {
        if (SpiderBossManager.Instance)
            SpiderBossManager.Instance.AddKey();
        _keyLight.enabled = false;
        IsInteractive = false;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
