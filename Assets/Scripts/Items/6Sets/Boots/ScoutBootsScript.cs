using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutBootsScript : InventoryItemBase
{
    public override string Name
    {
        get { return "ScoutBoots"; }
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
        get { return "Scout Boots\nThe scouts need something can keep up with them on the job.\n+50 Max Mana\n+5 Magic Resistance" +
                "\n+30% Magic Amplication\n+70 MovementSpeed\nActive Skill: Hasty\nBoost the MovementSpeed Of Player in short duration" +
                "\nManacost:20\nDuration: 5 seconds"; }
    }
}
