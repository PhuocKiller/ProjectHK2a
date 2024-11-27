using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlapSphereCreep : NetworkBehaviour
{
    CreepController creep;
    public CharacterController closestCharac;
    public List<PlayerController> enemyPlayers = new List<PlayerController>();
    
    RectTransform crossHair;
    public override void Spawned()
    {
        base.Spawned();
        creep = GetComponentInParent<CreepController>();
        crossHair = FindObjectOfType<UIManager>().crossHair;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
       
    }
    public List<CharacterController> CheckAllEnemyAround()
    {   List<CharacterController> allEnemies = new List<CharacterController>();
        allEnemies.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);

        foreach (var hitCollider in hitColliders)
        {
            PlayerController enemyPlayer = hitCollider.gameObject.GetComponent<PlayerController>();
            if(enemyPlayer != null && enemyPlayer.GetComponent<NetworkObject>().IsValid)
            {
                if (enemyPlayer.playerTeam != creep.playerTeam)
                {
                    allEnemies.Add(enemyPlayer.gameObject.GetComponent<CharacterController>());
                }
            }
            CreepController enemyCreep = hitCollider.gameObject.GetComponent<CreepController>();
            if (enemyCreep != null && enemyCreep.GetComponent<NetworkObject>().IsValid)
            {
                if (enemyCreep.playerTeam != creep.playerTeam)
                {
                    allEnemies.Add(enemyCreep.gameObject.GetComponent<CharacterController>());
                }
            }
        }
        return allEnemies;
    }
    public List<PlayerController> CheckPlayerFollowEnemy(List<CharacterController> checkAllEnemyAround)
    {
        List<PlayerController> listPlayerFollowEnemy = new List<PlayerController>();
        foreach (var enemy in checkAllEnemyAround)
        {
            if(enemy.GetComponent<PlayerController>() != null)
            {
                if(enemy.GetComponent<PlayerController>().playerStat.isFollowEnemy)
                {
                    listPlayerFollowEnemy.Add(enemy.GetComponent<PlayerController>());  
                }
            }
        }
        return listPlayerFollowEnemy;
    }
    public void CheckPlayerAround()
    {
        enemyPlayers.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 15f);

        foreach (var hitCollider in hitColliders)
        {
            PlayerController enemyPlayer = hitCollider.gameObject.GetComponent<PlayerController>();

            if (enemyPlayer != null && enemyPlayer.GetComponent<NetworkObject>().IsValid)
            {
                if (enemyPlayer.playerTeam != creep.playerTeam)
                {
                    enemyPlayers.Add(enemyPlayer);
                }
            }
        }
    }
    public CharacterController FindClosestCharacterInRadius(List<CharacterController> enemyCharac, Vector3 currentPos)
    {
        return enemyCharac
            .OrderBy(charac => Vector3.Distance(charac.transform.position, currentPos)).FirstOrDefault();
    }
    public PlayerController FindClosestPlayerFollowInRadius(List<PlayerController> playerFollow, Vector3 currentPos)
    {
        return playerFollow
            .OrderBy(player => Vector3.Distance(player.transform.position, currentPos)).FirstOrDefault();
    }
    
}
