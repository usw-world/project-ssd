using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Mirror;

[System.Serializable]
public class NetworkEvent : UnityEngine.Events.UnityEvent {}

public class LobbyManager : MonoBehaviour {
    static public LobbyManager instance;

    bool isHost { get => SSDNetworkManager.instance.isHost; }

    [SerializeField] private LoadingUI loadingUI;
    [SerializeField] private NetworkEvent onConnectServer;
    [SerializeField] private NetworkEvent onErrorConnectServer;
    [SerializeField] private NetworkEvent onStartWithoutSaveData;

    [SerializeField] private TMP_Text startText;

    #region Room UI
    [SerializeField] private GameObject lobbyUis;
    [SerializeField] private GameObject roomUis;
    [HideInInspector] public string hostName = null;
    [HideInInspector] public string guestName = null;
    [SerializeField] private TMP_Text hostNameText;
    [SerializeField] private TMP_Text guestNameText;
    [SerializeField] private Image roomTPlayerImage;
    [SerializeField] private Image roomQPlayerImage;
    [SerializeField] private InputField addressField;
    [SerializeField] private Button startButton;

    [SerializeField] private GameObject resultUI;
    [SerializeField] private TMP_Text[] resultText;
    #endregion Room UI

    #region Login UI
    [SerializeField] private TMP_InputField idField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private NetworkEvent onSuccessLogin;
    #endregion Login UI

    #region Join UI
    [SerializeField] private TMP_InputField joinIdField;
    [SerializeField] private TMP_InputField joinPasswordField;
    [SerializeField] private NetworkEvent onSuccessJoin;
    #endregion Join UI

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start() {
        UIManager.instance.FadeIn(1, 2, () => {
            startText.gameObject.SetActive(true);
        });
        if(GameManager.instance.result != null) {
            resultUI.SetActive(true);
            resultText[0].text = " + " + GameManager.instance.result.exp.ToString();
            resultText[1].text = " + " + GameManager.instance.result.exp.ToString();
        }
    }
    public void OnClickConnectServer() {
        loadingUI.OnStartLoading();
        ServerConnector.instance.Ping(
            ()             => { onConnectServer?.Invoke(); },
            (string error) => { onErrorConnectServer?.Invoke(); }
        );
    }
    public void StartWithoutSaveDate() {
        GameManager.instance.withoutSavedata = true;
        GameManager.instance.SetSaveData(new SaveDataVO());
        onStartWithoutSaveData?.Invoke();
    }
    public void OnClickHostButton() {
        SSDNetworkManager.instance.HostSession();
    }
    public void OnClickJoinButton() {
        SSDNetworkManager.instance.JoinSession(addressField.text);
    }
    public void OnClickCloseRoomButton() {
        SSDNetworkManager.instance.CloseSession();

        hostName = null;
        guestName = null;
        OpenLobbyUi();
    }
    
    public void OnJoinRoom() {
        OpenRoomUi();
        RefreshRoom();
    }
    public void RefreshRoom() {
        hostNameText.text = hostName ?? "_";
        guestNameText.text = guestName ?? "_";
        roomTPlayerImage.color = hostName==null ? Color.black : Color.white;
        roomQPlayerImage.color = guestName==null ? Color.black : Color.white;
        startButton.gameObject.SetActive(SSDNetworkManager.instance.isHost);
    }
    public void OpenLobbyUi() {
        roomUis.SetActive(false);
        lobbyUis.SetActive(true);
    }
    public void OpenRoomUi() {
        lobbyUis.SetActive(false);
        roomUis.SetActive(true);
    }
    public void OnClickStartButton(string sceneName) {
        SSDNetworkManager.instance.LoadScene(sceneName);
    }

    public void Login() {
        ServerConnector connector = ServerConnector.instance;
        if(connector != null) {
            connector.Login(
                idField.text,
                passwordField.text,
                ()             => {
                    this.onSuccessLogin?.Invoke();
                    ServerConnector.instance.RequestIngameData();
                },
                (string error) => {
                    UIManager.instance.AlertMessage(error);
                }
            );
        }
    }
    public void Join() {
        ServerConnector connector = ServerConnector.instance;
        if(connector != null) {
            connector.Register(
                joinIdField.text,
                joinPasswordField.text,
                ()             => { onSuccessJoin?.Invoke(); },
                (string error) => { UIManager.instance.AlertMessage(error); }
            );
        }
    }
    public void SetIMECompositionMode(bool next) {
        StartCoroutine(SetIMECompositionModeDelay(next));
    }
    private System.Collections.IEnumerator SetIMECompositionModeDelay(bool next) {
        yield return null;
        Input.imeCompositionMode = next ? IMECompositionMode.Auto : IMECompositionMode.Off;
    }
}