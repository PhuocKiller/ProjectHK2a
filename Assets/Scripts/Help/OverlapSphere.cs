using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlapSphere : NetworkBehaviour
{
    PlayerController player;
    public PlayerController closestEnemyPlayer;
    CreepController creep;
    public List<PlayerController> enemyPlayers =new List<PlayerController>();
    RectTransform crossHair;
    public override void Spawned()
    {
        base.Spawned();
        player=GetComponentInParent<PlayerController>();
        creep= GetComponentInParent<CreepController>();
        crossHair = FindObjectOfType<UIManager>().crossHair;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(HasStateAuthority)
        {
            CheckPlayerAround();
            if (player == null) return; //nếu là creep thì ko chạy đoạn dưới
            crossHair.gameObject.SetActive(enemyPlayers.Count > 0);
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
                if (player != null)
                {
                    if (enemyPlayer.playerTeam != player.playerTeam)
                    {
                        enemyPlayers.Add(enemyPlayer);
                    }
                }
                else //nếu là creep
                {
                    if (enemyPlayer.playerTeam != creep.playerTeam)
                    {
                        enemyPlayers.Add(enemyPlayer);
                    }
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
