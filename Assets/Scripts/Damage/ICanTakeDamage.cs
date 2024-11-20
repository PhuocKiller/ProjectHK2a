using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanTakeDamage
{
    public void ApplyDamage(int damage, bool isPhysicDamage, PlayerRef player,
        Action callback = null, Action<int> isKillPlayer = null, bool activeInjureAnim = true);
    public void ApplyEffect(PlayerRef player, bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float TimeEffect = 0f,  Action callback = null);
}
