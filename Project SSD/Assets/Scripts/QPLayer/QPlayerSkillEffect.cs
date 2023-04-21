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
		Invoke("HideGameobject", runTime);
	}
	void HideGameobject()
	{
		gameObject.SetActive(false);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			print("명중");
			IDamageable target = other.GetComponent<IDamageable>();
			float amount = property.skillAP * qPlayer.GetAP();
			target?.OnDamage(gameObject, amount);
		}
	}
}
