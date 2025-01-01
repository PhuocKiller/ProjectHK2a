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
    public int currentState { get; set; }
    public ClockManager clock;
    List<PlayerController> playerControllers = new List<PlayerController>();
    [Networked] public float currentTime { get; set; }
    [Networked] public TickTimer waitBeforeStartTime { get; set; }
    [Networked] public TickTimer transitionTime { get; set; }
    [Networked] public int levelCreep { get; set; }
    public Action reachMarkTime;
    bool flag; //đánh dầu có vào waitbeforestart
    [Networked(OnChanged = nameof(PlayerInRoomChange)), Capacity(6)] public NetworkArray<string> playersInRoom { get; }
    [Networked(OnChanged = nameof(KeyPlayerChanged))] public string keyPlayer { get; set; }
    [Networked(OnChanged = nameof(TimeChanged))] public int moonLightTime { get; set; }
    public override void Spawned()
    {
        base.Spawned();
        currentState = 1;
        clock = FindObjectOfType<ClockManager>();
        levelCreep = 0;
        if(HasStateAuthority) keyPlayer = Runner.UserId;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (HasStateAuthority && Object.IsValid)
        {
            SyncTime();
        }
        if (currentState == 2)
        {
            if (transitionTime.ExpiredOrNotRunning(Runner))
            {
                GoWaitBeforeStartState();
                FindObjectOfType<RoomGame>().ControlCooldownTimeBeforePlay("0", false);
            }
            else
            {
                FindObjectOfType<RoomGame>().ControlCooldownTimeBeforePlay(((int)transitionTime.RemainingTime(Runner)).ToString(), true);
            }
        }
        if (waitBeforeStartTime.ExpiredOrNotRunning(Runner) && currentState == 3)
        {
            moonLightTime = 0;
            currentState = 4;
            currentTime = 0;
            levelCreep = 1;
            FindObjectOfType<NetworkManager>().SpawnCreep(Runner.LocalPlayer);
        }
    }
    public void GoTransitionState()
    {
        currentState = 2;
        transitionTime = TickTimer.CreateFromSeconds(Runner, 3.2f); //thời gian đếm ngược trước khi playgame
    }
    public void GoWaitBeforeStartState()
    {
        currentState = 3;
        waitBeforeStartTime = TickTimer.CreateFromSeconds(Runner, 6f); //thời gian chuẩn bị mua đồ
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
        if (newState == GameState.WaitBeforeStart)
        {
            changed.Behaviour.flag = true;
            changed.Behaviour.StartGame();
            FindObjectOfType<NetworkManager>().SpawnPlayer(changed.Behaviour.Runner, changed.Behaviour.Runner.LocalPlayer);
        }
        if (newState == GameState.InGame)
        {

            if (!changed.Behaviour.flag)
            {
                changed.Behaviour.StartGame();
                FindObjectOfType<NetworkManager>().SpawnPlayer(changed.Behaviour.Runner, changed.Behaviour.Runner.LocalPlayer);
            }
        }
    }
    protected static void PlayerInRoomChange(Changed<GameManager> changed)
    {
        bool isHaveThisPlayer = false;
        for (int i = 0; i < 6; i++)
        {
            if (changed.Behaviour.playersInRoom.Get(i) == changed.Behaviour.Runner.GetPlayerUserId(changed.Behaviour.Runner.LocalPlayer))
            {
                isHaveThisPlayer = true;
            }
        }
        if (!isHaveThisPlayer) FindObjectOfType<RoomGame>().backBtn.onClick?.Invoke();
    }
    protected static void KeyPlayerChanged(Changed<GameManager> changed)
    {
        if (changed.Behaviour.keyPlayer == changed.Behaviour.Runner.GetPlayerUserId(changed.Behaviour.Runner.LocalPlayer))
        {
            changed.Behaviour.GetComponent<NetworkObject>().RequestStateAuthority();
        }
        changed.Behaviour.StartCoroutine(FindObjectOfType<RoomGame>().DelayUpdateUI());
    }
    protected static void TimeChanged(Changed<GameManager> changed)
    {
    }
    
    void StartGame()
    {
        FindObjectOfType<NetworkManager>().onConnected?.Invoke();
        RenderSettings.fog = true;
        RoomGame roomGame = FindObjectOfType<RoomGame>();
        if (roomGame) roomGame.gameObject.SetActive(false);
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
        if (state == GameState.WaitBeforeStart) currentTime = (float)waitBeforeStartTime.RemainingTime(Runner);
        if (state == GameState.InGame)
        {
            currentTime += Runner.DeltaTime;
            if (Mathf.FloorToInt(currentTime) / 30 > (levelCreep - 1))
            {
                levelCreep++;
                FindObjectOfType<NetworkManager>().SpawnCreep(Runner.LocalPlayer);
                reachMarkTime?.Invoke();
            };
            moonLightTime = Mathf.FloorToInt(currentTime / 30) % 2;
        }
    }
    public void AddPlayerWhenJoin(NetworkRunner m_runner, PlayerRef player)
    {
        for (int i = 0; i < 6; i++)
        {
            if (playersInRoom.Get(i) == "")
            {
                playersInRoom.Set(i, m_runner.GetPlayerUserId(player));
                return;
            }
        }
    }
    public void RemovePlayerWhenLeave(NetworkRunner m_runner, PlayerRef player)
    {
        for (int i = 0; i < 6; i++)
        {
            if (i == player.PlayerId)
            {
                playersInRoom.Set(i, "");
                return;
            }
        }
        StartCoroutine(ChangeKeyWhenLeaveMySelf());
    }
    public IEnumerator ChangeKeyWhenLeaveMySelf()
    {
        yield return new WaitForSeconds(0.5f);
        if (HasStateAuthority)
        {
            Debug.Log("a");
            keyPlayer = Runner.GetPlayerUserId(Runner.LocalPlayer);
        }
    }
}
