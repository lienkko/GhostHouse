using UnityEngine;

[RequireComponent(typeof(Interactive))]
public class Item : MonoBehaviour
{
    [SerializeField] private string Name;
    [SerializeField] private Sprite Icon;
    public bool IsCollectable { get; protected set; } = true;
    protected virtual void Awake()
    {
        GetComponent<Interactive>().isInteractive = true;
        GetComponent<Interactive>().SetListener(Hide);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
    public virtual void Unhide()
    {
        gameObject.SetActive(true);
    }
    public virtual bool UseAndDestroy()
    {
        print("Item's base Use method");
        return false;
    }
    public string GetName() { return Name; }
    public Sprite GetIcon() { return Icon; }
}
