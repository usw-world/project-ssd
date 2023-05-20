using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateDamageCntl : MonoBehaviour
{
	[SerializeField] private GameObject hitEffectPrefab;
	private string hitEffectPoolerKey;
	private void Awake()
	{
		hitEffectPoolerKey = hitEffectPrefab.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(hitEffectPoolerKey, hitEffectPrefab, false);
	}
	public void OnDamage(List<Transform> targets, Damage damage) 
    {
		for (int i = 0; i < targets.Count; i++)
		{
			print(targets[i].name + $"에게 {damage.amount} 데미지 줌!");
			IDamageable target = targets[i].GetComponent<IDamageable>();
			damage.forceVector = (targets[i].position - transform.position).normalized * 5f;
			target.OnDamage(damage);
			GameObject hitEffect = PoolerManager.instance.OutPool(hitEffectPoolerKey);
			hitEffect.transform.position = targets[i].transform.position;
			hitEffect.transform.position += Vector3.up;
			hitEffect.transform.parent = null;
			StartCoroutine(HideHitEffect(hitEffect));
		}
    }
	private IEnumerator HideHitEffect(GameObject hitEffect)
	{
		yield return new WaitForSeconds(2f);
		PoolerManager.instance.InPool(hitEffectPoolerKey, hitEffect);
	}
}
