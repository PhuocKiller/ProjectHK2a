using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeatherArmorScript : InventoryItemBase
{
    public override string Name
    {
        get { return "LeatherArmor"; }
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
        get { return "Leather Armor\nBasic and reliable light armor.\n+ 5 Armor\n+5 Magic Resistance\n+ 300 Max Health"; }
    }
}
