public class BigBob : InventoryItem
{
    private static readonly int _healValue = 40;
    private void Awake()
    {
        CanKeep = false;
    }
    public override void Use()
    {
        PlayerController.Instance.Heal(_healValue);
    }
}
