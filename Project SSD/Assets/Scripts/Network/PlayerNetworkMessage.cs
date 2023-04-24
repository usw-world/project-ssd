using UnityEngine;
using Mirror;

namespace C2SMessage {
    public struct LoadScene : NetworkMessage {
        string sceneName;
    }

    public struct CreateTPlayerPrefabMessage : NetworkMessage {
        Transform spawnPoint;
        public CreateTPlayerPrefabMessage(Transform spawnPoint) {
            this.spawnPoint = spawnPoint;
        }
    }
    public struct CreateQPlayerPrefabMessage : NetworkMessage {}
    
    public struct JoinRoomMessage : NetworkMessage {
        public bool isHost;
        public string userName;
        public JoinRoomMessage(bool isHost, string userName) {
            this.isHost = isHost;
            this.userName = userName;
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
}