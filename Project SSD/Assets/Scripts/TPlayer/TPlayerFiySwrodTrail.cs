using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerFiySwrodTrail : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 15f);
    }
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
            Damage damage = new Damage(
                TPlayer.instance.GetAp() * 1.5f,
                1f,
                other.transform.position - transform.position * 3f * Time.deltaTime,
                Damage.DamageType.Normal
                );
            other.GetComponent<IDamageable>()?.OnDamage(damage);
        }
	}
}
