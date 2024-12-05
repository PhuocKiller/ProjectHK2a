using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWarglaives : InventoryItemBase
{
    public override string Name
    {
        get { return "SWarglaives"; }
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

