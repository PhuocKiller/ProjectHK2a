using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        if(player.playerStat.isLive)
        {
            CheckInviVisual(player.playerStat.isVisible);
        }
        
       // VisualOfPlayer(player.playerStat.isLive);
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
    public void VisualOfPlayer(bool isLive)
    {
        player.GetComponent<CapsuleCollider>().enabled = isLive;
        foreach (var visual in visuals)
        {
            visual.gameObject.SetActive(isLive);
        }
    }
}
