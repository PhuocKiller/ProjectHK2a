using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPanel : MonoBehaviour
{
    PlayerController player;
    private void OnEnable()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        this.player = player;
    }
    public void LevelPlus_1()
    {
        player.playerStat.currentXP= player.playerStat.maxXP;
    }
    public void LevelBackTo_1()
    {
        player.playerStat.currentXP = 0;
        player.playerStat.level = 0;
        player.playerStat.levelPoint = 0;
        player.playerStat.UpgradeLevel();
        foreach(var button in FindObjectsOfType<SkillButton>())
        {
            if (button.skillButtonType == SkillButtonTypes.Skill_1 || button.skillButtonType == SkillButtonTypes.Skill_2
                || button.skillButtonType == SkillButtonTypes.Ultimate)
            {
                button.levelSkill = 0;
            }
        }
    }
    public void CoinPlus1000()
    {
        player.playerStat.coinsValue += 1000;
    }
    public void ResetCoolDown()
    {
        foreach (var button in FindObjectsOfType<SkillButton>())
        {
            if (button.skillButtonType == SkillButtonTypes.Skill_1 || button.skillButtonType == SkillButtonTypes.Skill_2
                || button.skillButtonType == SkillButtonTypes.Ultimate)
            {
                button.m_skillController.ResetCoolDownTime();
            }
        }
    }
}
