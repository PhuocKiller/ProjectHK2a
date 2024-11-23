using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None,
    Lobby,
    Transition,
    InGame,
}
public class GameManager : NetworkBehaviour
{
    [Networked(OnChanged = nameof(CurrentStateChanged))]
    private int currentState { get; set; }
    public ClockManager clock;

   [Networked] public float currentTime { get; set; }
    [Networked] public float startTime { get; set; }
    public override void Spawned()
    {
        base.Spawned();

        currentState = (int)GameState.Lobby;
        
        clock=FindObjectOfType<ClockManager>();
        startTime=Time.time;
        currentTime=startTime;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
       if(HasStateAuthority && Object.IsValid)
        {
          SyncTime();
        }
    }

    public GameState TypeOfGameState(int value)
    {
        switch (value)
        {
            case 0: return GameState.None;
            case 1: return GameState.Lobby;
            case 2: return GameState.Transition;
            case 3: return GameState.InGame;
            default: return GameState.None;
        }
    }
    public GameState state
    {
        get
        {
            if (Object.IsValid)
            {
                return TypeOfGameState(currentState);
            }
            else
            {
                return GameState.None;
            }
        }
    }

    private Action<GameState, GameState> onCurrentStateChanged;

    protected static void CurrentStateChanged(Changed<GameManager> changed)
    {
        changed.LoadOld();
        GameState oldState = changed.Behaviour.TypeOfGameState(changed.Behaviour.currentState);
        changed.LoadNew();
        GameState newState = changed.Behaviour.TypeOfGameState(changed.Behaviour.currentState);
        changed.Behaviour.onCurrentStateChanged?.Invoke(oldState, newState);
    }
    public void RegisterOnGameStateChanged(Action<GameState, GameState> listener)
    {
        onCurrentStateChanged += listener;
    }
    public void UnRegisterOnGameStateChanged(Action<GameState, GameState> listener)
    {
        onCurrentStateChanged -= listener;
    }
    
    public void SwitchState(GameState state)
    {
        currentState = (int)state;
    }
    

    //[Rpc(RpcSources.All, RpcTargets.All)]
    public void SyncTime()
    {
        currentTime = Time.time - startTime;
    }
}
