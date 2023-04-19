using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerAttackEffect : SkillEffect
{
    [SerializeField] Vector3 damageZoneSize;
    [SerializeField] Vector3 localPos;
    [SerializeField] Vector3 localRot;
    [SerializeField] float runTime = 0.5f;
	[SerializeField] Transform tPlayer;
    List<IDamageable> targets = new List<IDamageable>();

	void Awake() => tPlayer = TPlayer.instance.gameObject.transform; 
	void HideGameobject()
	{
		gameObject.SetActive(false);
		targets.Clear();
	}
	public override void OnActive(SkillProperty property)
	{
		this.property = property;

		Transform temp = transform.parent;
		transform.parent = tPlayer;
		transform.localPosition = localPos;
		transform.localEulerAngles = localRot;
		transform.parent = temp;

		Invoke("HideGameobject", runTime);

		Vector3 position = tPlayer.transform.position - tPlayer.transform.forward + tPlayer.transform.forward + tPlayer.transform.forward + Vector3.up;

		Collider[] hit = Physics.OverlapBox(position, damageZoneSize, tPlayer.transform.rotation, 1 << 8);

		if (hit != null)
		{
			for (int i = 0; i < hit.Length; i++)
			{
				IDamageable target = hit[i].GetComponent<IDamageable>();
				float amount = property.skillAP * tPlayer.GetComponent<IGetAPable>().GetAP();
				target?.OnDamage(gameObject, amount);
			}
		}
	}
}
