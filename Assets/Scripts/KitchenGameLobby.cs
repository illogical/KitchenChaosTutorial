using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class KitchenGameLobby : MonoBehaviour
{
    public static KitchenGameLobby Instance { get; private set; }

    private Lobby joinedLobby;
    private float heartBeatTimer;
    private const float heartBeatInterval = 15f;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeAuthentication();
    }

    private void Update()
    {
        HandleHeartBeat();
    }

    /// <summary>
    /// Prevents the lobby from timing out (30 seconds by default)
    /// </summary>
    private async void HandleHeartBeat()
    {
        if(IsLobbyHost())
        {
            heartBeatTimer -= Time.deltaTime;
            if(heartBeatTimer <= 0)
            {
                heartBeatTimer = heartBeatInterval;
                
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost() => joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;

    private async void InitializeAuthentication()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 10000).ToString());    // needed for the same PC to have multiple game sessions
            
            await UnityServices.InitializeAsync(initializationOptions);
        }

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYERS, new CreateLobbyOptions()
            {
                IsPrivate = isPrivate
            });

            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
        
    }

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }

    }

    public Lobby GetLobby() => joinedLobby;

}
