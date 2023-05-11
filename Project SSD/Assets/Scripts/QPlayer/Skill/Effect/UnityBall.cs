using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBall : MonoBehaviour, IPoolerableObject
{
	[SerializeField] protected CollisionEventHandler homingArea;
	protected List<Attachment> attachments = new List<Attachment>();
	protected List<Transform> targets = new List<Transform>();
	protected float runTime = 2f;
	protected float damageAmount;
	protected float speed;
	protected float homingPerformance = .1f;
	protected float lastExplosionDamage;
	protected bool isLastExplosion = false;
	protected bool isHoming = false;
	protected Coroutine hideCoroutine = null;
	protected string lastExplosionKey;
	public virtual void OnActive(float damage, float speed)
	{
		hideCoroutine = StartCoroutine(Hide());
		this.damageAmount = damage;
		this.speed = speed;
	}
	protected void Start()
	{
		if (homingArea != null)
		{
			homingArea.onTriggerEnter += OnDetectHomingTarget;
			homingArea.onTriggerExit += OnDetectHomingTarget;
		}
	}
	protected void Update()
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
	protected IEnumerator Hide() {
		yield return new WaitForSeconds(runTime);
		PoolerManager.instance.InPool(GetKey(), gameObject);
	}
	public virtual void AddDebuff(Attachment attachment) {
		attachments.Add(attachment);
	}
	public virtual void AddLastExplosion(string lastExplosionKey, float explosionDamage) {
		this.lastExplosionKey = lastExplosionKey;
		isLastExplosion = true;
		lastExplosionDamage = explosionDamage;
	}
	public virtual void OnActiveGuided(){
		isHoming = true;
	}
	protected virtual void OnDisable()
	{
		if (isLastExplosion)
		{
			GameObject obj = PoolerManager.instance.OutPool(lastExplosionKey);
			obj.transform.position = transform.position;
			obj.transform.localScale = transform.localScale;
			obj.GetComponent<UnityBallLastExplosion>().OnActive(lastExplosionDamage);
		}
		if (hideCoroutine != null) StopCoroutine(hideCoroutine);
		hideCoroutine = null;
		isLastExplosion = false;
		isHoming = false;
		homingPerformance = .1f;
		attachments.Clear();
		targets.Clear();
	}
	protected virtual void OnTriggerEnter(Collider other)
	{
		//print("Unity Ball hit the " + other.gameObject.name);
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
			PoolerManager.instance.InPool(GetKey(), gameObject);
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
	public string GetKey()
	{
		return GetType().ToString();
	}
}
