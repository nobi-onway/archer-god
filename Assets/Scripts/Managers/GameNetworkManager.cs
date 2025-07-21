using System;
using System.Collections.Generic;
using Netick;
using Netick.Unity;
using UnityEngine;

public enum ETeam { LEFT = 1, RIGHT = 0 }

public class GameNetworkManager : NetworkEventsListener
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Transform _leftTeamSpawnPoint, _rightTeamSpawnPoint;

    public override void OnStartup(NetworkSandbox sandbox)
    {
        if (!sandbox.IsServer) return;

        sandbox.InitializePool(sandbox.GetPrefab("Arrow"), 5);
    }

    public override void OnInput(NetworkSandbox sandbox)
    {
        PlayerCharacterInput playerCharacterInput = sandbox.GetInput<PlayerCharacterInput>();

        playerCharacterInput.IsLeftKeyHeld = Input.GetKey(KeyCode.LeftArrow);
        playerCharacterInput.IsRightKeyHeld = Input.GetKey(KeyCode.RightArrow);
        playerCharacterInput.IsLeftKeyUp = Input.GetKeyUp(KeyCode.LeftArrow);
        playerCharacterInput.IsRightKeyUp = Input.GetKeyUp(KeyCode.RightArrow);

        sandbox.SetInput(playerCharacterInput);
    }

    public override void OnPlayerJoined(NetworkSandbox sandbox, NetworkPlayerId player)
    {
        if (!sandbox.IsServer) return;

        MainCharacter mainCharacter = sandbox.NetworkInstantiate(_playerPrefab, player).GetComponent<MainCharacter>();

        ETeam team = (ETeam)Enum.GetValues(typeof(ETeam)).GetValue(sandbox.Players.Count % 2);
        Vector3 spawnPos = team == ETeam.LEFT ? _leftTeamSpawnPoint.position : _rightTeamSpawnPoint.position;

        mainCharacter.Init(team, spawnPos);

        sandbox.SetPlayerObject(player, mainCharacter.GetComponent<NetworkObject>());
        GameManager.Instance.RegisterPlayer(sandbox, player);
    }
}

public struct PlayerCharacterInput : INetworkInput
{
    public bool IsLeftKeyHeld;
    public bool IsRightKeyHeld;
    public bool IsLeftKeyUp;
    public bool IsRightKeyUp;
}