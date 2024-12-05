using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBow : InventoryItemBase
{
    public override string Name
    {
        get { return "MagicBow"; }
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