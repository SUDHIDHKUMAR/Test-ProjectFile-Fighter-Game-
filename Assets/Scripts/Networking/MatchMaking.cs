using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using FighterGame.UI;

public class MatchMaking : Fusion.Behaviour, INetworkRunnerCallbacks
{
    NetworkRunner runner;
    [SerializeField] string lobbyName = "Test";
    public static Action OnGettingOpponent;
    public async void StartHost()
    {
        var customProps = new Dictionary<string, SessionProperty>
        {
            ["team"] = (int)PlayerData.teamChoosed
        };

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            CustomLobbyName = lobbyName,
            PlayerCount = 2,
            SessionProperties = customProps,
            Scene = SceneManager.GetActiveScene().buildIndex,
        });

        if (result.Ok)
        {
            Debug.Log("Created Host");
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }
    public async void StartClient(GameTeam gameTeam)
    {
        var customProps = new Dictionary<string, SessionProperty>() {
        { "team", (int)gameTeam } };

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            CustomLobbyName = lobbyName,
            SessionProperties = customProps,
            Scene = SceneManager.GetActiveScene().buildIndex,
        });
        if (result.Ok)
        {
            Debug.Log("Joined a session");
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("Connected to Server"); }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.Log("Connectection Failed"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("Disconnect from Server"); }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {/*
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 0, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);
        }*/
        Debug.Log("PlayerJoined");
        if (runner.SessionInfo.PlayerCount == runner.SessionInfo.MaxPlayers)
        {
            Debug.Log("startGame");
            OnGettingOpponent?.Invoke();
        }

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        /* // Find and remove the players avatar
         if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
         {
             runner.Despawn(networkObject);
             _spawnedCharacters.Remove(player);
         }*/
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

        Debug.Log($"Session List Updated with {sessionList.Count} session(s)");

        SessionInfo session = null;

        foreach (var sessionItem in sessionList)
        {
            if (sessionItem.PlayerCount != sessionItem.MaxPlayers)
            {
                // Check for a specific Custom Property
                if (sessionItem.Properties.TryGetValue("team", out var propertyType) && propertyType.IsInt)
                {

                    var gameTeam = (int)propertyType.PropertyValue;

                    // Check for the desired Game Type
                    if (gameTeam != (int)PlayerData.teamChoosed)
                    {
                        // Store the session info
                        session = sessionItem;
                        break;
                    }
                }
            }
        }

        // Check if there is any valid session
        if (session != null)
        {
            Debug.Log($"Joining {session.Name}");

            // Join
            runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client, // Client GameMode, could be Shared as well
                SessionName = session.Name, // Session to Join
                                            // ...
            });
        }
        else
        {
            Debug.LogError("No Session Found... Starting to Host");
            StartHost();
        }
    }
    public async void JoinLobby()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        var result = await runner.JoinSessionLobby(SessionLobby.Custom, lobbyName);

        if (result.Ok)
        {
            // all good
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    private void OnEnable()
    {
        ChooseTeamScreen.onTeamSelected += () =>
        {
            JoinLobby();
        };
    }
}
