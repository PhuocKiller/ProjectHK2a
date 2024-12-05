using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionRegenSkill : NetworkBehaviour
{
    PlayerController player;
    private TickTimer timerToDestroy;
    float timerApplyRegen;
    TickTimer timerToApplyRegen;
    public float timerDestroy, timeEffect, timerApply;
    public int damage, levelSkill;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;

    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority && HasInputAuthority)
        {
            timerToDestroy = TickTimer.CreateFromSeconds(Runner, timerDestroy);
            timerToApplyRegen = TickTimer.CreateFromSeconds(Runner, timerApply);
        }
    }
    public void SetUp(PlayerController player, float timerApplyDamage, int levelDamage, bool isPhysicDamage, Transform parentObject = null,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float timeEffect = 0f, bool isDestroyWhenCollider = false, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        this.player = player;
        this.timerApply = timerApplyDamage;
        transform.SetParent(parentObject);
        damage = levelDamage;
        this.isPhysicDamage = isPhysicDamage;
        this.isMakeStun = isMakeStun;
        this.isMakeSlow = isMakeSlow;
        this.isMakeSilen = isMakeSilen;
        this.timeEffect = timeEffect;
        this.isDestroyWhenCollider = isDestroyWhenCollider;
        timerDestroy = timeTrigger;
        this.levelSkill = levelSkill;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!HasStateAuthority) return;
        if (timerToDestroy.Expired(Runner))
        {
            Destroy(gameObject);
        }
        if (timerToApplyRegen.Expired(Runner))
        {
            player.playerStat.currentHealth += damage;
            timerToApplyRegen = TickTimer.CreateFromSeconds(Runner, timerApply);
        }
    }

}
