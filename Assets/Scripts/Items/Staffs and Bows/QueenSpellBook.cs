using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenSpellBook : InventoryItemBase
{
    public override string Name
    {
        get { return "QueenSpellbook"; }
    }
    public override ItemTypes itemTypes
    {
        get { return ItemTypes.ActiveSkill; }
    }
    public override void OnUse(int indexSlot)
    {
        base.OnUse(indexSlot);
    }
}

