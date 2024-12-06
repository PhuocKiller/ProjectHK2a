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
        get { return "AdventurerShield\nTheShield protected from other damage\nPassive: Defend + 10"; }
    }
}