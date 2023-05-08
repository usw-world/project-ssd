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
	protected float homingPerformance = .1f;
	protected float lastExplosionDamage;
	protected bool lastExplosion = false;
	protected bool isHoming = false;
	[SerializeField] protected CollisionEventHandler homingArea;
	public virtual void OnActive(float damage, float speed)
	{
		Invoke("Hide", runTime);
		this.damageAmount = damage;
		this.speed = speed;

		homingArea.onTriggerEnter += OnDetectHomingTarget;
		homingArea.onTriggerExit += OnDetectHomingTarget;
	}
	private void Update()
	{
		if (isHoming)
		{
			if (targets.Count > 0)
			{
				if (targets[0] != null)
				{
					if (targets[0].gameObject.layer == 8)
					{
						Vector3 look = Vector3.Slerp(transform.forward, targets[0].position - transform.position, homingPerformance * Time.deltaTime);
						look.y = 0;
						transform.rotation = Quaternion.LookRotation(look);
						homingPerformance += Time.deltaTime * 30f;
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
		isHoming = true;
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
	protected virtual void OnTriggerEnter(Collider other)
	{
		print("Unity Ball hit the " + other.gameObject.name);
		if (other.gameObject.layer == 8)
		{
			IDamageable target = other.gameObject.GetComponent<IDamageable>();
			Damage damage = new Damage(
				gameObject,
				damageAmount,
				.5f,
				(other.transform.position - transform.position).normalized * 10f,
				Damage.DamageType.Normal
			);
			target.OnDamage(damage);
			for (int i = 0; i < attachments.Count; i++)
			{
				IAttachable attachable = other.gameObject.GetComponent<IAttachable>();
				attachable.AddAttachment(attachments[i]);
			}
			gameObject.SetActive(false);
		}
	}
	protected void OnDetectHomingTarget(Collider other) {
		if (isHoming)
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
	protected void OnLostHomingTarget(Collider other) {
		if (isHoming)
		{
			if (other.gameObject.layer == 8)
			{
				targets.Remove(other.transform);
			}
		}
	}
}
