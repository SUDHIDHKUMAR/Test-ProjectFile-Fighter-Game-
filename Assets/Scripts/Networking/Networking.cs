using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using FighterGame.UI;
using System.Linq;

public class Networking : Fusion.Behaviour, INetworkRunnerCallbacks
{
    public static Action OnGettingOpponent;
    public static Action OnDisConnect;
    public static Action<PlayerSide> OnPlayerLeftGame;

    [SerializeField] string lobbyName = "Test";
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] NetworkPrefabRef _playerPrefab;
    [SerializeField] GameInput gameInput;

    NetworkRunner runner;
    Dictionary<PlayerRef, Character> _spawnedCharacters = new Dictionary<PlayerRef, Character>();

    public static Networking Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Connect()
    {
        _spawnedCharacters.Clear();

        if (runner != null) return;

        runner = Instantiate(networkRunnerPrefab, transform);
        runner.AddCallbacks(this);
    }

    public void Disconnect()
    {
        Debug.Log($"Disconnect Called");
        if (runner == null) return;

        runner.Shutdown();
    }
    public async void StartHost()
    {
        var customProps = new Dictionary<string, SessionProperty>
        {
            ["team"] = (int)Utility.teamChoosed
        };

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            CustomLobbyName = lobbyName,
            PlayerCount = 2,
            SessionProperties = customProps,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("Created Host");
            Utility.side = PlayerSide.Left;

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
            Scene = SceneManager.GetActiveScene().buildIndex,                 // ...
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
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
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(gameInput.GetAllInputs());
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.SessionInfo.PlayerCount == runner.SessionInfo.MaxPlayers)
        {
            OnGettingOpponent?.Invoke();
        }
        SpawnCharacter(player);

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out Character networkObject))
        {
            OnPlayerLeftGame?.Invoke(networkObject.side);
            foreach (var chara in _spawnedCharacters)
            {
                runner.Despawn(chara.Value.GetBehaviour<NetworkObject>());
            }
            _spawnedCharacters.Clear();
            Disconnect();
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

        Debug.Log($"Session List Updated with {sessionList.Count} session(s)");

        SessionInfo session = null;

        foreach (var sessionItem in sessionList)
        {
            if (sessionItem.PlayerCount != sessionItem.MaxPlayers)
            {
                if (sessionItem.Properties.TryGetValue("team", out var propertyType) && propertyType.IsInt)
                {
                    var gameTeam = (int)propertyType.PropertyValue;

                    if (gameTeam != (int)Utility.teamChoosed)
                    {
                        session = sessionItem;
                        break;
                    }
                }
            }
        }
        if (session != null)
        {
            runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = session.Name,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
            Utility.side = PlayerSide.Right;
        }
        else
        {
            Debug.LogError("No Session Found... Starting to Host");
            StartHost();
        }
    }
    public async void JoinLobby()
    {
        //runner = gameObject.AddComponent<NetworkRunner>();
        Connect();
        runner.ProvideInput = true;
        var result = await runner.JoinSessionLobby(SessionLobby.Custom, lobbyName);

        if (result.Ok)
        {
            //StartHost();
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    void SpawnCharacter(PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPosition;
            Quaternion rot;
            bool isMaster = false;
            if (runner.SessionInfo.PlayerCount != 1)
            {
                spawnPosition = new Vector3(1.5f, 0, 0);
                rot = Quaternion.Euler(new Vector3(0, -90, 0));
            }
            else
            {
                spawnPosition = new Vector3(-1.5f, 0, 0);
                rot = Quaternion.Euler(new Vector3(0, 90, 0));
                isMaster = true;

            }
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, rot, player);
            Character chara = networkPlayerObject.gameObject.GetComponent<Character>();
            _spawnedCharacters.Add(player, chara);
            chara.dir = !isMaster ? -1 : 1;
            chara.side = isMaster ? PlayerSide.Left : PlayerSide.Right;
        }
    }
    public Character GetPlayer(PlayerRef playerRef)
    {
        if (runner is null)
        {
            Debug.LogWarning($"GetPlayer returning null, runner is null");
            return null;
        }

        _spawnedCharacters.TryGetValue(playerRef, out var player);

        if (player is null)
            Debug.LogError($"GetPlayer returning null, TryGetValue fail");

        return player;
    }

    public Character GetOtherPlayer(PlayerRef playerRef)
    {
        if (runner is null)
        {
            Debug.LogWarning($"GetOtherPlayer returning null, runner is null");
            return null;
        }

        var kvp = _spawnedCharacters.FirstOrDefault(p => p.Key != playerRef);

        if (kvp.Value is null)
            Debug.LogError($"GetOtherPlayer returning null, kvp.Value is null");

        return kvp.Value;
    }
    public Character GetPlayerBySide(PlayerSide playerSide)
    {
        if (runner is null)
        {
            Debug.LogWarning($"GetPlayer returning null, runner is null");
            return null;
        }

        var player = _spawnedCharacters.FirstOrDefault(p => p.Value.side == playerSide);

        if (player.Value is null)
            Debug.LogError($"GetPlayer returning null, FirstOrDefault fail");

        return player.Value;
    }
    public Character GetOtherPlayerBySide(PlayerSide playerSide)
    {
        if (runner is null)
        {
            Debug.LogWarning($"GetOtherPlayer returning null, runner is null");
            return null;
        }

        var kvp = _spawnedCharacters.FirstOrDefault(p => p.Value.side != playerSide);

        if (kvp.Value is null)
            Debug.LogError($"GetOtherPlayer returning null, kvp.Value is null");

        return kvp.Value;
    }
    private void OnEnable()
    {
        ChooseTeamScreen.onTeamSelected += JoinLobby;

    }
    private void OnDisable()
    {
        ChooseTeamScreen.onTeamSelected -= JoinLobby;
    }
    #region CallBacks
    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("Connected to Server"); }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.Log("Connectection Failed"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnect from Server");
        OnDisConnect?.Invoke();
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
}
