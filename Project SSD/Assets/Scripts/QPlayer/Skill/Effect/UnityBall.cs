using Mirror;
using S2CMessage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBall : MonoBehaviour, IPoolableObject
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
	public bool isHoming = false;
	protected Coroutine hideCoroutine = null;
	public string lastExplosionKey;
	public int networkId = -1;
	private static int unityBallCount = 0;
	public static Dictionary<int, UnityBall> unityBallInScene = new Dictionary<int, UnityBall>();
	public virtual void OnActive(float damage, float speed)
	{
		hideCoroutine = StartCoroutine(Hide());
		this.damageAmount = damage;
		this.speed = speed;
	}
	protected void Awake()
	{
		if (homingArea != null)
		{
			homingArea.onTriggerEnter += OnDetectHomingTarget;
			homingArea.onTriggerExit += OnLostHomingTarget;
		}
		networkId = unityBallCount;
		unityBallCount++;
		unityBallInScene.Add(networkId, this);
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
				damageAmount,
				.5f,
				(other.transform.position - transform.position).normalized * 10f,
				Damage.DamageType.Normal
			);
			target.OnDamage(damage);
			for (int i = 0; i < attachments.Count; i++)
			{
				Enemy enemy = other.gameObject.GetComponent<Enemy>();
				enemy?.AddAttachment(attachments[i]);
			}
			PoolerManager.instance.InPool(GetKey(), gameObject);
		}
	}
	protected void OnDetectHomingTarget(Collider other) {
		if (isHoming)
		{
			if (other.gameObject.layer == 8)
			{
				Enemy target = other.GetComponent<Enemy>();
				if (target != null)
				{
					if (!targets.Contains(other.transform))
					{
						if (SSDNetworkManager.instance.isHost)
						{
							int enemyNetId = other.GetComponent<Enemy>().networkId;
							NetworkServer.SendToAll<UnityBallSetTargetMessage>(new UnityBallSetTargetMessage(networkId, enemyNetId));
						}
					}
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
	public void AddTarget(int enemyNetId)
	{
		List<Enemy> enemys = EnemyManager.instance.enemiesInScene;
		Enemy target = null;
		for (int i = 0; i < enemys.Count; i++)
		{
			if(enemys[i].networkId == enemyNetId)
			{
				target = enemys[i];
				break;
			}
		}
		targets.Add(target.transform);
	}
	public string GetKey()
	{
		return GetType().ToString();
	}
}
