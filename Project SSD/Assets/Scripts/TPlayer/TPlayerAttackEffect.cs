using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerAttackEffect : MonoBehaviour
{
	public GameObject test;
	public bool testing = false;
	[SerializeField] Mode mode;
	[SerializeField] GameObject hitEffect;
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
	private void Start() {
		OnActive(new SkillProperty());
	}
	public void OnActive(SkillProperty property)
	{
		transform.parent = tPlayer.transform;
		transform.localPosition = localPos;
		transform.localEulerAngles = localRot;
		transform.parent = null;

		Invoke("HideGameobject", runTime);

		Vector3 position = Vector3.zero;
		Collider[] hit = null;

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

				if (target != null)
				{
					Damage damage = new Damage(
						this.gameObject,
						amount,
						.3f,
						(hit[i].transform.position - transform.position).normalized * .5f,
						Damage.DamageType.Normal
					);
					target.OnDamage(damage);

					GameObject temp = Instantiate(hitEffect, hit[i].transform);
					temp.transform.position += Vector3.up;
					temp.transform.parent = null;
				}
				
			}
		}
	}
}
enum Mode
{
	Nomal, Dodge
}