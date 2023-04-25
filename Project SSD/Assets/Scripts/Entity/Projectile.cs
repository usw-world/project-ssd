using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] float speed;
	[SerializeField] GameObject hitEffect;
	float amount;
    void Update()
    {
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
	public void Set(float amount)
	{
		this.amount = amount;
	}
	private void OnCollisionEnter(Collision collision)
	{
		
		if (collision.gameObject.layer == 8 || collision.gameObject.layer == 6)
		{
			Instantiate(hitEffect, transform.position, Quaternion.Euler(0, 0, 0));
			gameObject.SetActive(false);
			collision.gameObject.GetComponent<IDamageable>()?.OnDamage(gameObject, amount);
		}
	}
}
