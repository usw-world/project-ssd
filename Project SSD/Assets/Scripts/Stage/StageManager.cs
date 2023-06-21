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
    [SerializeField] private AudioClip[] bgmList;

    [SerializeField] private Transform cheatingPoint;
    
    [System.Serializable]
    public class UnityEvent : UnityEngine.Events.UnityEvent{}
    public UnityEvent onClearStage;
    
    public void OnClearStage() {
        var result = new GameManager.GameResult();
        result.exp = 1050;
        GameManager.instance.result = result;
        onClearStage?.Invoke();
        StartCoroutine(ClearCoroutine());
    }
    public IEnumerator ClearCoroutine() {
        float offset = 0;
        while(offset < .8f) {
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

        UIManager.instance.FadeOut(2, 3, () => {
            UIManager.instance.SetActiveHud(false);
            if(SSDNetworkManager.instance.isHost) {
                Destroy(TPlayerCamera.instance.gameObject);
                Destroy(TPlayer.instance.gameObject);
            } else {
                Destroy(QPlayerCamera.instance.gameObject);
                Destroy(QPlayer.instance.gameObject);
            }
            SSDNetworkManager.instance.LoadScene("Lobby Scene");
        });
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

    public void StopBgm() {
        StartCoroutine(StopBgmCoroutine());
    }
    public IEnumerator StopBgmCoroutine() {
        float origin = bgmAudioSource.volume;
        float offset = 0;
        while(offset < 1) {
            offset += Time.deltaTime;
            bgmAudioSource.volume = origin - origin*offset;
            yield return null;
        }
        bgmAudioSource.Stop();
    }
    public void PlayBgm(int index) {
        bgmAudioSource.volume = 0.09f;
        bgmAudioSource.Stop();
        bgmAudioSource.clip = bgmList[index];
        bgmAudioSource.Play();
    }
}