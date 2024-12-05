using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteKnight : InventoryItemBase
{
    public override string Name
    {
        get { return "WhiteKnight"; }
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
