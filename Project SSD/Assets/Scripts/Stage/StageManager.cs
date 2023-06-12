using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
    static public StageManager instance;

    [SerializeField] private GameObject tPlayerCamera;
    [SerializeField] private GameObject qPlayerCamera;

    [SerializeField] private Transform spawnPoint;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start() {
        if(SSDNetworkManager.instance is DebuggingNetworkManager) {
            DebuggingNetworkManager nManager = (DebuggingNetworkManager)SSDNetworkManager.instance;
            nManager.onStartHost += () => {
                if(nManager.testTarget == DebuggingNetworkManager.TestTarget.TPlayer) {
                    var message = new C2SMessage.CreateTPlayerPrefabMessage(spawnPoint);
                    Mirror.NetworkClient.Send(message);
                } else {
                    var message = new C2SMessage.CreateQPlayerPrefabMessage();
                    Mirror.NetworkClient.Send(message);
                }
            };
        } else {
            if(SSDNetworkManager.instance.isHost) {
                var message = new C2SMessage.CreateTPlayerPrefabMessage(spawnPoint);
                Mirror.NetworkClient.Send(message);
            } else {
                var message = new C2SMessage.CreateQPlayerPrefabMessage();
                Mirror.NetworkClient.Send(message);
            }
        }
    }
}