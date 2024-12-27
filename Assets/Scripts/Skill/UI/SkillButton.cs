using Cinemachine;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    SkillManager skillManager;
    [SerializeField] public Image m_skillIcon, skillLevelImage;
    [SerializeField] Image m_CooldownOverlay;
    [SerializeField] Image m_timeTriggerFilled;

    [SerializeField] Text m_amountTxt;
    [SerializeField] Text m_cooldownTxt;
    [SerializeField] Button m_btnComp, AddSkill_LevelBtn;


    SkillController m_skillController;
    [SerializeField] PlayerController player;
    int m_currentAmount;
    public SkillButtonTypes[] m_skillButtonTypes;
    public SkillButtonTypes skillButtonType;
    public SkillTypes skillType;
    public NetworkObject VfxEffect;
    public SkillName m_skillName;
    public Action Skill_Trigger;
    public int indexInventory;
    int indexSkill;
    [SerializeField] float timerTrigger;
    [SerializeField] int[] levelManaCosts;
    [SerializeField] AudioClip triggerSoundFX;
    [SerializeField] int[] levelDamages;
    [SerializeField] bool isPhysicDamage;
    [SerializeField] bool isMakeStun;
    [SerializeField] bool isMakeSlow;
    [SerializeField] bool isMakeSilen;
    [SerializeField] float timeEffect;
    [SerializeField] public int levelSkill;
    [SerializeField] int damageSkill, manaCost;
    public string skillInfo;
    public string fullNameSkill;
    Vector3? posMouseUp;
    private float holdTime = 0.5f;  // Thời gian giữ nút
    private float timer = 0f;     // Bộ đếm thời gian giữ nút
    private bool isHolding;
    private bool shouldInvokeClick;
    #region EVENTS
    void RegisterEvent()
    {
        if (m_skillController == null) return;
        m_skillController.OnCooldown.AddListener(UpdateCooldown);
        m_skillController.OnSkillUpdate.AddListener(UpdateTimerTrigger);
        m_skillController.OnCooldownStop.AddListener(UpdateUI);
        m_skillController.OnNoItem.AddListener(NoItem);
    }
    void UnRegisterEvent()
    {
        if (m_skillController == null) return;
        m_skillController.OnCooldown.RemoveListener(UpdateCooldown);
        m_skillController.OnSkillUpdate.RemoveListener(UpdateTimerTrigger);
        m_skillController.OnCooldownStop.RemoveListener(UpdateUI);
        m_skillController.OnNoItem.RemoveListener(NoItem);
    }
    private void OnDestroy()
    {
        UnRegisterEvent();
    }
    #endregion


    private void Start()
    {
        if (!(skillButtonType == SkillButtonTypes.Items))
        {
            StartCoroutine(DelayCheckPlayer());
        }

        if (skillButtonType == SkillButtonTypes.Jump || skillButtonType == SkillButtonTypes.NormalAttack || skillButtonType == SkillButtonTypes.Teleport)
        {
            AddSkill_LevelBtn.gameObject.SetActive(false);
        }
    }
    IEnumerator DelayCheckPlayer()
    {
        yield return new WaitForSeconds(0.2f);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        this.player = player;
    }
    private void Update()
    {
        if(isHolding)
        {
            timer += Time.deltaTime;
            if (timer >= holdTime)
            {
                ShowInfoSkill();  // Thực hiện hành động sau khi giữ đủ lâu
                isHolding = false; // Dừng lại khi đã giữ đủ lâu
                timer = 0f;        // Reset bộ đếm
                shouldInvokeClick = false;
            }
        }
        if (player == null || skillButtonType == SkillButtonTypes.Jump ||
            skillButtonType == SkillButtonTypes.NormalAttack || skillButtonType == SkillButtonTypes.Teleport) return;
        skillLevelImage.fillAmount = levelSkill * 0.25f;
        AddSkill_LevelBtn.gameObject.SetActive(player.playerStat.levelPoint > 0 && levelSkill < 4);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
            player.playerStat.UpgradeLevel();
        }
        if (skillButtonType == SkillButtonTypes.Ultimate)
        {
            int maxSkillPointCanHave = (int)(player.playerStat.level / 3);
            AddSkill_LevelBtn.gameObject.SetActive(levelSkill < maxSkillPointCanHave && levelSkill < 4 && player.playerStat.levelPoint > 0);
        }
        if (skillButtonType == SkillButtonTypes.Skill_1 || skillButtonType == SkillButtonTypes.Skill_2)
        {
            int maxSkillPointCanHave = Mathf.CeilToInt((float)player.playerStat.level / 3);
            AddSkill_LevelBtn.gameObject.SetActive(levelSkill < maxSkillPointCanHave && levelSkill < 4 && player.playerStat.levelPoint > 0);
        }
    }
    public void Initialize(SkillName skillName)
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        skillManager = player.GetComponentInChildren<SkillManager>();
        m_skillName = skillName;
        if (m_btnComp != null)
        {
            m_btnComp.onClick.RemoveAllListeners();
            m_btnComp.onClick.AddListener(TriggerSkill);
        }
        
        m_skillController = skillManager.GetSkillController(skillName);
        skillType = m_skillController.skillType;
        VfxEffect = m_skillController.skillStat.VfxEffect;
        triggerSoundFX = m_skillController.skillStat.triggerSoundFX;
        levelDamages = new int[6]; levelManaCosts = new int[6];
        isPhysicDamage = m_skillController.skillStat.isPhysicDamage;
        isMakeStun = m_skillController.skillStat.isMakeStun;
        isMakeSlow = m_skillController.skillStat.isMakeSlow;
        isMakeSilen = m_skillController.skillStat.isMakeSilen;
        timeEffect = m_skillController.skillStat.timeEffect;
        timerTrigger = m_skillController.skillStat.timerTrigger;
        triggerSoundFX = m_skillController.skillStat.triggerSoundFX;
        for (int i = 0; i < m_skillController.skillStat.levelDamages.Length; i++)
        {
            levelDamages[i] = m_skillController.skillStat.levelDamages[i];
            levelManaCosts[i] = m_skillController.skillStat.levelManaCosts[i];
        }
        UpdateUI();
        RegisterEvent();
        if (skillType == SkillTypes.Items) levelSkill = 1;
        CalculateDamageAndManaCost();
        skillInfo = m_skillController.skillStat.skillInfo;
        GetFormattedInfo(levelDamages[1], levelDamages[2], levelDamages[3], levelDamages[4],
            levelManaCosts[1], levelManaCosts[2], levelManaCosts[3], levelManaCosts[4],
            timerTrigger, timeEffect, m_skillController.skillStat.cooldownTime);
    }

    private void UpdateUI()
    {
        if (m_skillController == null) return;
        if (m_skillIcon && m_skillName != SkillName.NoSkill)
            m_skillIcon.sprite = m_skillController.skillStat.skillIcon;
        // UpdateAmountTxt();
        UpdateCooldown();

        //UpdateTimerTrigger();
        //bool canActiveMe = m_currentAmount > 0 || m_skillController.IsCooldowning;
        // gameObject.SetActive(true); bị lỗi khi swap item tạo newSkillbutton mới
    }
    void NoItem()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        if (skillType == SkillTypes.Items && player.inventory.mSlots[transform.GetSiblingIndex()].Count == 0)
        {
            Initialize(SkillName.None);
        }
    }
    private void UpdateTimerTrigger()
    {
        if (m_skillController == null || m_timeTriggerFilled == null) return;
        float triggerProgress = m_skillController.triggerProgress;
        m_timeTriggerFilled.fillAmount = triggerProgress;
        m_timeTriggerFilled.transform.parent.gameObject.SetActive(m_skillController.IsTriggered);
    }

    private void UpdateCooldown()
    {
        if (m_cooldownTxt)
            m_cooldownTxt.text = m_skillController.CooldownTime.ToString(m_skillController.CooldownTime >= 1 ? "f0" : "f1");
        float cooldownProgress = m_skillController.cooldownProgress;
        if (m_CooldownOverlay)
        {
            m_CooldownOverlay.fillAmount = cooldownProgress;
            m_CooldownOverlay.gameObject.SetActive(m_skillController.IsCooldowning);
        }
    }

    private void UpdateAmountTxt()
    {
        m_currentAmount = FindObjectOfType<SkillManager>().GetSkillAmount(m_skillName);
        if (m_amountTxt)
        {
            m_amountTxt.text = $"x {m_currentAmount}";
        }
    }
    public void UpdateLevelSkill()
    {

        switch (skillButtonType)
        {
            case SkillButtonTypes.Ultimate: { indexSkill = 0; break; }
            case SkillButtonTypes.Skill_1: { indexSkill = 1; break; }
            case SkillButtonTypes.Skill_2: { indexSkill = 2; break; }
        }
        player.skillManager.indexSkill_Level.Set(indexSkill, player.skillManager.indexSkill_Level.Get(indexSkill) + 1);
        levelSkill++;
        player.playerStat.levelPoint--;
        CalculateDamageAndManaCost();
    }
    void CalculateDamageAndManaCost()
    {
        if (levelDamages.Length > 0)
        {
            damageSkill = levelDamages[levelSkill];
        }
        if (levelManaCosts.Length > 0)
        {
            manaCost = levelManaCosts[levelSkill];
        }
    }
    #region Trigger SKill
    void TriggerSkill()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        if (skillButtonType == SkillButtonTypes.Items)
        {
            ItemDragHandler dragHandler = transform.GetChild(0).GetChild(0).GetComponent<ItemDragHandler>();
            IInventoryItem item = dragHandler.Item;
            if (item != null)
            {
                player.inventory.UseItemClickInventory
                (player.inventory.mSlots[transform.GetSiblingIndex()].FirstItem, transform.GetSiblingIndex(), out bool canActive);
                if (!canActive || m_skillName == SkillName.NoSkill) return;
            }
        }
        if (m_skillController != null && !m_skillController.IsCooldowning
            && levelSkill != 0 && player.playerStat.currentMana >= manaCost
            && state == 0 && !player.playerStat.isBeingStun)
        {
            if (!shouldInvokeClick) return;
            if (skillButtonType == SkillButtonTypes.Jump)
            {
                player.Jump(VfxEffect);
            }
            if (skillButtonType == SkillButtonTypes.NormalAttack)
            {
                player.NormalAttack(VfxEffect, damageSkill, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen
                    , timerTrigger, timeEffect);
            }
            if (skillButtonType == SkillButtonTypes.Ultimate)
            {
                if (player.playerStat.isBeingSilen) return;
                player.Ultimate(VfxEffect, damageSkill, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen,
             timerTrigger, timeEffect, posMouseUp, levelSkill);

            }
            if (skillButtonType == SkillButtonTypes.Skill_2)
            {
                if (player.playerStat.isBeingSilen) return;
                player.Skill_2(VfxEffect, damageSkill, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen,
             timerTrigger, timeEffect, posMouseUp, levelSkill);
            }
            if (skillButtonType == SkillButtonTypes.Skill_1)
            {
                if (player.playerStat.isBeingSilen) return;
                player.Skill_1(VfxEffect, damageSkill, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen,
             timerTrigger, timeEffect, posMouseUp, levelSkill);
            }
            if (skillButtonType == SkillButtonTypes.Teleport)
            {
                if (player.playerStat.isBeingSilen || player.playerStat.isBeingTele) return;
                player.Teleport(VfxEffect);
            }
            if (skillButtonType == SkillButtonTypes.Items)
            {
                player.UseItemSkill(m_skillName, VfxEffect, damageSkill, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen,
       timerTrigger, timeEffect, posMouseUp, levelSkill);
            }
            m_skillController.Trigger();
        }
        else
        {
          //  Singleton<AudioManager>.Instance.PlaySound(Singleton<AudioManager>.Instance.error);
        }
    }
    #endregion
    
    #region Pointer
    public void PointerDown() //khóa camera khi giữ chuột trái tại skill
    {
        isHolding = true;
        timer = 0f;  
        shouldInvokeClick = true;
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        if (m_skillController == null || m_skillController.IsCooldowning
            || levelSkill == 0 || player.playerStat.currentMana < manaCost || state != 0
            || skillType != SkillTypes.Direction_Active || player.playerStat.isBeingStun
            || player.playerStat.isBeingSilen) return;
        player.state = 5;
        player.gameObject.GetComponent<SkillDirection>().GetMouseDown();
        return;
    }
    public void PointDrag()
    {
        isHolding = false;
        timer = 0f;
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        if (m_skillController == null || m_skillController.IsCooldowning
             || levelSkill == 0 || player.playerStat.currentMana < manaCost || state != 5
             || skillType != SkillTypes.Direction_Active || player.playerStat.isBeingStun
             || player.playerStat.isBeingSilen) return;
        Quaternion look = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, look, 720 * Time.deltaTime);
        Singleton<CameraController>.Instance.MoveCameraUp();
    }
    public void PointerUp()
    {
        isHolding = false;
        timer = 0f;
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        player.gameObject.GetComponent<SkillDirection>().GetMouseUp(out Vector3? posMouseUp);
        
        if (m_skillController == null || m_skillController.IsCooldowning
                || levelSkill == 0 || player.playerStat.currentMana < manaCost || state != 5
                || skillType != SkillTypes.Direction_Active) return;
        player.state = 0;
        if (player.playerStat.isFollowEnemy)
        {
            this.posMouseUp = player.overlapSphere.closestEnemyPlayer.transform.position;
        }
        else if (posMouseUp != null)
        {
            this.posMouseUp = posMouseUp;
        }
        else
        {
            this.posMouseUp = player.transform.position + player.transform.forward * 20;
        }
        m_btnComp.onClick.Invoke();
    }
    #endregion
    void ShowInfoSkill()
    {
        FindObjectOfType<AbilityPlayer>().ShowInfoSkill(this);
    }
    public string GetFormattedInfo(int d1,int d2,int d3,int d4,int m1,int m2,int m3,int m4,
        float timeTrigger, float timeEffect, float cooldown)
    {
        skillInfo= skillInfo.Replace("{d1}", d1.ToString());
        skillInfo = skillInfo.Replace("{d2}", d2.ToString());
        skillInfo = skillInfo.Replace("{d3}", d3.ToString());
        skillInfo = skillInfo.Replace("{d4}", d4.ToString());
        skillInfo = skillInfo.Replace("{m1}", m1.ToString());
        skillInfo = skillInfo.Replace("{m2}", m2.ToString());
        skillInfo = skillInfo.Replace("{m3}", m3.ToString());
        skillInfo = skillInfo.Replace("{m4}", m4.ToString());
        skillInfo = skillInfo.Replace("{timeTrigger}", timeTrigger.ToString());
        skillInfo = skillInfo.Replace("{timeEffect}", timeEffect.ToString());
        skillInfo = skillInfo.Replace("{cd}", cooldown.ToString());
        return skillInfo;
    }
}

