using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPotion : InventoryItemBase
{
    public override string Name
    {
        get { return "HealPotion"; }
    }
    public override ItemTypes itemTypes
    {
        get { return ItemTypes.HealPotion; }
    }
    public override void OnUse()
    {
        base.OnUse();
        Singleton<Inventory>.Instance.RemoveItem(this);
       // Destroy(gameObject);
    }
}
