using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour, IPoolableObject
{
	[SerializeField] private EHitEffectType type;

	public string GetKey()
	{
		return GetType() + type.ToString(); ;
	}
}
public enum EHitEffectType
{
	boom_1,
	slash_1
}