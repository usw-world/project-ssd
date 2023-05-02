using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBallLastExplosion : MonoBehaviour
{
	float damage;
	public void OnActive(float damage) {
		this.damage = damage;
	}
}
