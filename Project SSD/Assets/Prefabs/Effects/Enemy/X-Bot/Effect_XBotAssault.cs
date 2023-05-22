using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_XBotAssault : MonoBehaviour {
	[SerializeField] private float amount = 10f;

	private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 7) {
			Vector3 force = (other.transform.position - transform.position).normalized * 3f;
			Damage damage = new Damage(
				gameObject,
				amount,
				0,
				force,
				Damage.DamageType.Normal
			); 
			other.GetComponent<IDamageable>()?.OnDamage(damage);
        }
    }
}
