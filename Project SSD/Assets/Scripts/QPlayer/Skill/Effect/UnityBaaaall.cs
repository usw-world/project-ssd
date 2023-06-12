using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBaaaall : UnityBall
{
	[SerializeField] private GameObject explosionEfect;
	public override void OnActive(float damage, float speed)
	{
		speed = speed * 0.5f;
		base.OnActive(damage, speed);
		isRun = false;
	}
	public void Explosion()
	{
		CameraManager.instance.MakeNoise(2f, 0.3f);
		Collider[] hit = Physics.OverlapSphere(transform.position, 2.5f, 1 << 8);
		for (int i = 0; i < hit.Length; i++)
		{
			IDamageable target = hit[i].gameObject.GetComponent<IDamageable>();
			Damage damage = new Damage(
				damageAmount,
				2f,
				(hit[i].transform.position - transform.position).normalized * 10f,
				Damage.DamageType.Down
			);
			target.OnDamage(damage);
			for (int j = 0; j < attachments.Count; j++)
			{
				Enemy enemy = hit[i].gameObject.GetComponent<Enemy>();
				enemy?.AddAttachment(attachments[j]);
			}
		}
		StartCoroutine(ExplosionCo());
	}
	private IEnumerator ExplosionCo()
	{
		explosionEfect.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		explosionEfect.SetActive(false);
	}
}
