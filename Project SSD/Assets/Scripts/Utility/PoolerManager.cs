using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolerManager : MonoBehaviour
{
    public static PoolerManager instance { get; private set; }

    private Dictionary<string, ObjectPooler> poolers = new Dictionary<string, ObjectPooler>();

    private void Awake()
	{
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }
    public void InsertPooler(string key, GameObject prefab) {
        if (!poolers.ContainsKey(key))
        {
            poolers.Add(key, new ObjectPooler(prefab));
            print($"{key}풀러 등록 성공!");
        }
        else
        {
            print($"{key}는 이미 저장되어 있습니다");
        }
    }
    public void InPool(string key, GameObject target)
    {
		if (poolers.ContainsKey(key))
		{
            target.SetActive(false);
            ObjectPooler temp = poolers[key];
            temp.InPool(target);
            print($"{key} InPool 성공!");
        }
		else
		{
            print($"{key}는 저장되어있지 않습니다");
		}
    }
    public GameObject OutPool(string key)
    {
        if (poolers.ContainsKey(key))
        {
            ObjectPooler temp = poolers[key];
            GameObject obj = temp.OutPool();
            obj.SetActive(true);
            obj.GetComponent<SkillEffect>().poolerKey = key;
            print($"{key} OutPool 성공!");
            return obj;
        }
        else
        {
            print($"{key}는 저장되어있지 않습니다");
            return null;
        }
    }
}
