
using UnityEngine;

using Mirror;
using C2SMessage;
using S2CMessage;

public partial class DebuggingNetworkManager : NetworkManager {
    public static DebuggingNetworkManager instance;
    
    public bool isHost { get; private set; } = true;
    
    public GameObject tPlayerObject;
    public GameObject qPlayerObject;

    public System.Action onStartHost;

    public enum TestTarget { TPlayer, QPlayer }
    public TestTarget testTarget = TestTarget.TPlayer;
    
    public override void Awake() {
        base.Awake();
        if(SSDNetworkManager.instance != null)
            Destroy(this.gameObject);
        
        if(DebuggingNetworkManager.instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    public override void Start() {
        base.Start();
        StartHost();
    }

    public override void OnStartServer() {
        base.OnStartServer();
    }

    public void LoadScene(string sceneName) {
        ServerChangeScene(sceneName);
    }
    public override void OnClientConnect() { // both host and guest.
        base.OnClientConnect();
        InitilizeHandler();
        onStartHost?.Invoke();
    }
    #region Message Handlers
    private void InitilizeHandler() {
        #region Server Handler Initialize
        if(isHost) {
            NetworkServer.RegisterHandler<C2SMessage.CreateTPlayerPrefabMessage>(OnCreateTPlayerPrefab);
            NetworkServer.RegisterHandler<C2SMessage.CreateQPlayerPrefabMessage>(OnCreateQPlayerPrefab);
        }
        #endregion Server Handler Initialize

        #region Client Handler Initialize
        /*  */
        #endregion Client Handler Initialize
    }
    private void OnCreateTPlayerPrefab(NetworkConnectionToClient conn,  CreateTPlayerPrefabMessage message) {
        NetworkServer.AddPlayerForConnection(conn, Instantiate(tPlayerObject));
    }
    private void OnCreateQPlayerPrefab(NetworkConnectionToClient conn,  CreateQPlayerPrefabMessage message) {
        NetworkServer.AddPlayerForConnection(conn,  Instantiate(qPlayerObject));
    }
    #endregion Message Handlers
}