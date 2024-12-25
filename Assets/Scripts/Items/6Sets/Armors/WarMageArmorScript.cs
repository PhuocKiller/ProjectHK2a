using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarMageArmorScript : InventoryItemBase
{
    public override string Name
    {
        get { return "WarMageArmor"; }
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
        get { return "WarMage Armor\nWhen magicians become too much of an issue, units with this armor will be deployed..\n+10 Armor\n+7 Magic Resistance\n+450 Max Health" +
                "\nActive Skill: Invicibility\nThe Player can be invisible to enemy\nMana cost: 15\n Duration: 10 seconds"; }
    }
}
