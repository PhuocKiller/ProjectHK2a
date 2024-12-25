using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldStaff : InventoryItemBase
{
    public override string Name
    {
        get { return "Old Staff"; }
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
        get { return "Old Staff\nHow is this staff still working?\n+50 Max Mana\n+60% Magic Amplication\n+5 Movement Speed"; }
    }
}