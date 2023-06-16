using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEffectPooling : TrackEffect, IPoolableObject
{
	protected override void Start()
	{
		base.Start();
		PoolerManager.instance.InsertPooler(GetKey(), this.gameObject, false);
	}
	public override void Enable()
	{
		GameObject obj = PoolerManager.instance.OutPool(GetKey());
		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;
		obj.GetComponent<TrackEffectPooling>().enabled = false;
		StartCoroutine(InPool(obj));
	}
	public override void Disable()
	{

	}
	private IEnumerator InPool(GameObject obj)
	{
		yield return new WaitForSeconds(4f);
		PoolerManager.instance.InPool(GetKey(), obj);
	}
	public string GetKey()
	{
		return GetType().ToString();
	}
}
