using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBall : MonoBehaviour
{
	float damage;
	float speed;
	public void OnActive(float damage, float speed)
	{
		this.damage = damage;
		this.speed = speed;
	}
	private void Update()
	{
		transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}
	public void AddDebuff() {
		// 매개변수로 디버프를 받아 목록에 추가한다 
	}
}
