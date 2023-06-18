using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;
using C2SMessage;
using S2CMessage;

public partial class SSDNetworkManager : NetworkManager {
    public Dictionary<System.Type, List<System.Action<NetworkMessage>>> messageHandlerMap = new Dictionary<System.Type, List<System.Action<NetworkMessage>>>();
    public void RegisterHandler<T>(System.Action<NetworkMessage> action) where T : struct, NetworkMessage {
        System.Type type = typeof(T);
        List<System.Action<NetworkMessage>> list;
        
        if(!messageHandlerMap.ContainsKey(type)) {
            list = new List<System.Action<NetworkMessage>>();
            messageHandlerMap.Add(type, list);
            
            NetworkClient.RegisterHandler<T>((message) => {
                for(int i=0; i<list.Count; i++) {
                    list[i]?.Invoke(message);
                }
            });
        } else {
            list = messageHandlerMap[type];
        }
        list.Add(action as System.Action<NetworkMessage>);
    }
    public void UnregisterHandler<T>(System.Action<NetworkMessage> action) where T : struct, NetworkMessage {
        System.Type type = typeof(T);
        var list = messageHandlerMap[type];
        list.Remove(action as System.Action<NetworkMessage>);
        if(list.Count <= 0) {
            messageHandlerMap.Remove(type);
            NetworkClient.UnregisterHandler<T>();
        }
    }

    #region Message Handlers
    private void OnCreateTPlayerPrefab(NetworkConnectionToClient conn, CreateTPlayerPrefabMessage message) {
        if(StageManager.instance != null)
            player = Instantiate(tPlayerObject, StageManager.instance.spawnPoint.position, StageManager.instance.spawnPoint.rotation);
        else
            player = Instantiate(tPlayerObject);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
    private void OnCreateQPlayerPrefab(NetworkConnectionToClient conn, CreateQPlayerPrefabMessage message) {
        if(StageManager.instance != null)
            player = Instantiate(qPlayerObject, StageManager.instance.spawnPoint.position, StageManager.instance.spawnPoint.rotation);
        else
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