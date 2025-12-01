using UnityEngine;

public class Interactive : MonoBehaviour
{
    public delegate void InteractDelegate();
    private event InteractDelegate OnInteracted;
    [HideInInspector] public bool isInteractive = false;

    public void Interact()
    {
        if (isInteractive)
        {
            OnInteracted?.Invoke();
            return;
        }
    }

    public void SetListener(InteractDelegate listener)
    {
        OnInteracted += listener;
    }

    public void RemoveListener()
    {
        OnInteracted = null;
    }
}
