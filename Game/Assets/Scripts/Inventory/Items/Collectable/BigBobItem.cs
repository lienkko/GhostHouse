using UnityEngine;

public class BigBobItem : CollectableItem
{
    private readonly int healValue = 30;
    public override void Use()
    {
        PlayerController.Instance.Heal(healValue);
    }

    protected override void Awake()
    {
        base.Awake();
        IsUsable = false;
    }
}
