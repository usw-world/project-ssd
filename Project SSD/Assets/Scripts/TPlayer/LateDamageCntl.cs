using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateDamageCntl : MonoBehaviour
{
	[SerializeField] private GameObject hitEffectBoom01_prefab;
	[SerializeField] private GameObject hitEffectSlash01_prefab;
	private string hitEffectBoom01PoolerKey;
	private string hitEffectSlash01PoolerKey;
	private string currentHitEffectPoolerKey;
	private void Awake()
	{
		hitEffectBoom01PoolerKey = hitEffectBoom01_prefab.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(hitEffectBoom01PoolerKey, hitEffectBoom01_prefab, false);
		hitEffectSlash01PoolerKey = hitEffectSlash01_prefab.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(hitEffectSlash01PoolerKey, hitEffectSlash01_prefab, false);
	}
	public void OnDamage(List<Transform> targets, Damage damage, EHitEffectType type)
	{
		switch (type)
		{
			case EHitEffectType.boom_1:
				currentHitEffectPoolerKey = hitEffectBoom01PoolerKey;
				break;
			case EHitEffectType.slash_1:
				currentHitEffectPoolerKey = hitEffectSlash01PoolerKey;
				break;
		}
		for (int i = 0; i < targets.Count; i++)
		{
			IDamageable target = targets[i].GetComponent<IDamageable>();
			damage.forceVector = (targets[i].position - transform.position).normalized * 5f;
			target.OnDamage(damage);
			GameObject hitEffect = PoolerManager.instance.OutPool(currentHitEffectPoolerKey);
			hitEffect.transform.position = targets[i].transform.position;
			hitEffect.transform.position += Vector3.up;
			hitEffect.transform.parent = null;
			StartCoroutine(HideHitEffect(hitEffect));
		}
    }
	private IEnumerator HideHitEffect(GameObject hitEffect)
	{
		yield return new WaitForSeconds(2f);
		PoolerManager.instance.InPool(currentHitEffectPoolerKey, hitEffect);
	}
}
