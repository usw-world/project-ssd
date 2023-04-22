using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DBManager : MonoBehaviour
{
    private const string apiUrl = "http://localhost:8082/controller"; 
    // 주소

    void Start()
    {
        StartCoroutine(PostData());
    }

    IEnumerator PostData()
    {
        string postData = "{\"status\": \"a\" }"; 
        // POST로 보낼 JSON 데이터
        byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(postData);

        UnityWebRequest request = UnityWebRequest.Post(apiUrl, "POST");
        // POST 요청 생성

        request.uploadHandler.Dispose();
        // 메모리 누수 관련 요거 스캐너 쓸때처럼 dispose 해줘야함


        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postDataBytes); 
        // 요청 바디에 JSON 데이터 추가

        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); 
        // 응답 수신 방식 지정

        yield return request.SendWebRequest(); 
        // 요청 보내기

        if (request.result != UnityWebRequest.Result.Success) 
            // 요청 실패시 에러 출력
        {
            Debug.LogError(request.error);
            yield break;
        }


        string jsonResponse = request.downloadHandler.text; 
        // 응답 JSON 문자열 저장
        Debug.Log(jsonResponse);

        UserData user = JsonUtility.FromJson<UserData>(jsonResponse); 
        // JSON 문자열 파싱

        Debug.Log(user.status);

        request.Dispose();
        // 위와 동일
    }

    private class UserData
    {
        public string status;
    }
}
