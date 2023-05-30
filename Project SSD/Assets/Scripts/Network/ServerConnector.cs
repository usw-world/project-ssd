using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerConnector : MonoBehaviour
{
    public static ServerConnector instance;
    [SerializeField] private string networkStatusText = "none";
    [SerializeField] private bool showNetworkStatus = true;
    
    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    // private string apiUrl = "https://uwu-web.azurewebsites.net";
    private string apiUrl = "http://localhost:5034";

    public void Login(string id, string password, Action callback=null, Action<string> errorCallback=null) {
        string json = $"{{\"user_id\":\"{id}\",\"user_pw\":\"{password}\"}}";
        StartCoroutine(LoginCoroutine(json, callback, errorCallback));
    }
    private IEnumerator LoginCoroutine(string postData, Action callback, Action<string> errorCallback=null) {
        byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(postData.ToString());

        UnityWebRequest request = UnityWebRequest.Post(apiUrl+"/login", "POST");
        // POST 요청 생성

        request.uploadHandler.Dispose();
        // 메모리 누수 관련 요거 스캐너 쓸때처럼 dispose 해줘야함
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postDataBytes); 
        // 요청 바디에 JSON 데이터 추가
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); 
        // 응답 수신 방식 지정
        yield return request.SendWebRequest(); 
        // 요청 보내기

        if(request.responseCode == 200) {
            string jsonResponse = request.downloadHandler.text;

            SaveDataVO saveData = new SaveDataVO(jsonResponse);
            GameManager.instance?.SetSaveData(saveData);

            callback?.Invoke();
        } else if(request.responseCode == 400) {
            errorCallback?.Invoke("아이디 또는 패스워드가 일치하지 않습니다.");
        } else if(request.responseCode == 500) {
            errorCallback?.Invoke("서버가 요청을 처리할 수 없습니다.");
        }
        request.Dispose();
    }
    private void OnGUI() {
        if(showNetworkStatus)
            GUI.Label(Rect.zero, networkStatusText);
    }

    public void Register(string id, string password, Action callback=null, Action<string> errorCallback=null) {
        string json = $"{{\"user_id\":\"{id}\",\"user_pw\":\"{password}\"}}";
        StartCoroutine(RegisterCoroutine(json, callback, errorCallback));
    }
    private IEnumerator RegisterCoroutine(string postData, Action callback=null, Action<string> errorCallback=null) {
        // string postData = "{\"status\": \"test\" }"; 
        // // POST로 보낼 JSON 데이터
        Debug.Log(postData);
        byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(postData.ToString());
        UnityWebRequest request = UnityWebRequest.Post(apiUrl+"/register", "POST");
        // POST 요청 생성
        request.uploadHandler.Dispose();
        // 메모리 누수 관련 요거 스캐너 쓸때처럼 dispose 해줘야함
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postDataBytes); 
        // 요청 바디에 JSON 데이터 추가
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); 
        // 응답 수신 방식 지정
        yield return request.SendWebRequest(); 
        // 요청 보내기

        if (request.responseCode == 200) {
            string jsonResponse = request.downloadHandler.text; 
            print(jsonResponse);
            
            SaveDataVO saveData = new SaveDataVO();
            saveData = JsonUtility.FromJson<SaveDataVO>(jsonResponse);
            GameManager.instance.SetSaveData(saveData);

            Debug.Log(saveData.ToString());
            callback?.Invoke();
            print("Success Register.");
        }
        else
        {
            Debug.LogError(request.responseCode);
        }

        request.Dispose();
        // 위와 동일
    }
    
    private IEnumerator SendIngameDataCoroutine(string postData) {
        Debug.Log(postData);
        byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(postData.ToString());
        UnityWebRequest request = UnityWebRequest.Post(apiUrl+"/update", "POST");
        request.uploadHandler.Dispose();
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postDataBytes); 
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); 
        yield return request.SendWebRequest(); 
        if (request.responseCode == 200)
        {
            Debug.Log("Update Success");
        }
        else
        {
            Debug.LogError(request.responseCode);
        }

        request.Dispose();
        // 위와 동일
    }
    
    private void RequestIngameData(Action<string> errorCallback=null) {
        string userToken = GameManager.instance.saveData.token;
        if(userToken != null) {
            StartCoroutine(RequestIngameDataCoroutiune(userToken));
        } else {
            errorCallback?.Invoke("사용자 인증에 실패하였습니다.");
        }
    }
    private IEnumerator RequestIngameDataCoroutiune(string userToken) {
        byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(userToken);
        UnityWebRequest request = UnityWebRequest.Post(apiUrl+"/get-data", "POST");
        request.uploadHandler.Dispose();
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postDataBytes); 
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); 
        yield return request.SendWebRequest(); 
        if (request.responseCode == 200) {
            string jsonResponse = request.downloadHandler.text;
            SaveDataVO saveData = new SaveDataVO();
            saveData.Write(jsonResponse);
            saveData = JsonUtility.FromJson<SaveDataVO>(jsonResponse);
            Debug.Log("download Success");
        } else {
            Debug.LogError(request.responseCode);
        }

        request.Dispose();
        // 위와 동일
    }
}
