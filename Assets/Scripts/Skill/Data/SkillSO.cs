using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillSystem/Create Skill Data")]
public class SkillSO : ScriptableObject
{
    public SkillName skillName;
    public SkillTypes skillType;
    public float timerTrigger;
    public float cooldownTime;
    public int[] levelManaCosts;
    public Sprite skillIcon;
    public AudioClip triggerSoundFX;
    public int[] levelDamages;
    public bool isPhysicDamage;
    public bool isMakeStun;
    public bool isMakeSlow;
    public bool isMakeSilen;
    public float timeEffect;
    public NetworkObject VfxEffect;
    [TextArea(3, 10)] public string skillInfo;
    public string SkillInfo
    {
        get
        {
          return  GetFormattedInfo(levelDamages[1], levelDamages[2], levelDamages[3], levelDamages[4],
            levelManaCosts[1], levelManaCosts[2], levelManaCosts[3], levelManaCosts[4], timerTrigger, timeEffect);
        }
    }
    public string GetFormattedInfo(int d1, int d2, int d3, int d4, int m1, int m2, int m3, int m4, float timeTrigger, float timeEffect)
    {
        skillInfo = skillInfo.Replace("{d1}", d1.ToString());
        skillInfo = skillInfo.Replace("{d2}", d2.ToString());
        skillInfo = skillInfo.Replace("{d3}", d3.ToString());
        skillInfo = skillInfo.Replace("{d4}", d4.ToString());
        skillInfo = skillInfo.Replace("{m1}", m1.ToString());
        skillInfo = skillInfo.Replace("{m2}", m2.ToString());
        skillInfo = skillInfo.Replace("{m3}", m3.ToString());
        skillInfo = skillInfo.Replace("{m4}", m4.ToString());
        skillInfo = skillInfo.Replace("{timeTrigger}", timeTrigger.ToString());
        skillInfo = skillInfo.Replace("{timeEffect}", timeEffect.ToString());
        return skillInfo;
    }
}

