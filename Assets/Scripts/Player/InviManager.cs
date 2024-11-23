using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InviManager : NetworkBehaviour
{
    public GameObject[] visuals;
    public PlayerController player;
    public override void Spawned()
    {
        base.Spawned();
        player= GetComponentInParent<PlayerController>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        CheckInviVisual(player.playerStat.isVisible);
    }

    private void CheckInviVisual(bool isVisible)
    {
        if(!HasStateAuthority) //con nào chủ thế của invi thì ko bị ảnh hưởng
        {
            foreach (var visual in visuals)
            {
                visual.gameObject.SetActive(isVisible);
            }
        }
    }
}
