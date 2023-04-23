using Mirror;

namespace C2SMessage {
    public struct LoadScene : NetworkMessage {
        string sceneName;
    }

    public struct CreateTPlayerPrefabMessage : NetworkMessage {}
    public struct CreateQPlayerPrefabMessage : NetworkMessage {}
    
    public struct JoinRoomMessage : NetworkMessage {
        public bool isHost;
        public string userName;
        public JoinRoomMessage(bool isHost, string userName) {
            this.isHost = isHost;
            this.userName = userName;
        }
    }
    
    public struct LeaveRoomMessage : NetworkMessage {
        public bool isHost;
        public LeaveRoomMessage(bool isHost) {
            this.isHost = isHost;
        }
    }
}

namespace S2CMessage {
    public struct ShareUserInformations : NetworkMessage {
        public string hostName;
        public string guestName;
        public ShareUserInformations(string hostName, string guestName) {
            this.hostName = hostName;
            this.guestName = guestName;
        }
    }
    
    public struct ServerDieMessage : NetworkMessage {
        public string text;
        public ServerDieMessage(string message) {
            this.text = message;
        }
    }
    
    public struct AllowLeaveRoomMessage : NetworkMessage {}
}