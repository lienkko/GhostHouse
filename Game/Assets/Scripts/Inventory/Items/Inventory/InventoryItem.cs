using UnityEngine;

public abstract class InventoryItem : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    private CollectableItem _collectableItem;
    public string GetName => _name;
    public Sprite GetIcon => _icon;
    public CollectableItem GetCollectableItem => _collectableItem;
    public bool CanKeep { get; protected set; } = true;

    public abstract void Use();
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public void SetCollectableItem(CollectableItem item)
    {
        if (!_collectableItem)
            _collectableItem = item;
    }
}
