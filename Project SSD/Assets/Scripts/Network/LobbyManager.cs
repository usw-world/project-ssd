using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Mirror;

[System.Serializable]
public class NetworkEvent : UnityEngine.Events.UnityEvent {}

public class LobbyManager : MonoBehaviour {
    static public LobbyManager instance;

    bool isHost { get => SSDNetworkManager.instance.isHost; }

    #region Room UI
    [SerializeField] private GameObject lobbyUis;
    [SerializeField] private GameObject roomUis;

    [HideInInspector] public string hostName = null;
    [HideInInspector] public string guestName = null;

    [SerializeField] private Text hostNameText;
    [SerializeField] private Text guestNameText;

    [SerializeField] private InputField addressField;

    [SerializeField] private Button startButton;
    #endregion Room UI

    #region Login UI
    [SerializeField] private TMP_InputField idField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private NetworkEvent onLoginSuccess;
    #endregion Login UI

    #region Join UI
    [SerializeField] private TMP_InputField joinIdField;
    [SerializeField] private TMP_InputField joinPasswordField;
    #endregion Join UI

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
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
    public void OnClickStartButton() {
        SSDNetworkManager.instance.StartGame();
    } 

    public void Login() {
        ServerConnector connector = ServerConnector.instance;
        if(connector != null) {
            connector.Login(
                idField.text,
                passwordField.text,
                () => {
                    print("Login Success.");
                    onLoginSuccess?.Invoke();
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
                () => {
                    print("Join Success.\n");
                },
                (string error) => {
                    UIManager.instance.AlertMessage(error);
                }
            );
        }
    }
}