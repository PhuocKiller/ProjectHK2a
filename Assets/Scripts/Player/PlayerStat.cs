﻿using System;
using System.ComponentModel;
using Fusion;
using UnityEngine;

public class PlayerStat: NetworkBehaviour
{
    [SerializeField] PlayerBuffManager playerBuffManager;
    [Networked] public int level {  get; set; }
    [Networked] public int levelPoint { get; set; }
    [Networked] public int b_maxHealth {  get; set; }
    [Networked] public int currentHealth { get; set; }
    [Networked] public int b_maxMana { get; set; }
    [Networked] public int currentMana { get; set; }
    [Networked] public int b_damage { get; set; }
    [Networked] public int b_defend { get; set; }
    [Networked] public float b_magicResistance { get; set; }
    [Networked] public float b_magicAmpli { get; set; }
    [Networked] public float b_criticalChance { get; set; }
    [Networked] public float b_criticalDamage { get; set; }
    [Networked] public int b_moveSpeed { get; set; }
    [Networked] public int b_attackSpeed { get; set; }
    [Networked] public int counterDamage { get; set; }
    [Networked] public float b_lifeSteal { get; set; }
    [Header("Full Stat")]
    public int maxHealth;
    public int maxMana;
    [Networked] public int maxXP { get; set; }
    [Networked] public int currentXP { get; set; }
    public int damage;
    public int defend;
    public float magicResistance;
    public float magicAmpli;
    public float criticalChance;
    public float criticalDamage;
    public int moveSpeed;
    public int attackSpeed;
    public float lifeSteal;
    [Space(1)]
    [Header("Multiple Stat")]
    public int multipleHealth;
    public int multipleMana;
    public int multipleDamage;
    public int multipleDefend;
    public float multipleMagicResistance;
    public float multipleMagicAmpli;
    public float multipleCriticalChance;
    public float multipleCriticalDamage;
    public int multipleMoveSpeed;
    public int multipleAttackSpeed;
    public float multipleLifeSteal;

    PlayerController player;
    [HideInInspector][Networked] public bool isBeingStun { get; set; }
    [HideInInspector][Networked] public bool isBeingSlow { get; set; }
    [HideInInspector][Networked] public bool isBeingSilen { get; set; }
    [HideInInspector][Networked] public bool isLifeSteal { get; set; }
    [Networked] public bool isVisible { get; set; }
    [Networked] public bool isUnstopAble { get; set; }
    [Networked] public bool isCounter { get; set; }
    [Networked] public bool isLive { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        player=transform.parent.parent.GetComponent<PlayerController>();
        currentHealth = 1; //tránh bị bằng =0 trong lần đầu tiên cập nhật
        levelPoint = 0;
        UpgradeLevel();
        isVisible =true; isLive =true; isLifeSteal =true;
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        UpdateFullStat();
    }
    public void UpdateBaseStat(int level, int multipleHealth, int multipleMana, int multipleDamage, int multipleDefend,
        float multipleMagicResistance, float multipleMagicAmpli,
        float multipleCriticalChance,float multipleCriticalDamage,int multipleMoveSpeed,int multipleAttackSpeed, float multipleLifeSteal)
    {
        b_maxHealth = 300 + (level-1) * multipleHealth; b_maxMana = 100 + (level - 1) * multipleMana;
        if(level>=12) currentXP = 0;
        maxXP = 100 + (level - 1) * (level - 1) * 50;
        b_damage = 50 + (level - 1) * multipleDamage; b_defend= 5 + ((level - 1) * multipleDefend);
        b_magicResistance = 5 + (level - 1) * multipleMagicResistance; b_magicAmpli = 0 + (level - 1) * multipleMagicAmpli;
        b_criticalChance =0+ (level - 1) * multipleCriticalChance; b_criticalDamage= 1+ (level - 1) * multipleCriticalDamage;
        b_moveSpeed=300+((level - 1) * multipleMoveSpeed);
        b_attackSpeed=100 + ((level - 1) * multipleAttackSpeed);
        b_lifeSteal=0 + ((level - 1) * multipleLifeSteal);
    }
    public void UpgradeLevel()
    {
        level++; levelPoint++;
        UpdateBaseStat(level, multipleHealth, multipleMana, multipleDamage, multipleDefend,
            multipleMagicResistance,multipleMagicAmpli, 
            multipleCriticalChance, multipleCriticalDamage, multipleMoveSpeed, multipleAttackSpeed, multipleLifeSteal);
        UpdateFullStat();
        currentHealth=maxHealth;
        currentMana=maxMana;
    }

    private void UpdateFullStat()
    {
        if (player.state == 3) return;
        maxHealth = b_maxHealth + playerBuffManager.maxHealth;
        if(maxHealth<1) maxHealth = 1;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        
        maxMana = b_maxMana + playerBuffManager.maxMana;
        if(currentMana > maxMana) { currentMana = maxMana; }
        damage = b_damage + playerBuffManager.damage;
        defend = b_defend + playerBuffManager.defend;
        magicResistance = b_magicResistance + playerBuffManager.magicResistance;
        criticalChance = b_criticalChance + playerBuffManager.criticalChance;
        if(criticalChance>1) criticalChance = 1;    
        criticalDamage = b_criticalDamage + playerBuffManager.criticalDamage;
        moveSpeed = b_moveSpeed + playerBuffManager.moveSpeed;
        attackSpeed = b_attackSpeed + playerBuffManager.attackSpeed;
        lifeSteal = b_lifeSteal + playerBuffManager.lifeSteal;
        if (attackSpeed < 25) attackSpeed = 25;
    }
    public void GainXPWhenKill (int XPGain)
    {
        currentXP += XPGain;
        player.playerScore.assistScore += 1;
    }
    
}
