using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObjects : NetworkBehaviour
{
    PlayerController player;
    CalculateTriggerEnter trigger;
    private Vector3 direction;
    private NetworkRigidbody rb;
    private List<Collider> collisions = new List<Collider>();
    private TickTimer timer;
    public float timerDespawn, timeEffect;
    public int damage, levelSkill;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;
    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
        rb = GetComponent<NetworkRigidbody>();
        trigger=GetComponent<CalculateTriggerEnter>();
        if (HasStateAuthority && HasInputAuthority)
        {
            timer = TickTimer.CreateFromSeconds(Runner, timerDespawn);
            if (rb!=null)
            {
                isDestroyWhenCollider = true;
                rb.Rigidbody.AddForce(direction * 1500);
                transform.forward = direction;
            }
        }
    }
    public void SetUp(PlayerController player, int levelDamage, bool isPhysicDamage, Transform parentObject = null,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float timeEffect = 0f, bool isDestroyWhenCollider = false, int levelSkill = 1)
    {
        this.player = player;
        transform.SetParent(parentObject);
        damage = levelDamage;
        this.isPhysicDamage = isPhysicDamage;
        this.isMakeStun = isMakeStun;
        this.isMakeSlow = isMakeSlow;
        this.isMakeSilen = isMakeSilen;
        this.timeEffect = timeEffect;
        this.isDestroyWhenCollider = isDestroyWhenCollider;
        timerDespawn = timeTrigger;
        this.levelSkill = levelSkill;
    }
    
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (HasStateAuthority && timer.Expired(Runner)
            )
        {
           player.GetComponent<Tesla>()?.EffectShotGun.gameObject.SetActive(false);
           Destroy(gameObject);
        }

    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if(HasStateAuthority)
        {
            trigger.ControlTrigger(other,collisions,player,damage,timeEffect,isPhysicDamage,
                isMakeStun, isMakeSlow, isMakeSilen,isDestroyWhenCollider,Object.InputAuthority);
            if (player.playerType==Player_Types.DumbleDore) collisions.Clear();
        }
    }
}
