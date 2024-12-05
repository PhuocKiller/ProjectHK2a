using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeshiftShield : InventoryItemBase
{
    public override string Name
    {
        get { return "MakeshiftShield"; }
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