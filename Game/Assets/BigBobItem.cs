using UnityEngine;

public class BigBobItem : Item
{
    private readonly int healValue = 40;
    public override bool UseAndDestroy()
    {
        PlayerController.Instance.Heal(healValue);
        return true;
    }
}
