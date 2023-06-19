using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerAttachTest : MonoBehaviour
{
	float time = 0;
	public float power = 10f;
    void Update()
    {
		time += Time.deltaTime * power;
		transform.position = new Vector3(Mathf.Cos(time * Mathf.Deg2Rad), 0.5f, Mathf.Sin(time * Mathf.Deg2Rad));
    }
}
