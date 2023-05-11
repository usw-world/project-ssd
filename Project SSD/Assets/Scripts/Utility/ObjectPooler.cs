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
    
    public ObjectPooler(GameObject poolingObject, Action<GameObject> onInPool=null, Action<GameObject> onOutPool=null, Transform parent=null, int count=10, int restoreCount=5) {
        this.poolingObject = poolingObject;
        this.parent = PoolerManager.instance.transform;
        this.onInPool = onInPool;
        this.onOutPool = onOutPool;
        Store(count);
    }
    public void InPool(GameObject target) {
        onInPool?.Invoke(target);
		target.transform.SetParent(parent);
		target.SetActive(false);
		queue.Enqueue(target);
    }
    public GameObject OutPool() {
		if (queue.Count <= 0)
		{
			Store(restoreCount);
		}
		GameObject go = queue.Dequeue();
		go.SetActive(true);
		go.transform.SetParent(null);
        onOutPool?.Invoke(go);
        return go;
    }
    public GameObject OutPool(Vector3 point, Quaternion rotation) {
		if (queue.Count <= 0)
		{
			Store(restoreCount);
		}
		GameObject go = queue.Dequeue();
        go.transform.SetParent(null);
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
	public List<GameObject> GetAllItem() {
		List<GameObject> temp = new List<GameObject>();
		for (int i = 0; i < queue.Count; i++)
		{
			temp.Add(queue.Dequeue());
		}
		return temp;
	}
}
class NotMatchWithPrefabException : Exception {
    public string message = "Inpooling GameObject is not matched to prefab GameObject.";
}