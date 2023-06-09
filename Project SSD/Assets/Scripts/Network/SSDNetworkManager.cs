using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

using Mirror;
using C2SMessage;
using S2CMessage;

public partial class SSDNetworkManager : NetworkManager {
    public static SSDNetworkManager instance;
    
    public bool isHost { get; protected set; } = false;

    public (NetworkConnectionToClient connection, string userName) hostUser;
    public (NetworkConnectionToClient connection, string userName) guestUser;
    
    public GameObject tPlayerObject;
    public GameObject qPlayerObject;

    public GameObject player;
    
    public override void Awake() {
        base.Awake();
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public override void OnStartServer() {
        base.OnStartServer();
        isHost = true;

        NetworkServer.OnDisconnectedEvent += (NetworkConnectionToClient conn) => {
            LobbyManager.instance.guestName = null;
            LobbyManager.instance.RefreshRoom();
        };
    }

    public void LoadScene(string sceneName) {
        ServerChangeScene(sceneName);
    }
    public void HostSession() {
        StartHost();
    }
    public void JoinSession(string address) {
        networkAddress = address;
        StartClient();
    }
    public void CloseSession() {
        if(NetworkServer.active){
            StopHost();
            isHost = false;
        }
        if(NetworkClient.isConnected) {
            NetworkClient.OnDisconnectedEvent -= OnCloseRoom;
            StopClient();
        }
    }

    private void OnCloseRoom() {
        NetworkClient.OnDisconnectedEvent -= OnCloseRoom;
        if(isHost) return;
        AlertUI alert = UIManager.instance.AlertMessage("Server is dead.");
        alert?.onClickConfirmButton.AddListener(() => {
            if(LobbyManager.instance != null) {
                LobbyManager.instance.OpenLobbyUi();
            } else {
                throw new System.NotImplementedException("Server die event handling when Player is not in lobby.");
            }
        });
    }
    public override void OnClientConnect() { // both host and guest.
        base.OnClientConnect();
        InitilizeHandler();
        NetworkClient.OnDisconnectedEvent += OnCloseRoom;

        /* temporary >> */
        string playerName = GameManager.instance.saveData?.userId ?? (isHost ? "host" : "guest");
        /* << temporary */

        LobbyManager.instance.hostName = playerName;
        LobbyManager.instance.guestName = null;
        LobbyManager.instance.OnJoinRoom();
        C2SMessage.JoinRoomMessage message = new C2SMessage.JoinRoomMessage(isHost, playerName);
        NetworkClient.Send(message);
    }
    #region Message Handlers
    protected void InitilizeHandler() {
        #region Server Handler Initialize
        if(isHost) {
            NetworkServer.RegisterHandler<C2SMessage.CreateTPlayerPrefabMessage>(OnCreateTPlayerPrefab);
            NetworkServer.RegisterHandler<C2SMessage.CreateQPlayerPrefabMessage>(OnCreateQPlayerPrefab);
            NetworkServer.RegisterHandler<C2SMessage.JoinRoomMessage>(OnJoinRoom);
            NetworkServer.RegisterHandler<C2SMessage.ChangeStateMessage>(OnChangeState);
        }
        #endregion Server Handler Initialize

        #region Client Handler Initialize
        NetworkClient.RegisterHandler<S2CMessage.ShareUserInformationsMessage>(OnShareUserInformations);
        NetworkClient.RegisterHandler<S2CMessage.SyncEnemyMessage>(OnSyncEnemy);
        NetworkClient.RegisterHandler<S2CMessage.SyncEnemyStateMessage>(OnSyncEnemyState);
        NetworkClient.RegisterHandler<S2CMessage.UnityBallSetTargetMessage>(OnUnityBallSetTarget);
        #endregion Client Handler Initialize
    }
    protected void OnJoinRoom(NetworkConnectionToClient conn, JoinRoomMessage message) {
        if(message.isHost) {
            hostUser = (conn, message.userName);
        } else {
            guestUser = (conn, message.userName);
            S2CMessage.ShareUserInformationsMessage infoMessage = new S2CMessage.ShareUserInformationsMessage(hostUser.userName, guestUser.userName);
            NetworkServer.SendToAll(infoMessage);
        }
    }
    protected void OnShareUserInformations(ShareUserInformationsMessage message) {
        LobbyManager.instance.hostName = message.hostName;
        LobbyManager.instance.guestName = message.guestName;
        LobbyManager.instance.RefreshRoom();
    }
    #endregion Message Handlers
}