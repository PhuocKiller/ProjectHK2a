using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlayerInfo : NetworkBehaviour
{
    public RoomGame roomGame;

    public override void Spawned()
    {
        base.Spawned();
        roomGame= FindObjectOfType<RoomGame>();
    }

    [Networked] public string playerID { get; set; }
    [Networked(OnChanged = nameof(PlayerTeamChanged))] public int playerTeam { get; set; }
    protected static void PlayerTeamChanged(Changed<RoomPlayerInfo> changed)
    {
        changed.Behaviour.StartCoroutine(changed.Behaviour.roomGame.DelayUpdateUI());
        if(changed.Behaviour.HasStateAuthority) FindObjectOfType<NetworkManager>().playerTeam= changed.Behaviour.playerTeam;
    }
    [Networked(OnChanged = nameof(PlayerIndexChanged))] public int playerIndex { get; set; }
    protected static void PlayerIndexChanged(Changed<RoomPlayerInfo> changed)
    {
        changed.Behaviour.StartCoroutine(changed.Behaviour.roomGame.DelayUpdateUI());
        if (changed.Behaviour.HasStateAuthority) FindObjectOfType<NetworkManager>().playerIndex = changed.Behaviour.playerIndex;
    }
}
