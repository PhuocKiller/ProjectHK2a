using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlapSpherePlayer : NetworkBehaviour
{
    PlayerController player;
    public PlayerController closestEnemyPlayer;
    public List<PlayerController> enemyPlayers =new List<PlayerController>();
    RectTransform crossHair;
    public override void Spawned()
    {
        base.Spawned();
        player=GetComponentInParent<PlayerController>();
        crossHair = FindObjectOfType<UIManager>().crossHair;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(HasStateAuthority)
        {
            CheckPlayerAround();
            crossHair.gameObject.SetActive(enemyPlayers.Count > 0); //hiện hình crossHair
            if (enemyPlayers.Count > 0)
            {
                closestEnemyPlayer = FindClosestObjectInRadius(enemyPlayers, transform.position);
                Vector3 posViewPort = Camera.main.WorldToScreenPoint(closestEnemyPlayer.transform.position+Vector3.up*2);
                crossHair.position = posViewPort;
            }
            else 
            { 
                closestEnemyPlayer = null;
                player.playerStat.isFollowEnemy = false;
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
                    if (enemyPlayer.GetComponent<NetworkObject>().IsValid&& enemyPlayer.playerTeam != player.playerTeam&& enemyPlayer.state!=3)
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
