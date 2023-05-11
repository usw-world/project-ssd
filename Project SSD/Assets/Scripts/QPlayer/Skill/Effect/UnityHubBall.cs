using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityHubBall : UnityBall
{
	public string subUnityBallPoolerKey;

	protected override void OnDisable()
	{
		for (int i = 0; i < 8; i++)
		{
			float y = 0;
			switch (i)
			{
				case 0: y = 0; break;
				case 1: y = 45f; break;
				case 2: y = 90f; break;
				case 3: y = 135f; break;
				case 4: y = 180f; break;
				case 5: y = 225f; break;
				case 6: y = 270; break;
				case 7: y = 315; break;
			}
			GameObject obj = PoolerManager.instance.OutPool(subUnityBallPoolerKey);
			obj.transform.position = transform.position;
			obj.transform.eulerAngles = new Vector3(0, y, 0);
			UnityBall temp = obj.GetComponent<UnityBall>();
			temp.OnActive(damageAmount, speed);
			print("isHoming : " + isHoming);
			if (isHoming)
			{
				temp.OnActiveGuided();
			}
		}
		base.OnDisable();
	}
	public void SetSubObjectKey(string subUnityBallPoolerKey)
	{
		this.subUnityBallPoolerKey = subUnityBallPoolerKey;
	}
}
