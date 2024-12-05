using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriestessWand : InventoryItemBase
{
    public override string Name
    {
        get { return "PriestessWand"; }
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

