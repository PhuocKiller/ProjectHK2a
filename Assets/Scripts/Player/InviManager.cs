using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InviManager : NetworkBehaviour
{
    public GameObject[] visuals;
    public PlayerController player;
    public SkinnedMeshRenderer[] skinnedMeshRenderer;
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
        CharacterControllerActiveRPC(isLive);
        foreach (var visual in visuals)
        {
            visual.gameObject.SetActive(isLive);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)] public void CharacterControllerActiveRPC(bool isLive)
    {
        player.GetComponent<CharacterController>().enabled = isLive;
    }
    void ControlInvi()
    {
        //skinnedMeshRenderer[0].material.red
    }
}
