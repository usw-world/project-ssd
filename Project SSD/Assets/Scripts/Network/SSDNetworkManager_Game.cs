using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;
using C2SMessage;
using S2CMessage;

public partial class SSDNetworkManager : NetworkManager {
    #region Message Handlers
    private void OnCreateTPlayerPrefab(NetworkConnectionToClient conn, CreateTPlayerPrefabMessage message) {
        player = Instantiate(tPlayerObject);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
    private void OnCreateQPlayerPrefab(NetworkConnectionToClient conn, CreateQPlayerPrefabMessage message) {
        player = Instantiate(qPlayerObject);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
    private void OnSyncEnemy(SyncEnemyMessage message) {
        EnemyManager.instance?.SyncEnemy(message.networkId, message.position, message.rotation);
    }
    private void OnChangeState(NetworkConnectionToClient conn, ChangeStateMessage message) {
        NetworkServer.SendToAll(new S2CMessage.SyncEnemyStateMessage(message.networkId, message.stateName));
    }
    private void OnSyncEnemyState(SyncEnemyStateMessage message) {
        EnemyManager.instance.ChangeEnemyState(message.networkId, message.stateName);
    }
	private void OnUnityBallSetTarget(UnityBallSetTargetMessage message) {
		UnityBall ub = UnityBall.unityBallInScene[message.networkId];
		ub.AddTarget(message.targetNetworkId);
	}
	#endregion Message Handlers
}