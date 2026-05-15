public class BatteryItem : CollectableItem
{
    private readonly float _procentage = 0.3f;
    public override void Interact()
    {
        foreach (var item in Inventory.Instance.InventoryItems)
        {
            if (item is Flashlight flashlight)
            {
                flashlight.FlaslightCharge += _procentage;
                Destroy(gameObject);
                return;
            }
        }
    }

    protected override void Awake()
    {
        _isUsable = false;
    }


}
