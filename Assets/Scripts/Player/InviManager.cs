using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InviManager : NetworkBehaviour
{
    public GameObject[] visuals;
    public PlayerController player;
    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    public MeshRenderer[] meshRenderers;
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
            // CheckInviVisual(player.playerStat.isVisible);
            //ControlInvi();
            if(player.playerStat.isStartFadeInvi)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    Debug.Log("meshRenderers[i].material.color.a" + meshRenderers[i].material.color.a);
                    ControlMaterial(3, meshRenderers[i].material, meshRenderers[i].material.color.a - 0.4f * Runner.DeltaTime);
                }
                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    ControlMaterial(3, skinnedMeshRenderers[i].material, skinnedMeshRenderers[i].material.color.a - 0.4f * Runner.DeltaTime);
                                   }
                if (meshRenderers[0].material.color.a < 0.4f || skinnedMeshRenderers[0].material.color.a < 0.4f)
                {
                    player.playerStat.isVisible = false;
                    player.playerStat.isStartFadeInvi = false;
                }
                
            }
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
    void ControlMaterial(int modeRender, Material material, float alpha)
    {
        material.SetFloat("_Mode", 3);

        material.color = new Color(material.color.r, material.color.g, material.color.b,alpha);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}
