using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_FightGhostFistExplosion : MonoBehaviour, IPoolableObject
{
	public void Run(float damageAmount)
	{
		Collider[] hit = Physics.OverlapSphere(transform.position, 2f, 1 << 8);

		for (int i = 0; i < hit.Length; i++)
		{
			Damage damage = new Damage(
				damageAmount,
				0,
				Vector3.zero,
				Damage.DamageType.Normal
			) ;
			hit[i].GetComponent<IDamageable>().OnDamage(damage);
		}
	}
	public string GetKey()
	{
		return GetType().ToString();
	}
}
