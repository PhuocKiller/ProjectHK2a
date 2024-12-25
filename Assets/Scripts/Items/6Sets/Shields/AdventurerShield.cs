using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerShield : InventoryItemBase
{
    public override string Name
    {
        get { return "AdventurerShield"; }
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
        get { return "Adventurer Shield\nA light shield to start your journey.\n+100 Max Health\n+5 Armor" +
                "\nActive Skill: AdventurerShield\nCreate the shield that can block 200 damage from enemies" +
                "\nMana Cost:20\nDuration: 10 seconds"; }
    }
}