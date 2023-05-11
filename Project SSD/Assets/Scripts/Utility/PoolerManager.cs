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
    public void InsertPooler(string key, GameObject prefab, bool destroy) {
        if (!poolers.ContainsKey(key))
        {
            poolers.Add(key, new ObjectPooler(prefab));
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
            ObjectPooler temp = poolers[key];
            temp.InPool(target);
        }
    }
    public GameObject OutPool(string key)
    {
        if (poolers.ContainsKey(key))
        {
            ObjectPooler temp = poolers[key];
            GameObject obj = temp.OutPool();
            obj.SetActive(true);
            return obj;
        }
		return null;
    }
	public void DestroyPooler()
	{
		for (int i = 0; i < destroyedKeys.Count; i++)
		{
			ObjectPooler pooler = poolers[destroyedKeys[i]];
			List<GameObject> temp = pooler.GetAllItem();
			for (int j = 0; j < temp.Count; )
			{
				Destroy(temp[j]);
			}
			poolers.Remove(destroyedKeys[i]);
		}
		destroyedKeys.Clear();
	}
}
