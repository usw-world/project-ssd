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

    public struct ChangeStateMessage : NetworkMessage {
        public int networkId;
        public string stateName;
        public ChangeStateMessage(int networkId, string stateName) {
            this.networkId = networkId;
            this.stateName = stateName;
        }
    }
}
namespace S2CMessage {
    public struct ShareUserInformationsMessage : NetworkMessage {
        public string hostName;
        public string guestName;
        public ShareUserInformationsMessage(string hostName, string guestName) {
            this.hostName = hostName;
            this.guestName = guestName;
        }
    }

    public struct SyncEnemyMessage : NetworkMessage {
        public int networkId;
        public Position position;
        public Rotation rotation;
        public SyncEnemyMessage(int networkId, Position position, Rotation rotation) {
            this.networkId = networkId;
            this.position = position;
            this.rotation = rotation;
        }
    }

    public struct SyncEnemyStateMessage : NetworkMessage {
        public int networkId;
        public string stateName;
        public SyncEnemyStateMessage(int networkId, string stateName) {
            this.networkId = networkId;
            this.stateName = stateName;
        }
    }

    public struct DamageMessage : NetworkMessage {
        string networkId;
        Damage damage;
    }
}
public struct Position {
    public float x;
    public float y;
    public float z;
    public Position(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
public struct Rotation {
    public float x;
    public float y;
    public float z;
    public float w;
    public Rotation(float x, float y, float z, float w) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
}