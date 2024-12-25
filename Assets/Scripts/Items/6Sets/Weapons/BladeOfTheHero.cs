using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeOfTheHero : InventoryItemBase
{
    public override string Name
    {
        get { return "BladeOfTheHero"; }
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
        get { return "Blade Of The Hero\nThe relentless and undying blade of the hero.\n+15 Damage\n+3% Critical Chance\n+20% Critical Damage" +
                "\nActive Skill: Blink\nTeleport to location\nMana Cost: 10"; }
    }
}
