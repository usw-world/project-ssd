using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBallLastExplosion : MonoBehaviour, IPoolableObject
{
	public string GetKey()
	{
		return GetType().ToString();
	}

	public void OnActive(float damageAmount) {
		Collider[] hit = null;
		hit = Physics.OverlapSphere(transform.position, transform.localScale.x * 0.5f, 1 << 8);
		for (int i = 0; i < hit.Length; i++)
		{
			IDamageable target = hit[i].GetComponent<IDamageable>();
			Damage damage = new Damage(
				damageAmount,
				.3f,
				(hit[i].transform.position - transform.position).normalized * 2f,
				Damage.DamageType.Normal
			);
			target?.OnDamage(damage);
		}
		StartCoroutine(Hide());
	}
	protected IEnumerator Hide()
	{
		yield return new WaitForSeconds(1f);
		PoolerManager.instance.InPool(GetKey(), gameObject);
	}
}
