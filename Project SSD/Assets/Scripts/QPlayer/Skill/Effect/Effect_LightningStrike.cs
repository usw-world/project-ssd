using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_LightningStrike : Effect_Lightning
{
	override public string GetKey()
	{
		return GetType().ToString();
	}
}
