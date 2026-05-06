public interface IChargeableItem
{
    public InventoryItem ItemObj { get; }
    public float CurrentChargeNormalized { get; }
}
