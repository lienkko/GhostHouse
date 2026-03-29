using Unity.VisualScripting;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    public virtual KeyCode KeyToInteract { get; }
    public bool IsInteractive { get; protected set; }
    public GameObject HintField { get; }

    public abstract void Interact();
    bool CanInteract();
}
