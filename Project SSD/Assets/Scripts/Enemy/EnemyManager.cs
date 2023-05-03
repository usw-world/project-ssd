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
    private void Start() {
    }
    private void Update() {
        elapsedTime += Time.deltaTime;
        if(enemiesSyncInterval >= elapsedTime) {
            elapsedTime -= enemiesSyncInterval;
        }
    }
    [Command]
    public void Add(GameObject gobj) {
        Add(gobj.GetComponent<Enemy>());
        print(enemiesInScene.Count);
    }
    public void Add(Enemy enemy) {
        enemiesInScene.Add(enemy);
    }
}
public struct Position {
    float x;
    float y;
    float z;
}