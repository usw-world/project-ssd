using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEffectActive : TrackEffect
{
	public override void Enable()
	{
		gameObject.SetActive(true);
	}
	public override void Disable()
	{
		gameObject.SetActive(false);
	}
}
