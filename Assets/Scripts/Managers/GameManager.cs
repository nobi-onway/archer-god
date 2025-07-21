using System.Collections.Generic;
using Netick;
using Netick.Unity;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private Dictionary<int, MainCharacter> _players;
    private Dictionary<int, MainCharacter> Players => _players ??= new();

    public void RegisterPlayer(NetworkSandbox sandbox, NetworkPlayerId player)
    {
        if (Players.ContainsKey(player.Id)) return;

        Players[player.Id] = sandbox.GetPlayerObject<MainCharacter>(player);
    }

    public Transform GetOpponent(ETeam team)
    {
        foreach (MainCharacter player in Players.Values)
        {
            if (player.Team != team) return player.transform;
        }

        return null;
    }
}