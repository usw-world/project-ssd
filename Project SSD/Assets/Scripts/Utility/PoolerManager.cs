using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolerManager : MonoBehaviour
{
    public static PoolerManager instance { get; private set; }

    private Dictionary<string, ObjectPooler> poolers = new Dictionary<string, ObjectPooler>();

	private List<string> destroyedKeys = new List<string>();
    private void Awake()
	{
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }
    public void InsertPooler(string key, GameObject prefab, bool destroy, int count=10, int restoreCount=5) {
        if (!poolers.ContainsKey(key))
        {
            ObjectPooler pooler = new ObjectPooler(
                prefab,
                (GameObject instance) => {
                    instance.SetActive(false);
                },
                (GameObject instance) => {
                    instance.SetActive(true);
                },
                this.transform,
                count,
                restoreCount
            );
            poolers.Add(key, pooler);
			if (destroy)
			{
				destroyedKeys.Add(key);
			}
        }
    }
    public void InPool(string key, GameObject target)
    {
		if (poolers.ContainsKey(key))
		{
            ObjectPooler pooler = poolers[key];
            pooler.InPool(target);
        }
    }
    public GameObject OutPool(string key)
    {
        if (poolers.ContainsKey(key))
        {
            ObjectPooler pooler = poolers[key];
            GameObject obj = pooler.OutPool();
            return obj;
        }
        Debug.LogWarning("OutPool method was called with unvaild key.");
		return null;
    }
	public void DestroyPooler()
	{
		for (int i = 0; i < destroyedKeys.Count; i++)
		{
            ObjectPooler pooler = this.poolers[destroyedKeys[i]];
			List<GameObject> allPoolers = pooler.GetAllItem();
			for (int j = 0; j < allPoolers.Count; )
			{
				Destroy(allPoolers[j]);
			}
			this.poolers.Remove(destroyedKeys[i]);
		}
		destroyedKeys.Clear();
	}
}
