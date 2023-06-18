using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_PteranodonAssault : MonoBehaviour {
	[SerializeField] private float amount = 15f;
	[SerializeField] private float forceScalar = 25f;
    public bool isActive = false;
	private bool hasHit = false;
    
    public void Active() {
        isActive = true;
        hasHit = false;
    }
    public void Inactive() {
        isActive = false;
    }
	private void OnTriggerStay(Collider other) {
        if(!hasHit
        && isActive
		&& other.gameObject.layer == 7) {
			Vector3 force = (other.transform.position - transform.position).normalized * forceScalar;
			Damage damage = new Damage(
				amount,
				.55f,
				force,
				Damage.DamageType.Normal
			); 
			other.GetComponent<IDamageable>()?.OnDamage(damage);
			hasHit = true;
        }
    }
}
