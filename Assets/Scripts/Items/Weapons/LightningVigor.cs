using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningVigor : InventoryItemBase
{
    public override string Name
    {
        get { return "LightningVigor"; }
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
