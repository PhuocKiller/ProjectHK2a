using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixedBow : InventoryItemBase
{
    public override string Name
    {
        get { return "MixedBow"; }
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