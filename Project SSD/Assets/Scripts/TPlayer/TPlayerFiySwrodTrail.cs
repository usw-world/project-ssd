using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerFiySwrodTrail : MonoBehaviour
{
    private float damageAmount;
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 15f);
    }
    public void SetDamage(float damageAmount) {
        this.damageAmount = damageAmount;
    }
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
            Damage damage = new Damage(
                damageAmount,
                1f,
                other.transform.position - transform.position * 3f * Time.deltaTime,
                Damage.DamageType.Normal
            );
            other.GetComponent<IDamageable>()?.OnDamage(damage);
        }
	}
}
