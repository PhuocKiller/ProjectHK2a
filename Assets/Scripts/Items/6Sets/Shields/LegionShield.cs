using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionShield : InventoryItemBase
{
    public override string Name
    {
        get { return "LegionShield"; }
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
        get { return "Legion Shield\nThe first countermeasure against Magics.\n+5 Armor\n+10 Magic Resistance\n+200 Max Health\n+50 Max Mana"; }
    }
}