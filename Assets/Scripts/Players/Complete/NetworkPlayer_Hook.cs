using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class NetworkPlayer_Hook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        NetworkPlayer networkPlayer = gamePlayer.GetComponent<NetworkPlayer>();

        networkPlayer.Initialize(lobby.playerTeam, lobby.playerColor, lobby.playerName);
    }
}
