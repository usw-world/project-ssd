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

    private SaveDataVO saveData = new SaveDataVO();

    // private string apiUrl = "https://uwu-web.azurewebsites.net";
    private string apiUrl = "http://localhost:5034";

    // 주소

    void Start() {
        // StartCoroutine(PostData());
    }

    private void Update()
    {
        /*
        // 회원가입 - 이미 존재하면 500 return
        if (Input.GetKeyDown(KeyCode.Q))
        {
            saveData = new SaveDataVO();
            saveData.user_id = "Lucius1";
            saveData.user_pw = "asdf";
            string json = JsonUtility.ToJson(saveData);
            StartCoroutine(Register(json));
        }
        
        // 로그인 - 접속과 동시에 세이브 데이터 받아옴
        if(Input.GetKeyDown(KeyCode.Space))
        {
            saveData = new SaveDataVO();
            saveData.user_id = "Lucius1";
            saveData.user_pw = "asdf";
            string json = JsonUtility.ToJson(saveData);
            StartCoroutine(Login(json));
        }

        // 데이터 보내기 - 서버로 데이터 전송
        if (Input.GetKeyDown(KeyCode.W))
        {
            saveData = SaveTest.saveData;
            string json = JsonUtility.ToJson(saveData);
            StartCoroutine(SendInGameData(json));
        }
        
        // 데이터 받아오기 - 서버에서 데이터 받아옴
        if (Input.GetKeyDown(KeyCode.E))
        {
            saveData = SaveTest.saveData;
            string json = JsonUtility.ToJson(saveData);
            StartCoroutine(RequestInGameData(json));
        }
        */
    }

    public void Login(string json, Action<string> callback) {
        StartCoroutine(LoginCoroutine(json, callback));
    }
    private IEnumerator LoginCoroutine(string postData, Action<string> callback) {
        // string postData = "{\"status\": \"test\" }"; 
        // // POST로 보낼 JSON 데이터
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
            // 응답 JSON 문자열 저장
            Debug.Log(jsonResponse);
            SaveDataVO saveData = new SaveDataVO();
            this.saveData.Load(jsonResponse);
            GameManager.instance?.SetSaveData(this.saveData);
            callback?.Invoke(saveData.token);
        } else {
            Debug.LogError(request.responseCode);
        }
        request.Dispose();
    }
    private void OnGUI() {
        if(showNetworkStatus)
            GUI.Label(Rect.zero, networkStatusText);
    }

    private IEnumerator RegisterCoroutine(string postData, Action callback=null) {
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
            // 응답 JSON 문자열 저장
            Debug.Log(jsonResponse);
            Debug.Log("회원가입 성공");
            saveData = JsonUtility.FromJson<SaveDataVO>(jsonResponse); 
            // JsonUtility.FromJsonOverwrite(jsonResponse, saveData);
            // JSON 문자열 파싱

            Debug.Log(saveData.ToString());
            callback?.Invoke();
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
    
    private IEnumerator RequestIngameDataCoroutiune(string postData)
    {
        Debug.Log(postData);
        byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(postData.ToString());
        UnityWebRequest request = UnityWebRequest.Post(apiUrl+"/getData", "POST");
        request.uploadHandler.Dispose();
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postDataBytes); 
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); 
        yield return request.SendWebRequest(); 
        if (request.responseCode == 200) {
            string jsonResponse = request.downloadHandler.text; 
            saveData = JsonUtility.FromJson<SaveDataVO>(jsonResponse);
            Debug.Log("download Success");
        } else {
            Debug.LogError(request.responseCode);
        }

        request.Dispose();
        // 위와 동일
    }
}
