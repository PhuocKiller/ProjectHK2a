using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistChakram : InventoryItemBase
{
    public override string Name
    {
        get { return "CultistChakram"; }
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
