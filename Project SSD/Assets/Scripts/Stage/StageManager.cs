using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
    static public StageManager instance;

    [SerializeField] private GameObject tPlayerCamera;
    [SerializeField] private GameObject qPlayerCamera;

    Transform spawnPoint;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start() {
        if(SSDNetworkManager.instance.isHost) {
            var message = new C2SMessage.CreateTPlayerPrefabMessage(spawnPoint);
            print("Send message (Create T Player)");
            Mirror.NetworkClient.Send(message);
        } else {
            var message = new C2SMessage.CreateQPlayerPrefabMessage();
            print("Send message (Create Q Player)");
            Mirror.NetworkClient.Send(message);
        }
    }
}