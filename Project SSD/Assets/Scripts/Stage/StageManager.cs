using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
    static public StageManager instance;

    [SerializeField] private GameObject tPlayerCamera;
    [SerializeField] private GameObject qPlayerCamera;

    [SerializeField] public Transform spawnPoint;
    
    [SerializeField] public AudioSource bgmAudioSource;

    [SerializeField] private Transform cheatingPoint;
    
    [System.Serializable]
    public class UnityEvent : UnityEngine.Events.UnityEvent{}
    public UnityEvent onClearStage;
    
    public void OnClearStage() {
        onClearStage?.Invoke();
        StartCoroutine(ClearCoroutine());
    }
    public IEnumerator ClearCoroutine() {
        float offset = 0;
        while(offset < 1) {
            offset += Time.deltaTime;
            Time.timeScale = 1-offset;
            yield return null;
        }
        while(offset > 0) {
            offset -= Time.deltaTime;
            Time.timeScale = 1-offset;
            yield return null;
        }
        Time.timeScale = 1;
    }

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        UIManager.instance?.SetActiveHud(true);
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

    public void Update() {
        if(Input.GetKeyDown(KeyCode.Keypad9)) {
            if(TPlayer.instance!=null) TPlayer.instance.transform.position=cheatingPoint.position;
            if(QPlayer.instance!=null) QPlayer.instance.transform.position=cheatingPoint.position;
        }
    }
}