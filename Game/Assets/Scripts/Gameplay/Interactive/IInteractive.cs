using UnityEngine;

public interface IInteractive
{
    public KeyCode KeyToInteract { get; }
    public bool IsInteractive { get; }
    public string HintText { get; }

    public void Interact();
    public bool CanInteract();
}
