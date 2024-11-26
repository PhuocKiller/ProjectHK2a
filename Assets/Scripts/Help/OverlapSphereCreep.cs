using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlapSphereCreep : NetworkBehaviour
{
    CreepController creep;
    public PlayerController closestEnemyPlayer;
    public List<PlayerController> enemyPlayers = new List<PlayerController>();
    public List<CharacterController> allEnemies = new List<CharacterController>();
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
        if (HasStateAuthority)
        {
            CheckPlayerAround();
            CheckAllEnemyAround();
            if (enemyPlayers.Count > 0)
            {
                closestEnemyPlayer = FindClosestObjectInRadius(enemyPlayers, transform.position);
            }
            else
            {
                closestEnemyPlayer = null;
            }
        }
    }
    public void CheckAllEnemyAround()
    {
        allEnemies.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);

        foreach (var hitCollider in hitColliders)
        {
            PlayerController enemyPlayer = hitCollider.gameObject.GetComponent<PlayerController>();
            if(enemyPlayer != null)
            {
                if (enemyPlayer.playerTeam != creep.playerTeam)
                {
                    allEnemies.Add(enemyPlayer.gameObject.GetComponent<CharacterController>());
                }
            }
            CreepController enemyCreep = hitCollider.gameObject.GetComponent<CreepController>();
            if (enemyCreep != null)
            {
                if (enemyCreep.playerTeam != creep.playerTeam)
                {
                    allEnemies.Add(enemyCreep.gameObject.GetComponent<CharacterController>());
                }
            }
        }
        foreach(var charac in allEnemies)
        {
            Debug.Log(charac);
        }
    }
    public void CheckPlayerAround()
    {
        enemyPlayers.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 15f);

        foreach (var hitCollider in hitColliders)
        {
            PlayerController enemyPlayer = hitCollider.gameObject.GetComponent<PlayerController>();

            if (enemyPlayer != null)
            {
                if (enemyPlayer.playerTeam != creep.playerTeam)
                {
                    enemyPlayers.Add(enemyPlayer);
                }
            }
        }
    }
    PlayerController FindClosestObjectInRadius(List<PlayerController> enemyPlayers, Vector3 currentPos)
    {
        return enemyPlayers
            .OrderBy(player => Vector3.Distance(player.transform.position, currentPos)).FirstOrDefault();
    }
}
