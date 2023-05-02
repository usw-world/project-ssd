using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBall : MonoBehaviour
{
	protected List<Transform> targets = new List<Transform>();
	protected GameObject lastExplosionPrefab;
	protected float runTime = 2f;
	protected float damage;
	protected float speed;
	protected float lastExplosionDamage;
	protected bool lastExplosion = false;
	protected bool guided = false;
	public virtual void OnActive(float damage, float speed)
	{
		Invoke("Hide", runTime);
		this.damage = damage;
		this.speed = speed;
	}
	private void Update()
	{
		if (guided)
		{
			if (targets.Count > 0)
			{
				if (targets[0] == null)
				{
					if (targets[0].gameObject.layer == 8)
					{
						Vector3 look = Vector3.Slerp(transform.forward, targets[0].position, 10f * Time.deltaTime);
						transform.rotation = Quaternion.LookRotation(look);
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
		gameObject.SetActive(false);
	}
	public virtual void AddDebuff() {
		// 매개변수로 디버프를 받아 목록에 추가한다 
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
			temp.GetComponent<UnityBallLastExplosion>().OnActive(lastExplosionDamage);
			temp.transform.localScale = transform.localScale;
		}
		// 초기화 해줘야함 변수
	}
	protected virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 8)
		{
			IDamageable temp = collision.gameObject.GetComponent<IDamageable>();
			temp.OnDamage(gameObject, damage);
			// 디버프 넘겨주기
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
