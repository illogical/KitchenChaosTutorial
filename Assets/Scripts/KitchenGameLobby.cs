using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameLobby : MonoBehaviour
{
    public static KitchenGameLobby Instance { get; private set; }

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinFailed;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> LobbyList;
    }

    private Lobby joinedLobby;
    private float heartBeatTimer;
    private float listLobbiesTimer;
    private const float heartBeatInterval = 15f;
    private const float listLobbyInterval = 5f;

    private const string KEY_RELAY_JOIN_CODE = "relayJoinCode";

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeAuthentication();
    }

    private void Update()
    {
        HandleHeartBeat();
        HandlePeriodicListLobbies();
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

    private void HandlePeriodicListLobbies()
    {
        if(joinedLobby != null || !AuthenticationService.Instance.IsSignedIn || SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString())
        {
            return;
        }

        listLobbiesTimer -= Time.deltaTime;
        if (listLobbiesTimer <= 0)
        {
            listLobbiesTimer = listLobbyInterval;

            ListLobbies();
        }
    }

    private bool IsLobbyHost() => joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;

    private async void InitializeAuthentication()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());    // needed for the same PC to have multiple game sessions
            
            await UnityServices.InitializeAsync(initializationOptions);
        }

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYERS - 1); // -1 because the host doesn't need to allocate a relay
            
            return allocation;
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            return await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelayWithLobbyCode(Lobby lobby)
    {
        try
        {
            string joinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            return await JoinRelay(joinCode);
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYERS, new CreateLobbyOptions()
            {
                IsPrivate = isPrivate
            });

            Allocation allocation = await AllocateRelay();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>()
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException ex)
        {
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(ex);
        }
    }

    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            await JoinRelayWithLobbyCode(joinedLobby);

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(ex);
        }
        
    }

    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            await JoinRelayWithLobbyCode(joinedLobby);


            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(ex);
        }

    }

    public async void JoinWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            await JoinRelayWithLobbyCode(joinedLobby);

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(ex);
        }

    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
            {
                Filters = new List<QueryFilter>()
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs() { LobbyList = queryResponse.Results });
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void DeleteLobby()
    {
        if(joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
            }
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if(IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
            }
        }
    }

    public Lobby GetLobby() => joinedLobby;

}
