using System.Collections.Generic;
using Netick;
using Netick.Unity;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameNetworkManager : NetworkEventsListener
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Transform _leftTeamSpawnPoint, _rightTeamSpawnPoint;

    private Dictionary<NetworkPlayerId, MainCharacter> PlayerLookUp;

    public override void OnStartup(NetworkSandbox sandbox)
    {
        PlayerLookUp = new Dictionary<NetworkPlayerId, MainCharacter>();
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
        if (sandbox.IsClient) return;

        Vector3 spawnPos = sandbox.Players.Count == 1 ? _leftTeamSpawnPoint.position : _rightTeamSpawnPoint.position;

        MainCharacter mainCharacter = sandbox.NetworkInstantiate(_playerPrefab, spawnPos, Quaternion.identity, player).GetComponent<MainCharacter>();
        PlayerLookUp.Add(player, mainCharacter);

        if (sandbox.Players.Count == 2)
        {
            PlayerLookUp[sandbox.Players[0]].Init(PlayerLookUp[sandbox.Players[1]].transform);
            PlayerLookUp[sandbox.Players[1]].Init(PlayerLookUp[sandbox.Players[0]].transform);
        }
    }
}

public struct PlayerCharacterInput : INetworkInput
{
    public bool IsLeftKeyHeld;
    public bool IsRightKeyHeld;
    public bool IsLeftKeyUp;
    public bool IsRightKeyUp;
}