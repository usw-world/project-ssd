using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBallLastExplosion : MonoBehaviour
{
	float damage;
	public void OnActive(float damage) {
		this.damage = damage;
		Collider[] hit = null;
		hit = Physics.OverlapSphere(transform.position, transform.localScale.x, 1 << 8);
		for (int i = 0; i < hit.Length; i++)
		{
			IDamageable temp = hit[i].GetComponent<IDamageable>();
			temp?.OnDamage(gameObject, damage);
		}
		Invoke("Hide", 1f);
	}
	void Hide() {
		gameObject.SetActive(false);
	}
}
