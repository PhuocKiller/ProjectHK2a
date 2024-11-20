using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Sagittarius : PlayerController
{
    public string playerLayer = "Player";

    TickTimer timerSkill2;

    public override void Spawned()
    { base.Spawned(); }

    public override void NormalAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    {

        base.NormalAttack(VFXEffect, levelDamage, isPhysicDamage, timeTrigger: timeTrigger);
        
        Runner.Spawn(VFXEffect.gameObject, normalAttackTransform.transform.position, normalAttackTransform.rotation, inputAuthority: Object.InputAuthority
     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
     {
         obj.GetComponent<AttackObjects>().SetUp(this, playerStat.b_damage, isPhysicDamage, null,
              isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
         
         obj.GetComponent<AttackObjects>().SetDirection(transform.forward);
     });
    }
  
    public override void Skill_1(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_1(VFXEffect, levelDamage, manaCost, isPhysicDamage, isMakeStun: isMakeStun,
            TimeEffect: TimeEffect, timeTrigger: timeTrigger);
        if(CheckHitPlayer())
        {
            CheckHitPlayer().GetComponent<PlayerController>().SkillRPC
         (0, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
        }
        else
        {
            SkillRPC(0, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
        }
    }

    Transform CheckHitPlayer()
    {
        int layer = LayerMask.NameToLayer(playerLayer);
        LayerMask layerMask = 1 << layer;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
       Physics.Raycast(rayCastTransform.position,transform.forward,out RaycastHit hit, 500, 1<<layer);
        return hit.transform;
    }
    
    public override void Skill_2(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_2(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        if (CheckHitPlayer())
        {
            CheckHitPlayer().GetComponent<PlayerController>().SkillRPC
         (1, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, levelSkill);
        }
        else
        {
            SkillRPC(1,levelDamage,manaCost,isPhysicDamage,isMakeStun,isMakeSlow, isMakeSilen,timeTrigger, TimeEffect, levelSkill);
        }
    }
    public override void Ultimate(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Ultimate(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        StartCoroutine(DelayUltimate(VFXEffect, levelDamage, isPhysicDamage,
            isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, posMouseUp, levelSkill));


    }
    IEnumerator DelayUltimate(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        yield return new WaitForSeconds(0f);
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, posMouseUp, Quaternion.identity, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<EnvironmentObjects>().SetUp(this,playerTeam, 0.05f, levelDamage, isPhysicDamage, null,
                 isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
                obj.GetComponent<BuffsOfPlayer>().levelSkill = levelSkill;
            });
        
    }
    
}

