using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour, IPoolableObject
{
	[SerializeField] private EHitEffectType type;

	public string GetKey()
	{
		return GetType() + type.ToString();
	}
}
enum EHitEffectType
{
	TPlayer_normal_1
}