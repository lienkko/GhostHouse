using UnityEngine;

public class Interactive : MonoBehaviour
{
    public delegate void InteractDelegate(GameObject playerObj);
    private event InteractDelegate OnInteracted;
    [HideInInspector] public bool isInteractive = false;

    public void Interact(GameObject playerObj)
    {
        if (isInteractive)
        {
            OnInteracted?.Invoke(playerObj);
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
