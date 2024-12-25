using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteKnightShield : InventoryItemBase
{
    public override string Name
    {
        get { return "EliteKnightShield"; }
    }
    public override ItemTypes itemTypes
    {
        get { return ItemTypes.ActiveSkill; }
    }
    public override void OnUse(int indexSlot)
    {
        base.OnUse(indexSlot);
    }
public override string Info
{
    get { return "Elite Knight Shield\nA shield only the seasoned veterans would be equipped with.\n+10 Armor\n+350 Max Health"; }
}
}
