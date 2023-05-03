using System.Collections.Generic;

using UnityEngine;

using Mirror;

public class EnemyManager : MonoBehaviour {
    static public EnemyManager instance;

    [SerializeField] private float enemiesSyncInterval = .2f;
    private float elapsedTime = 0;
    private bool isHost = false;

    [SerializeField] private List<Enemy> enemiesInScene;

    Queue<Enemy> registerQueue;
    
    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start() {
        if(DebuggingNetworkManager.instance == null)
            this.isHost = SSDNetworkManager.instance.isHost;
        else
            this.isHost = DebuggingNetworkManager.instance.isHost;
    }
    private void Update() {
        if(isHost) {
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= enemiesSyncInterval) {
                elapsedTime -= enemiesSyncInterval;
                for(int i=0; i<enemiesInScene.Count; i++) {
                    GameObject enemy = enemiesInScene[i].gameObject;
                    Position pos = new Position(enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z);
                    Rotation rot = new Rotation(enemy.transform.rotation.x, enemy.transform.rotation.y, enemy.transform.rotation.z, enemy.transform.rotation.w);
                    NetworkServer.SendToAll(new S2CMessage.SyncEnemyMessage(i, pos, rot));
                    print("send");
                }
            }
        }
    }
    public void SyncEnemy(int index, Position position, Rotation rotation) {
        if(!isHost) {
            Enemy target = enemiesInScene[index];
            target.transform.position = new Vector3(position.x, position.y, position.z);
            target.transform.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
            print(new Vector3(position.x, position.y, position.z));
            print(new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w));
        }
    }
}