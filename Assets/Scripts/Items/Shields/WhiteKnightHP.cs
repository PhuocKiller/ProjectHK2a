using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteKnightHP : InventoryItemBase
{
    public override string Name
    {
        get { return "WhiteKnight(HP)"; }
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
