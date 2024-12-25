using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierArmorScript : InventoryItemBase
{
    public override string Name
    {
        get { return "SoldierArmor"; }
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
        get { return "Soldier Armor\nReliable mass produced footmen wear, this version is a little enchanted.\n+ 5 Armor\n+8 Magic Resistance\n+ 400 Max Health"; }
    }
}