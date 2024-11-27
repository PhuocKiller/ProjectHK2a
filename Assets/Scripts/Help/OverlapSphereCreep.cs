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
            if (allEnemies.Count > 0)
            {
                closestCharac = FindClosestObjectInRadius(allEnemies, transform.position);
            }
            else
            {
                closestCharac = null;
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
    CharacterController FindClosestObjectInRadius(List<CharacterController> enemyCharac, Vector3 currentPos)
    {
        return enemyCharac
            .OrderBy(charac => Vector3.Distance(charac.transform.position, currentPos)).FirstOrDefault();
    }
}
