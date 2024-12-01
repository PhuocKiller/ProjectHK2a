
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcticReaper : InventoryItemBase
{
    public override string Name
    {
        get { return "ArcticReaper"; }
    }
    public override ItemTypes itemTypes
    {
        get { return ItemTypes.ActiveSkill; }
    }

    public override void OnUse()
    {
        base.OnUse();
    }
}

