using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBall : MonoBehaviour
{
	[SerializeField]protected GameObject lastExplosionPrefab;
	protected List<Transform> targets = new List<Transform>();
	protected List<Attachment> attachments = new List<Attachment>();
	protected float runTime = 2f;
	protected float damageAmount;
	protected float speed;
	protected float guidedPerformance = 7f;
	protected float lastExplosionDamage;
	protected bool lastExplosion = false;
	protected bool guided = false;
	public virtual void OnActive(float damage, float speed)
	{
		Invoke("Hide", runTime);
		this.damageAmount = damage;
		this.speed = speed;
	}
	private void Update()
	{
		if (guided)
		{
			if (targets.Count > 0)
			{
				if (targets[0] != null)
				{
					if (targets[0].gameObject.layer == 8)
					{
						Vector3 look = Vector3.Slerp(transform.forward, targets[0].position - transform.position, guidedPerformance * Time.deltaTime);
						look.y = 0;
						transform.rotation = Quaternion.LookRotation(look);
						guidedPerformance += Time.deltaTime * 30f;
					}
					else
					{
						targets.Remove(targets[0]);
					}
				}
				else
				{
					targets.Remove(targets[0]);
				}
			}
		}
		transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}
	public void Hide() {
		gameObject?.SetActive(false);
	}
	public virtual void AddDebuff(Attachment attachment) {
		attachments.Add(attachment);
	}
	public virtual void AddLastExplosion(float explosionDamage) {
		lastExplosion = true;
		lastExplosionDamage = explosionDamage;
	}
	public virtual void OnActiveGuided() {
		guided = true;
	}
	protected virtual void OnDisable()
	{
		if (lastExplosion)
		{
			GameObject temp = Instantiate(lastExplosionPrefab, transform.position, Quaternion.Euler(0, 0, 0));
			temp.transform.localScale = transform.localScale;
			temp.GetComponent<UnityBallLastExplosion>().OnActive(lastExplosionDamage);
		}
		// 초기화 해줘야함 변수
	}
	protected virtual void OnCollisionEnter(Collision collision)
	{
		print("Unity Ball hit the " + collision.gameObject.name);
		if (collision.gameObject.layer == 8)
		{
			IDamageable target = collision.gameObject.GetComponent<IDamageable>();
			Damage damage = new Damage(
				gameObject,
				damageAmount,
				.4f,
				(collision.transform.position - transform.position).normalized * 1.5f,
				Damage.DamageType.Normal
			);
			target.OnDamage(damage);
			for (int i = 0; i < attachments.Count; i++)
			{
				IAttachable attachable = collision.gameObject.GetComponent<IAttachable>();
				attachable.AddAttachment(attachments[i]);
			}
			gameObject.SetActive(false);
		}
	}
	protected void OnTriggerEnter(Collider other)
	{
		if (guided)
		{
			if (other.gameObject.layer == 8)
			{
				if (!targets.Contains(other.transform))
				{
					targets.Add(other.transform);
				}
			}
		}
	}
	protected void OnTriggerExit(Collider other)
	{
		if (guided)
		{
			if (other.gameObject.layer == 8)
			{
				targets.Remove(other.transform);
			}
		}
	}
}
