public class MedKit : InventoryItem
{
    private static readonly int _healValue = 60;
    private void Awake()
    {
        CanKeep = false;
    }
    public override void Use()
    {
        PlayerController.Instance.Heal(_healValue);
    }
}
