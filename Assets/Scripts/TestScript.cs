using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TestScript : NetworkBehaviour
{
    public NetworkManager runnerManager;
    public CharacterController targetCharacter;
    public GameManager gameManager;
    public OverlapSphereCreep overlapSphere;
    public PlayerScore playerScore;
    public Bars hpBar;
    [Networked] public int playerTeam { get; set; }

    public CharacterController characterControllerPrototype;

    [Networked] public int state { get; set; }

    [Networked] public int currentHealth { get; set; }
    [Networked] public int maxHealth { get; set; }
    [Networked] public int damage { get; set; }
    [Networked] public int defend { get; set; }
    [Networked] public bool isLive { get; set; }
    public Mesh[] meshTower;
    public override void Spawned()
    {
        base.Spawned();
        runnerManager = FindObjectOfType<NetworkManager>();
        gameManager = FindObjectOfType<GameManager>();
        hpBar = GetComponentInChildren<Bars>();
        maxHealth = 3000 + gameManager.levelCreep * 50;
        currentHealth = maxHealth;
        state = 0;
        characterControllerPrototype = GetComponent<CharacterController>();
        Debug.Log("vo spawm" + GetComponent<NetworkObject>().Id);
        Debug.Log("vo spawm" + GetComponent<NetworkObject>().Id);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        Debug.Log("vo update" + GetComponent<NetworkObject>().Id);
        if (state != 3)
        {
            hpBar.UpdateBar(currentHealth, maxHealth);
        }
        if (HasStateAuthority)
        {
            if (overlapSphere.CheckAllEnemyAround().Count == 0)
            {

            }
            else //có enemy xung quanh
            {
                if (overlapSphere.CheckPlayerFollowEnemy(overlapSphere.CheckAllEnemyAround()).Count == 0)// nhưng ko có player follow
                {
                    targetCharacter = overlapSphere.FindClosestCharacterInRadius(overlapSphere.CheckAllEnemyAround(), transform.position);
                }
                else //có player follow
                {
                    targetCharacter = overlapSphere.FindClosestPlayerFollowInRadius
                        (overlapSphere.CheckPlayerFollowEnemy(overlapSphere.CheckAllEnemyAround()), transform.position)
                        .GetComponent<CharacterController>();
                }
            }
            Quaternion look = Quaternion.LookRotation((targetCharacter.transform.position - transform.position).normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, 360 * Runner.DeltaTime);
        }
    }
}
