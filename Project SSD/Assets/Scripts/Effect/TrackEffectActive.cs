using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEffectActive : TrackEffect
{
	public override void Enable()
	{
		Update();
		gameObject.SetActive(true);
	}
	public override void Disable()
	{
		Update();
		gameObject.SetActive(false);
	}
}
