using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObjectsTower : NetworkBehaviour
{
    TowerController tower;
    CalculateTriggerEnter trigger;
    private Vector3 direction;
    private NetworkRigidbody rb;
    private List<Collider> collisions = new List<Collider>();
    private TickTimer timer;
    public float timerDespawn, timeEffect;
    public int damage;
    
    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
        rb = GetComponent<NetworkRigidbody>();
        trigger = GetComponent<CalculateTriggerEnter>();
        if (HasStateAuthority)
        {
            timer = TickTimer.CreateFromSeconds(Runner, timerDespawn);
            if (rb != null)
            {
                rb.Rigidbody.AddForce(direction * 1500);
                transform.forward = direction;
            }
        }
    }
    public void SetUpTower(TowerController tower, int levelDamage, Transform parentObject = null,
         float timeTrigger = 0f)
    {
        this.tower = tower;
        transform.SetParent(parentObject);
        damage = levelDamage;
        timerDespawn = timeTrigger;
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (HasStateAuthority && timer.Expired(Runner))
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(CharacterController character)
    {
      //  direction = newDirection;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority)
        {
            if (other.gameObject.layer == 7 && collisions.Count == 0
            && other.gameObject.GetComponent<PlayerController>().state != 3
            && other.gameObject.GetComponent<PlayerController>().playerTeam != tower.playerTeam)
            {
                trigger.ControlTriggerTower(other, collisions, null, damage, true,
                true, Object.InputAuthority);
            }
            if (other.gameObject.layer == 8 && collisions.Count == 0
            && other.gameObject.GetComponent<CreepController>().state != 3
            && other.gameObject.GetComponent<CreepController>().playerTeam != tower.playerTeam)
            {
                trigger.ControlTriggerTower(other, collisions, null, damage, true,
                true, Object.InputAuthority);
            }
        }
    }
}
