using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillEffect : SkillEffect
{
	[SerializeField] float runTime = 0.5f;
	QPlayer qPlayer;
	private void Awake() => qPlayer = QPlayer.instance; 
	public override void OnActive(SkillProperty property)
	{
		property.nowCoolTime = 0;
		this.property = property;
		StartCoroutine(HideGameobject());
	}
	IEnumerator HideGameobject()
	{
		yield return new WaitForSeconds(0.2f);
		Collider temp = GetComponent<Collider>();
		if (temp != null) temp.enabled = false;
		yield return new WaitForSeconds(runTime);
		gameObject.SetActive(false);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			print("This message was called in 'QPlayerSkillEffect' that will be expected to removed.");
			IDamageable target = other.GetComponent<IDamageable>();
			float amount = property.skillAP * qPlayer.GetAP();
			// target?.OnDamage(gameObject, amount);
		}
	}
}
