using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerAttackEffect : SkillEffect
{
	public GameObject test;
	public bool testing = false;
	[SerializeField] Mode mode;
	[SerializeField] Vector3 damageZoneSize;
    [SerializeField] Vector3 localPos;
    [SerializeField] Vector3 localRot;
    [SerializeField] float runTime = 0.5f;
	[SerializeField] TPlayer tPlayer;
    List<IDamageable> targets = new List<IDamageable>();
	void Awake() => tPlayer = TPlayer.instance;
	void HideGameobject()
	{
		gameObject.SetActive(false);
		targets.Clear();
	}
	public override void OnActive(SkillProperty property)
	{
		this.property = property;

		Transform temp = transform.parent;
		transform.parent = tPlayer.transform;
		transform.localPosition = localPos;
		transform.localEulerAngles = localRot;
		transform.parent = temp;

		Invoke("HideGameobject", runTime);

		Vector3 position = Vector3.zero;
		Collider[] hit = null;
		GameObject testobj = null;

		switch (mode)
		{
			case Mode.Nomal:
				position = tPlayer.transform.position - tPlayer.transform.forward + ( tPlayer.transform.forward * 2f ) + Vector3.up;
				hit = Physics.OverlapBox(position, damageZoneSize, tPlayer.transform.rotation, 1 << 8);
				break;
			case Mode.Dodge:
				position = tPlayer.transform.position - tPlayer.transform.forward - ( tPlayer.transform.forward * 2f ) + Vector3.up;
				hit = Physics.OverlapBox(position, damageZoneSize, tPlayer.transform.rotation, 1 << 8);
				break;
		}
		if (hit != null)
		{
			for (int i = 0; i < hit.Length; i++)
			{
				IDamageable target = hit[i].GetComponent<IDamageable>();
				float amount = property.skillAP * tPlayer.GetAP();
				target?.OnDamage(gameObject, amount);
			}
		}

		if (testing)
		{
			switch (mode)
			{
				case Mode.Nomal:
					testobj = Instantiate(test, position, tPlayer.transform.rotation);
					testobj.transform.localScale = damageZoneSize;
					break;
				case Mode.Dodge:
					testobj = Instantiate(test, position, tPlayer.transform.rotation);
					testobj.transform.localScale = damageZoneSize;
					testobj.transform.parent = tPlayer.transform;
					testobj.transform.localPosition = localPos;
					testobj.transform.parent = null;
					break;
			}
		}
	}
}
enum Mode
{
	Nomal, Dodge
}