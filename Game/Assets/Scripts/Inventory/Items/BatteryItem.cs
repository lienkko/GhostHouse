using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BatteryItem : Item
{
    private readonly float _procentage = 0.3f;
    public override bool UseAndDestroy()
    {
        foreach (var item in Inventory.Instance.InventoryItems)
        {
            if (item is FlashlightItem flashlight)
            {
                flashlight.FlaslightCharge += _procentage;
                return true;
            }
        }
        return false;
    }

    protected override void Awake()
    {
        base.Awake();
        IsCollectable = false;
    }


}
