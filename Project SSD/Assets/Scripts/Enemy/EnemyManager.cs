using System.Collections.Generic;

using UnityEngine;

using Mirror;

public class EnemyManager : NetworkBehaviour {
    static public EnemyManager instance;

    [SerializeField] private float enemiesSyncInterval = .2f;
    private float elapsedTime = 0;

    private List<Enemy> enemiesInScene;

    Queue<Enemy> registerQueue;
    
    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        
        enemiesInScene = new List<Enemy>();
    }
    private void Start() {}
    private void Update() {
        elapsedTime += Time.deltaTime;
        if(enemiesSyncInterval >= elapsedTime) {
            elapsedTime -= enemiesSyncInterval;
            CmdSyncronizeEnemies();
        }
    }
    [Command]
    private void CmdSyncronizeEnemies() {
        for(int i=0; i<enemiesInScene.Count; i++) {
            SyncTransform(i, enemiesInScene[i].transform);
        }
    }
    [ClientRpc]
    private void SyncTransform(int index, Transform target) {
        enemiesInScene[index].transform.position = target.position;
        enemiesInScene[index].transform.rotation = target.rotation;
    }
    public void Add(GameObject enemy) {
        CmdRegister(enemy);
    }
    [Command]
    private void CmdRegister(GameObject enemy) {
        Register(enemy.gameObject);
    }
    [ClientRpc]
    private void Register(GameObject enemy) {
        enemiesInScene.Add(enemy.GetComponent<Enemy>());
    }
}