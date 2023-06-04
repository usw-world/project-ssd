using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_LightningStrike : Effect_Lightning
{
	private List<GameObject> targets = new List<GameObject>();
	private List<Attachment> attachments = new List<Attachment>();
	public override void Run()
	{
		if (isBroadAttack)
			transform.localScale = new Vector3(3f, 1f, 1f);
		else
			transform.localScale = new Vector3(2f, 1f, 1f);

		if (isAttachmentDamage) attachments.Add(this.damage);
		if (isAttachmentInability) attachments.Add(inability);

		if (previewDamageZone)
		{
			GameObject previewBox = Instantiate(previewBoxPrefab, transform.position + transform.forward * 5f, QPlayer.instance.transform.rotation);
			previewBox.transform.localScale = transform.localScale;
			Destroy(previewBox, 2f);
		}
		StartCoroutine(InPool());
		StartCoroutine(damageEnemy(damageAmount, attachments));
	}
	protected override IEnumerator InPool()
	{
		yield return new WaitForSeconds(5f);
		PoolerManager.instance.InPool(GetKey(), gameObject);
		targets.Clear();
		attachments.Clear();
	}
	private IEnumerator damageEnemy(float lastDamageAmount, List<Attachment> attachments) 
	{
		float oneDotDamage = lastDamageAmount / 20f;
		
		for (int i = 0; i < 20; i++)
		{
			for (int j = 0; j < targets.Count; j++)
			{
				if (i == 0)
				{
					Enemy enemy;
					if (targets[j].TryGetComponent<Enemy>(out enemy))
					{
						for (int x = 0; x < attachments.Count; x++)
						{
							enemy.AddAttachment(attachments[x]);
						}
					}
				}
				Damage damage = new Damage(
					oneDotDamage,
					0.2f,
					(targets[j].transform.position - QPlayer.instance.transform.position).normalized * 5f,
					Damage.DamageType.Normal
				);
				targets[j]?.GetComponent<IDamageable>().OnDamage(damage);
			}
			yield return new WaitForSeconds(0.1f);
		}

		QPlayer.instance.ResetState();
	}
	private void OnTriggerEnter(Collider other)
	{
		IDamageable enemy;
		if (other.TryGetComponent<IDamageable>(out enemy))
		{
			if (!targets.Contains(other.gameObject))
			{
				targets.Add(other.gameObject);
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		IDamageable enemy;
		if (other.TryGetComponent<IDamageable>(out enemy))
		{
			if (targets.Contains(other.gameObject))
			{
				targets.Remove(other.gameObject);
			}
		}
	}
	override public string GetKey()
	{
		return GetType().ToString();
	}
}
