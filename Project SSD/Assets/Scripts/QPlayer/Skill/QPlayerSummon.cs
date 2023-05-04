using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSummon : SkillEffect
{
	[SerializeField] Transform muzzle;
	[SerializeField] GameObject attackBullet;
	[SerializeField] float runTime;
	List<Transform> targets = new List<Transform>();
	float attackTime = 1f;
	// SkillProperty property;
	private void Start()
	{
		StartCoroutine(Hide());
	}
	private void Update()
	{
		if (targets.Count != 0)
		{
			attackTime += Time.deltaTime;
			if (attackTime >= 1f)
			{
				if (targets[0] != null)
				{
					if (targets[0].gameObject.layer == 8)
					{
						attackTime = 0;

						transform.LookAt(targets[0]);
						Vector3 temp = transform.eulerAngles;
						temp.x = 0; temp.z = 0;
						transform.eulerAngles = temp;

						Attack();
					}
					else
					{
						targets.Remove(targets[0]);
						attackTime = 100f;
					}
					
				}
				else
				{
					targets.Remove(targets[0]);
					attackTime = 100f;
				}
			}
		}
	}
	void Attack()
	{
		GameObject temp = Instantiate(attackBullet, muzzle.position, transform.rotation);
		Projectile projectile = temp.GetComponent<Projectile>();
		projectile.Set(QPlayer.instance.GetAP() * property.skillAP);
	}
	IEnumerator Hide()
	{
		yield return new WaitForSeconds(runTime);
		gameObject.SetActive(false);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer ==  8)
		{
			Transform temp = other.gameObject.transform;
			if (!targets.Contains(temp))
			{
				targets.Add(temp);
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer ==  8)
		{
			targets.Remove(other.gameObject.transform);
		}
	}

	public override void OnActive(SkillProperty property)
	{
		this.property = property;
	}
}
