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
    WaitBeforeStart,
    InGame,
}
public class GameManager : NetworkBehaviour
{
    [Networked(OnChanged = nameof(CurrentStateChanged))]
    private int currentState { get; set; }
    public ClockManager clock;

   [Networked] public float currentTime { get; set; }
    [Networked] public float startTime { get; set; }
    [Networked] public TickTimer waitBeforeStartTime {  get; set; }
    public override void Spawned()
    {
        base.Spawned();

        currentState = 3;
        clock=FindObjectOfType<ClockManager>();
        waitBeforeStartTime = TickTimer.CreateFromSeconds(Runner, 10f);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
       if(HasStateAuthority && Object.IsValid)
        {
          SyncTime();
        }
       if(waitBeforeStartTime.ExpiredOrNotRunning(Runner) && currentState==3)
        {
            currentState = 4;
            startTime = Time.time;
            Debug.Log("hetgio");
        }
    }

    public GameState TypeOfGameState(int value)
    {
        switch (value)
        {
            case 0: return GameState.None;
            case 1: return GameState.Lobby;
            case 2: return GameState.Transition;
            case 3: return GameState.WaitBeforeStart;
            case 4: return GameState.InGame;
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
    public void SyncTime()
    {
        if (state == GameState.WaitBeforeStart) currentTime= (float)waitBeforeStartTime.RemainingTime(Runner);
        if (state == GameState.InGame)currentTime = Time.time - startTime;
    }
}
