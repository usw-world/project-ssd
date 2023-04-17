using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ObjectPooler {
    [SerializeField] private GameObject poolingObject;
    private Transform parent = null;
    private int count;
    private int restoreCount;
    private Queue<GameObject> queue = new Queue<GameObject>();

    public Action<GameObject> onInPool;
    public Action<GameObject> onOutPool;
    
    public ObjectPooler(GameObject poolingObject, Action<GameObject> onInPool, Action<GameObject> onOutPool, int count=10, int restoreCount=5, Transform parent=null) {
        this.poolingObject = poolingObject;
        this.parent = parent;
        this.onInPool = onInPool;
        this.onOutPool = onOutPool;
        Store(count);
    }
    public void InPool(GameObject target) {
        onInPool?.Invoke(target);
        queue.Enqueue(target);
    }
    public GameObject OutPool(Transform parent=null) {
        GameObject go = queue.Dequeue();
        go.transform.SetParent(parent);
        onOutPool?.Invoke(go);
        return go;
    }
    public GameObject OutPool(Vector3 point, Quaternion rotation, Transform parent=null) {
        GameObject go = queue.Dequeue();
        go.transform.SetParent(parent);
        go.transform.position = point;
        go.transform.rotation = rotation;
        onOutPool?.Invoke(go);
        return go;
    }
    private void Store(int count) {
        for(int i=0; i<count; i++) {
            GameObject go = GameObject.Instantiate(poolingObject, parent);
            onInPool?.Invoke(go);
            queue.Enqueue(go);
        }
    }
}
class NotMatchWithPrefabException : Exception {
    public string message = "Inpooling GameObject is not matched to prefab GameObject.";
}
