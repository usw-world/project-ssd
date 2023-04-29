using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;
using C2SMessage;
using S2CMessage;

public partial class SSDNetworkManager : NetworkManager {
    #region Message Handlers
    private void OnCreateTPlayerPrefab(NetworkConnectionToClient conn,  CreateTPlayerPrefabMessage message) {
        player = Instantiate(tPlayerObject);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
    private void OnCreateQPlayerPrefab(NetworkConnectionToClient conn,  CreateQPlayerPrefabMessage message) {
        player = Instantiate(qPlayerObject);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
    #endregion Message Handlers
}