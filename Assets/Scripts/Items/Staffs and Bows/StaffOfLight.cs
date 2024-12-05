using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffOfLight : InventoryItemBase
{
    public override string Name
    {
        get { return "StaffOfLight"; }
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
