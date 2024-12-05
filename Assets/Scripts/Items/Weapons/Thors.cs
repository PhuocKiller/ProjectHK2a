using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thors : InventoryItemBase
{
    public override string Name
    {
        get { return "Thors"; }
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