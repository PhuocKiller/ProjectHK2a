using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysShieldPHY : InventoryItemBase
{
    public override string Name
    {
        get { return "PhysShield (PHYS)"; }
    }
    public override ItemTypes itemTypes
    {
        get { return ItemTypes.ActiveSkill; }
    }
    public override void OnUse(int indexSlot)
    {
        base.OnUse(indexSlot);
    }
}
