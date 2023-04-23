using UnityEngine;
using UnityEngine.UI;

using Mirror;

public class LobbyManager : MonoBehaviour {
    static public LobbyManager instance;

    bool isHost { get => SSDNetworkManager.instance.isHost; }
    
    [SerializeField] private InputField addressField;
    [SerializeField] private InputField portField;

    #region UIs
    [SerializeField] private GameObject lobbyUis;
    [SerializeField] private GameObject roomUis;

    [HideInInspector] public string hostName = null;
    [HideInInspector] public string guestName = null;

    [SerializeField] private Text hostNameText;
    [SerializeField] private Text guestNameText;
    #endregion UIs

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
        if(string.IsNullOrWhiteSpace(portField.text)) {
            SSDNetworkManager.instance.JoinSession(addressField.text);
        } else {
            SSDNetworkManager.instance.JoinSession($"{addressField.text}:{portField.text}");
        }
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
    }
    public void OpenLobbyUi() {
        roomUis.SetActive(false);
        lobbyUis.SetActive(true);
    }
    public void OpenRoomUi() {
        lobbyUis.SetActive(false);
        roomUis.SetActive(true);
    }
}