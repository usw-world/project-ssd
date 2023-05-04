using UnityEngine;

public partial class DebuggingNetworkManager : SSDNetworkManager {
    public System.Action onStartHost;

    public enum TestTarget { TPlayer, QPlayer }
    public TestTarget testTarget = TestTarget.TPlayer;
    
    public override void Awake() {
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
        isHost = true;
    }
    
    public override void OnClientConnect() { // both host and guest.
        Mirror.NetworkClient.Ready();
        InitilizeHandler();
        onStartHost?.Invoke();
    }
}