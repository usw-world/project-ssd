using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_FightGhostFistExplosion : MonoBehaviour, IPoolableObject
{
	public void Run(float damageAmount)
	{
		// 오버렙으로 데미지 주기
	}
	public string GetKey()
	{
		return GetType().ToString();
	}
}
