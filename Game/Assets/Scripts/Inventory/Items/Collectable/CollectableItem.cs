using UnityEngine;

public class CollectableItem : MonoBehaviour, IInteractive
{
    [SerializeField] private GameObject _inventoryItemPrefab;
    private InventoryItem _inventoryItem;
    public InventoryItem GetInventoryItem => _inventoryItem;
    public string HintText { get; } = "Pick up - F";
    public KeyCode KeyToInteract { get; } = KeyCode.F;
    public bool IsInteractive { get; protected set; } = true;
    protected bool _isUsable = true;

    protected virtual void Awake()
    {
        _inventoryItem = Instantiate(_inventoryItemPrefab, transform.position, Quaternion.identity, RoomsManager.Instance.CurrentRoom.transform).GetComponent<InventoryItem>();
        _inventoryItem.SetCollectableItem(this);
    }
    public virtual void HideItem()
    {
        gameObject.SetActive(false);
    }

    public virtual void Interact()
    {
        if (Inventory.Instance.PickUp(_inventoryItem, _isUsable))
            HideItem();
    }
    public bool CanInteract()
    {
        return GameManager.CanUseKeyboard && IsInteractive;
    }

    public virtual void ShowItem()
    {
        gameObject.SetActive(true);
    }

}
