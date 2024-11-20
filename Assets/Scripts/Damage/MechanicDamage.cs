using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicDamage : MonoBehaviour
{
    public float deltaDamage = 0.06f;
    private void Awake()
    {
        
    }
    public int GetDamageOfTwoObject(int damage, bool isPhysicDamage, int defend,float magicResistance, float damageIncrease, float damageReduce)
    {
        return (int)(damage * UnityEngine.Random.Range(0.95f, 1.05f) *
            (1 - (deltaDamage * defend / (1 + deltaDamage * defend)))
           * damageIncrease * damageReduce);
    }
    public float IncreaseDamagePlayer(bool isCrit)
    {
        float damageFromRage;
     

        
        if (isCrit)
        {
            return 2f;
        }
        else { damageFromRage = 1f; }
        return damageFromRage;
    }
    public float DecreaseDamageMonster()
    {
        return 0;
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
