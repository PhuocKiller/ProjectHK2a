using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicDamage : MonoBehaviour
{
    public float deltaDamage = 0.03f;
    private void Awake()
    {
        
    }
    public int GetDamageOfTwoObject(int damage, bool isPhysicDamage, PlayerController playerAttack, Collider ObjectBeAttack)
    {
            if (ObjectBeAttack.GetComponent<PlayerController>() != null)
            {
                return (int)(damage * Random.Range(0.95f, 1.05f) *
            (1 - (deltaDamage *(isPhysicDamage? ObjectBeAttack.GetComponent<PlayerController>().playerStat.defend: ObjectBeAttack.GetComponent<PlayerController>().playerStat.magicResistance)
            / (1 + deltaDamage * ObjectBeAttack.GetComponent<PlayerController>().playerStat.defend)))
            * (playerAttack != null ? IncreasePhysicDamage(playerAttack) : 1));
            }
            else if (ObjectBeAttack.GetComponent<CreepController>() != null)
            {
                return (int)(damage * Random.Range(0.95f, 1.05f) *
            (1 - (deltaDamage * (isPhysicDamage ? ObjectBeAttack.GetComponent<CreepController>().playerStat.defend : ObjectBeAttack.GetComponent<CreepController>().playerStat.magicResistance)
            / (1 + deltaDamage * ObjectBeAttack.GetComponent<CreepController>().playerStat.defend)))
            * (playerAttack != null ? IncreaseMagicDamage(playerAttack) : 1));
            }
            else return (int)(damage * Random.Range(0.95f, 1.05f));
    }
    public float IncreasePhysicDamage(PlayerController playerAttack)
    {
        if (GetChance(playerAttack.playerStat.criticalChance))
        {
            return playerAttack.playerStat.criticalDamage;
        }
        else return 1;
    }
    public float IncreaseMagicDamage(PlayerController playerAttack)
    {
       
            return (1+playerAttack.playerStat.magicAmpli);
       
    }
    public bool GetChance(float chance)
    {
        float r = Random.Range(1f, 100f);
        if (r / 100 < chance)
        {
            return true;
        }
        else return false;
    }
}
