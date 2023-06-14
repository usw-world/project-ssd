using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotate;
	private void Update()
	{
		transform.Rotate(rotate * Time.deltaTime);
	}
}
