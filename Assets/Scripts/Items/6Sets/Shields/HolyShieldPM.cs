using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyShieldPM : InventoryItemBase
{
    public override string Name
    {
        get { return "HolyShield"; }
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
        get { return "Holy Shield\nCrafted exclusively just for the Royal Knights, don't ask how I got them..\n+5 Magic Resistance\n+200 Max Health\n+30 Max Mana"; }
    }
}
