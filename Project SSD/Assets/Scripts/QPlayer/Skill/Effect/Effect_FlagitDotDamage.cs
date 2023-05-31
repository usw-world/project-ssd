using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_FlagitDotDamage : MonoBehaviour, IPoolableObject
{
	public string GetKey()
	{
		return GetType().ToString();
	}
}
