using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEffectTime : TrackEffect
{
	[SerializeField] private int rateOverTime = 5;
	public override void Enable()
	{
		ParticleSystem.EmissionModule emission = GetComponent<ParticleSystem>().emission;
		emission.rateOverTime = rateOverTime;
	}
	public override void Disable()
	{
		ParticleSystem.EmissionModule emission = GetComponent<ParticleSystem>().emission;
		emission.rateOverTime = 0f;
	}
}
