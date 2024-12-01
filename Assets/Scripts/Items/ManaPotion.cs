using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : InventoryItemBase
{
    public override string Name
    {
        get { return "ManaPotion"; }
    }
    public override ItemTypes itemTypes
    {
        get { return ItemTypes.Consumable; }
    }
    public override void OnUse()
    {
        base.OnUse();
        Singleton<Inventory>.Instance.RemoveItem(this);
    }
    public override string Info
    {
        get { return "The Potion will recover the Mana of player"; }
    }
}
