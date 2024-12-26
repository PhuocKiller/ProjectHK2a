using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPlayer : MonoBehaviour
{ 
    [SerializeField] GameObject abilityPanel;
    [SerializeField] Image iconSkill;
    [SerializeField] TextMeshProUGUI skillInfo;
    [SerializeField] TextMeshProUGUI fullNameSkill;
    public void ShowInfoSkill(SkillButton button)
    {
        abilityPanel.SetActive(true);
        iconSkill.sprite=button.m_skillIcon.sprite;
        skillInfo.text = button.skillInfo;
        fullNameSkill.text = button.m_skillName.ToString();
    }
}
