using UnityEngine;

public abstract class CollectableItem : MonoBehaviour, IInteractive
{
    [SerializeField] private GameObject _inventoryItemPrefab;
    public InventoryItem InventoryItem { get; private set; }
    public string HintText { get; } = "Pick up - F";
    public KeyCode KeyToInteract { get; } = KeyCode.F;
    public bool IsInteractive { get; protected set; } = true;
    public bool IsUsable { get; protected set; } = true;

    protected virtual void Awake()
    {
        InventoryItem = _inventoryItemPrefab.GetComponent<InventoryItem>();
    }
    public virtual void HideItem()
    {
        gameObject.SetActive(false);
    }

    public void Interact()
    {
        HideItem();
    }
    public bool CanInteract()
    {
        return GameManager.CanUseKeyboard && IsInteractive;
    }
    public abstract void Use();

    public virtual void ShowItem()
    {
        gameObject.SetActive(true);
    }
    public InventoryItem GetInventoryItem()
    {
        if (InventoryItem)
        {
            return InventoryItem;
        }
        return null;
    }

}
