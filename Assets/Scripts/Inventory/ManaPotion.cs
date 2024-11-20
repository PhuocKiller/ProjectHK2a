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
        get { return ItemTypes.ManaPotion; }
    }
    public override void OnUse()
    {
        base.OnUse();
    }

}
