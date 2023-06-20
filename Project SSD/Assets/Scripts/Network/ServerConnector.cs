using System;
using System.Collections;

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

    public void Ping(Action callback=null, Action<string> errorCallback=null) {
        StartCoroutine(PingCorourine(callback, errorCallback));
    }
    private IEnumerator PingCorourine(Action callback=null, Action<string> errorCallback=null) {
        UnityWebRequest request = UnityWebRequest.Post(apiUrl+"/ping", "");
        yield return request.SendWebRequest();
        if(request.responseCode == 200) {
            callback?.Invoke();
        } else {
            ErrorMessage error = JsonUtility.FromJson<ErrorMessage>(request.downloadHandler.text);
            if(error == null
            || error.message == ""
            || error.message == null) {
                errorCallback?.Invoke("서버에 연결 할 수 없습니다.");
            } else {
                errorCallback?.Invoke(error.message);
            }
        }
    }
    public void Login(string id, string password, Action callback=null, Action<string> errorCallback=null) {
        string json = $"{{\"user_id\":\"{id}\",\"user_pw\":\"{password}\"}}";
        StartCoroutine(LoginCoroutine(json, callback, errorCallback));
    }
    private IEnumerator LoginCoroutine(string postData, Action callback, Action<string> errorCallback=null) {
        byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(postData.ToString());

        UnityWebRequest request = UnityWebRequest.Post(apiUrl+"/login", "POST");
        // POST 요청 생성

        request.uploadHandler.Dispose(); // 메모리 누수 관련 >> dispose 해줘야함
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(postDataBytes); 
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer(); 
        
        yield return request.SendWebRequest(); 

        string json = request.downloadHandler.text;
        if(request.responseCode == 200) {
            SaveDataVO saveData = new SaveDataVO(json);
            GameManager.instance?.SetSaveData(saveData);
            print(saveData);

            callback?.Invoke();
        } else if(request.responseCode == 400) {
            var error = JsonUtility.FromJson<ErrorMessage>(json);
            errorCallback?.Invoke(error.message);
        } else if(request.responseCode == 500) {
            var error = JsonUtility.FromJson<ErrorMessage>(json);
            errorCallback?.Invoke(error.message);
        }
        request.Dispose();
    }
    public void Register(string id, string password, Action callback=null, Action<string> errorCallback=null) {
        string json = $"{{\"user_id\":\"{id}\",\"user_pw\":\"{password}\"}}";
        StartCoroutine(RegisterCoroutine(json, callback, errorCallback));
    }
    private IEnumerator RegisterCoroutine(string postData, Action callback=null, Action<string> errorCallback=null) {
        byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(postData.ToString());
        UnityWebRequest request = UnityWebRequest.Post(apiUrl+"/register", "POST");
        
        request.uploadHandler.Dispose(); // 메모리 누수 관련 >> dispose 해줘야함
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(postDataBytes); 
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer(); 
        
        yield return request.SendWebRequest(); 

        if (request.responseCode == 200) {
            string jsonResponse = request.downloadHandler.text;
            
            SaveDataVO saveData = new SaveDataVO();
            saveData = JsonUtility.FromJson<SaveDataVO>(jsonResponse);
            GameManager.instance.SetSaveData(saveData);

            Debug.Log(saveData.ToString());
            callback?.Invoke();
        } else {
            try {
                ErrorMessage error = JsonUtility.FromJson<ErrorMessage>(request.downloadHandler.text);
                errorCallback?.Invoke(error.message);
            } catch {
                errorCallback?.Invoke("우리의 힘과 능력으로도 감지할 수 없는 에러가 발생하였습니다.");
            }
        }
        request.Dispose();
    }
    
    public IEnumerator SendIngameDataCoroutine(string postData) {
        Debug.Log(postData);
        byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(postData.ToString());
        UnityWebRequest request = UnityWebRequest.Post(apiUrl+"/update", "POST");
        request.uploadHandler.Dispose();
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(postDataBytes); 
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer(); 
        yield return request.SendWebRequest(); 
        if (request.responseCode == 200) {
            Debug.Log("Update Success");
        } else {
            Debug.LogError(request.responseCode);
        }
        request.Dispose();
    }
    
    public void RequestIngameData(Action<string> errorCallback=null) {
        string json = $"{{\"token\":\"{GameManager.instance.saveData.token}\"}}";
        // string userToken = GameManager.instance.saveData.token;
        // if(userToken != null) {
        if(json != name) {
            StartCoroutine(RequestIngameDataCoroutiune(json));
        } else {
            errorCallback?.Invoke("사용자 인증에 실패하였습니다.");
        }
    }
    private IEnumerator RequestIngameDataCoroutiune(string json) {
        byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = UnityWebRequest.Post(apiUrl+"/get-data", "POST");
        request.uploadHandler.Dispose();
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postDataBytes); 
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); 
        yield return request.SendWebRequest(); 
        if (request.responseCode == 200) {
            string jsonResponse = request.downloadHandler.text;
            print(jsonResponse);
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
