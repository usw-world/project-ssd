using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ShieldExplosion : MonoBehaviour, IPoolableObject
{
	public void Run(float damageAmount)
	{
		Transform tPlayer = TPlayer.instance.transform;
		Collider[] hit = Physics.OverlapSphere(tPlayer.position, 4f, 1 << 8);

		for (int i = 0; i < hit.Length; i++)
		{
			Damage damage = new Damage(
				damageAmount,
				1f,
				(hit[i].transform.position - transform.position).normalized * 5f,
				Damage.DamageType.Normal
			);
			hit[i].GetComponent<IDamageable>().OnDamage(damage);
		}
	}
	public string GetKey()
	{
		return GetType().ToString();
	}
}
