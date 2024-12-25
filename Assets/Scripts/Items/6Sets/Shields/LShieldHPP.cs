using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LShieldHPP : InventoryItemBase
{
    public override string Name
    {
        get { return "LShieldHPP"; }
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
        get { return "Luprus Ra's God Shield\nThat one shield the Hero's used throughout his life, was preserved and mantained well.\n+15 Armor\n+5 Magic Resistance\n+500 Max Health\n+20 Movement Speed"; }
    }
}
