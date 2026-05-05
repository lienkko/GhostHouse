public class BatteryItem : CollectableItem
{
    private readonly float _procentage = 0.3f;
    public override void Use()
    {
        foreach (var item in Inventory.Instance.InventoryItems)
        {
            if (item is FlashlightItem flashlight)
            {
                flashlight.FlaslightCharge += _procentage;
                return;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        IsUsable = false;
    }


}
