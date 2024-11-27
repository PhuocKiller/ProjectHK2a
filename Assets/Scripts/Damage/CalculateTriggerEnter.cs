using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateTriggerEnter : MonoBehaviour
{
    public void ControlTriggerPlayer(Collider other, List<Collider> collisions, PlayerController player, int damage,float timeEffect,
        bool isPhysicDamage, bool isMakeStun, bool isMakeSlow, bool isMakeSilen, bool isDestroyWhenCollider,PlayerRef InputAuthority,int levelSkill=1)
    {
            collisions.Add(other);
            other.gameObject.GetComponent<ICanTakeDamage>().ApplyDamage(damage, isPhysicDamage, player,
                counter: (int counterDamage, bool isPhysicDamage) =>
                {
                    player.ApplyDamage(counterDamage, isPhysicDamage,
                         other.gameObject.GetComponent<PlayerController>());
                }
                , isKillPlayer: (int levelHeroKilled, List<PlayerController> playerMakeDamage) => // Nhận exp khi giêt địch ở đây
                {
                    player.playerStat.currentXP += (int)(100 * Mathf.Lerp(1 / playerMakeDamage.Count, 1, 0.5f) * levelHeroKilled);
                    player.playerScore.killScore += 1;
                    player.playerScore.assistScore -= 1;
                    if (player.playerType==Player_Types.DumbleDore) player.playerStat.currentMana += (int)(player.playerStat.maxMana * 0.2 * levelSkill);
                }
                , lifeSteal: (int damage) =>
                {
                    if (player.playerStat.isLifeSteal) player.playerStat.currentHealth += (int)(player.playerStat.lifeSteal * damage);
                }
                );
            other.gameObject.GetComponent<ICanTakeDamage>().ApplyEffect(InputAuthority, isMakeStun, isMakeSlow, isMakeSilen,
                TimeEffect: timeEffect, callback: () =>
                {
                    if (isDestroyWhenCollider) Destroy(gameObject);//khi chạm vào địch thì hủy vật thể
                }
                );
    }
    public void ControlTriggerCreep(Collider other, List<Collider> collisions,CreepController creep, int damage, float timeEffect,
       bool isPhysicDamage, bool isMakeStun, bool isMakeSlow, bool isMakeSilen, bool isDestroyWhenCollider, PlayerRef InputAuthority, int levelSkill = 1)
    {
        collisions.Add(other);
        other.gameObject.GetComponent<ICanTakeDamage>().ApplyDamage(damage, isPhysicDamage, null,
            counter: (int counterDamage, bool isPhysicDamage) =>
            {
                creep.ApplyDamage(counterDamage, isPhysicDamage,null);
            }
            , isKillPlayer: (int levelHeroKilled, List<PlayerController> playerMakeDamage) => // Nhận exp khi giêt địch ở đây
            {
               
            }
            , lifeSteal: (int damage) =>
            {
                if (creep.playerStat.isLifeSteal) creep.playerStat.currentHealth += (int)(creep.playerStat.lifeSteal * damage);
            }
            );
        other.gameObject.GetComponent<ICanTakeDamage>().ApplyEffect(InputAuthority, isMakeStun, isMakeSlow, isMakeSilen,
            TimeEffect: timeEffect, callback: () =>
            {
                if (isDestroyWhenCollider) Destroy(gameObject);//khi chạm vào địch thì hủy vật thể
            }
            );
    }
}
