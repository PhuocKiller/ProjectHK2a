using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicDamage : MonoBehaviour
{
    public float deltaDamage = 0.02f;
    public bool isCritPhysicDamage;
    public int GetDamageOfTwoObject(int damage, bool isPhysicDamage, PlayerController playerAttack, Collider ObjectBeAttack, out bool isCritPhysic)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (ObjectBeAttack.GetComponent<PlayerController>() != null)
        {
            PlayerController playerBeAtk=ObjectBeAttack.GetComponent<PlayerController>();
            int baseDamage = (int)(damage * Random.Range(0.95f, 1.05f) *
          (1 - (deltaDamage * (isPhysicDamage ? playerBeAtk.playerStat.defend : playerBeAtk.playerStat.magicResistance)
          / (1 + deltaDamage * ObjectBeAttack.GetComponent<PlayerController>().playerStat.defend))));
            CheckCritPhysicDamage(playerAttack, isPhysicDamage, out float increaseDamage);
            isCritPhysic = isCritPhysicDamage;
            bool getMoreDamgeFromDayTime = gameManager.moonLightTime != playerBeAtk.playerTeam;
            return (int)(baseDamage * increaseDamage * (getMoreDamgeFromDayTime?1.2f:1));
        }
        else if (ObjectBeAttack.GetComponent<CreepController>() != null)
        {
            CreepController creep = ObjectBeAttack.GetComponent<CreepController>();
            int baseDamage = (int)(damage * Random.Range(0.95f, 1.05f) *
        (1 - (deltaDamage * (isPhysicDamage ? creep.playerStat.defend : creep.playerStat.magicResistance)
        / (1 + deltaDamage * creep.playerStat.defend))));
            CheckCritPhysicDamage(playerAttack, isPhysicDamage, out float increaseDamage);
            isCritPhysic = isCritPhysicDamage;
            bool getMoreDamgeFromDayTime = gameManager.moonLightTime != creep.playerTeam;
            return (int)(baseDamage * increaseDamage * (getMoreDamgeFromDayTime ? 1.2f : 1));
        }

        else if (ObjectBeAttack.GetComponent<BuildingController>() != null)
        {
            BuildingController building= ObjectBeAttack.GetComponent<BuildingController>();
            if (!isPhysicDamage)
            {
                isCritPhysic = isCritPhysicDamage;
                return 0;
            }
            else
            {
                isCritPhysic = isCritPhysicDamage;
                bool getMoreDamgeFromDayTime = gameManager.moonLightTime != building.playerTeam;
                return (int)(damage * Random.Range(0.95f, 1.05f) * (getMoreDamgeFromDayTime ? 1.2f : 1)*
          (1 - (deltaDamage * (building.defend)
          / (1 + deltaDamage * building.defend))));
            }

        }
        else
        {
            isCritPhysic = isCritPhysicDamage;
            return (int)(damage * Random.Range(0.95f, 1.05f));
        }

    }
    void CheckCritPhysicDamage(PlayerController playerAttack, bool isPhysicDamage, out float increaseDamage)
    {
        if (playerAttack)
        {
            if (isPhysicDamage)
            {
                increaseDamage = IncreasePhysicDamage(playerAttack, out bool isCrited);
                isCritPhysicDamage = isCrited;
            }
            else
            {
                increaseDamage = IncreaseMagicDamage(playerAttack);
                isCritPhysicDamage = false;
            }
        }
        else
        {
            increaseDamage = 1;
            isCritPhysicDamage = false;
        }
    }
    public float IncreasePhysicDamage(PlayerController playerAttack, out bool isCrited)
    {
        if (GetChance(playerAttack.playerStat.criticalChance))
        {
            isCrited = true;
            return playerAttack.playerStat.criticalDamage;
        }
        else
        {
            isCrited = false; return 1;
        }
    }
    public float IncreaseMagicDamage(PlayerController playerAttack)
    {
        return (1 + playerAttack.playerStat.magicAmpli);
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
