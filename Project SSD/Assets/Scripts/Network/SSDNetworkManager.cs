using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;
using C2SMessage;
using S2CMessage;

public class SSDNetworkManager : NetworkManager {
    public static SSDNetworkManager instance;
    
    public bool isHost = false;

    public (NetworkConnectionToClient connection, string userName) hostUser;
    public (NetworkConnectionToClient connection, string userName) guestUser;
    
    public GameObject TPlayerObject;
    public GameObject QPlayerObject;

    public GameObject Player;
    
    public override void Awake() {
        base.Awake();
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        print(offlineScene);
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
            // var message = new S2CMessage.ServerDieMessage("Host closed the room.");
            // NetworkServer.SendToAll(message);
        }
        if(NetworkClient.isConnected) {
            NetworkClient.OnDisconnectedEvent -= OnServerDie;
            StopClient();
            // C2SMessage.LeaveRoomMessage message = new C2SMessage.LeaveRoomMessage(isHost);
            // NetworkClient.Send(message);
        }
    }

    #region Message Handlers
    private void InitilizeHandler() {
        #region Server Handler Initialize
        if(isHost) {
            NetworkServer.RegisterHandler<C2SMessage.CreateTPlayerPrefabMessage>(OnCreateTPlayerPrefab);
            NetworkServer.RegisterHandler<C2SMessage.CreateQPlayerPrefabMessage>(OnCreateQPlayerPrefab);
            NetworkServer.RegisterHandler<C2SMessage.JoinRoomMessage>(OnJoinRoom);
            NetworkServer.RegisterHandler<C2SMessage.LeaveRoomMessage>(OnLeaveRoom);
        }
        #endregion Server Handler Initialize

        #region Client Handler Initialize
        NetworkClient.RegisterHandler<S2CMessage.ShareUserInformations>(OnShareUserInformations);
        NetworkClient.RegisterHandler<S2CMessage.AllowLeaveRoomMessage>(OnAllowLeaveRoom);
        #endregion Client Handler Initialize
    }
    private void OnServerDie() {
        NetworkClient.OnDisconnectedEvent -= OnServerDie;
        if(isHost) return;
        GameObject gobj = UIManager.instance.AlertMessage();
        AlertUI alert = gobj.GetComponent<AlertUI>();
        alert?.SetMessage("Server is dead.");
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
        NetworkClient.OnDisconnectedEvent += OnServerDie;
        
        var lobbyManager = LobbyManager.instance;

        /* temporary >> */
        string playerName = isHost ? "host" : "guest";
        /* << temporary */

        LobbyManager.instance.hostName = playerName;
        LobbyManager.instance.guestName = null;
        LobbyManager.instance.OnJoinRoom();
        C2SMessage.JoinRoomMessage message = new C2SMessage.JoinRoomMessage(isHost, playerName);
        NetworkClient.Send(message);
    }

    private void OnJoinRoom(NetworkConnectionToClient conn, JoinRoomMessage message) {
        if(message.isHost) {
            hostUser = (conn, message.userName);
        } else {
            guestUser = (conn, message.userName);
            
            S2CMessage.ShareUserInformations userInformations = new S2CMessage.ShareUserInformations(hostUser.userName, guestUser.userName);
            NetworkServer.SendToAll(userInformations);
        }
    }
    private void OnLeaveRoom(NetworkConnectionToClient conn, LeaveRoomMessage message) {
        if(message.isHost) {
            LobbyManager.instance.guestName = null;
            LobbyManager.instance.RefreshRoom();
        } else {
            conn.Send(new AllowLeaveRoomMessage());
        }
    }
    private void OnAllowLeaveRoom(AllowLeaveRoomMessage message) {
        StopClient();
        LobbyManager.instance.OpenLobbyUi();
    }
    private void OnCreateTPlayerPrefab(NetworkConnectionToClient conn,  CreateTPlayerPrefabMessage message) {
        // Player = Instantiate(TPlayerObject);
        // NetworkServer.AddPlayerForConnection(conn, Player);
    }
    private void OnCreateQPlayerPrefab(NetworkConnectionToClient conn,  CreateQPlayerPrefabMessage message) {
        // Player = Instantiate(QPlayerObject);
        // NetworkServer.AddPlayerForConnection(conn, Player);
    }
    private void OnShareUserInformations(ShareUserInformations message) {
        LobbyManager.instance.hostName = message.hostName;
        LobbyManager.instance.guestName = message.guestName;
        LobbyManager.instance.RefreshRoom();
    }
    #endregion Message Handlers
}