using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeisArmorScript : InventoryItemBase
{
    public override string Name
    {
        get { return "AgeisArmor"; }
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
        get { return "Ageis Armor\nMade from the body of the Ageis itself!.\n+100 Max Health\n+5 Magic Resist"; }
    }
}