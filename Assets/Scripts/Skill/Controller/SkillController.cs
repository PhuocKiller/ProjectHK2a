using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillController : MonoBehaviour
{
    public SkillName skillName;
    public SkillTypes skillType;
    public NetworkObject VFXPrefab;
    public SkillSO skillStat;
    protected bool m_isTriggered, m_isCooldowning;
    protected float m_cooldownTime;
    protected float m_triggerTime;
    public UnityEvent OnTriggerEnter, OnSkillUpdate, OnCooldown, OnStop,OnCooldownStop;
    public UnityEvent<SkillName, int> OnStopWithType;
    

    public float cooldownProgress
    {
        get => CooldownTime / skillStat.cooldownTime;
    }
    public float triggerProgress
    {
        get => m_triggerTime / skillStat.timerTrigger;
    }
    public bool IsTriggered { get => m_isTriggered; }
    public bool IsCooldowning { get => m_isCooldowning; }
    public float CooldownTime { get => m_cooldownTime; }
    public virtual void LoadStat()
    {
        if (skillStat == null) return;
        m_cooldownTime = skillStat.cooldownTime;
        m_triggerTime = skillStat.timerTrigger;
        VFXPrefab = skillStat.VfxEffect;
    }
    public void Trigger() //khi ấn button lệnh ở đây
    {
        if (m_isTriggered || m_isCooldowning) return;
        m_isCooldowning = true;
       // m_isTriggered = true;
        OnTriggerEnter?.Invoke();
    }
    private void Update()
    {
        CoreHandle();
    }
    void CoreHandle()
    {
      //  ReduceTriggerTime();
        ReduceCooldownTime();
    }
    void ReduceTriggerTime()
    {
        if (!m_isTriggered) return;
        m_triggerTime -= Time.deltaTime;
        if (m_triggerTime<=0)
        {
            Stop();
        }
        OnSkillUpdate?.Invoke();
    }

    void ReduceCooldownTime()
    {
        if (!m_isCooldowning) return;
        m_cooldownTime-= Time.deltaTime;
        OnCooldown?.Invoke();
        if (m_cooldownTime > 0) return;
        m_isCooldowning = false;
        OnCooldownStop?.Invoke();
        m_cooldownTime = skillStat.cooldownTime;
    }
    public void Stop()
    {
        m_triggerTime = skillStat.timerTrigger;
        m_isTriggered=false;
        OnStopWithType?.Invoke(skillName,1);
        OnStop?.Invoke();
    }
    public void ForceStop()
    {
        m_isCooldowning=false;
        m_isTriggered=false;
        LoadStat();
    }
}
