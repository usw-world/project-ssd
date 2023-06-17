using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Flower_throwObj : MonoBehaviour, IPoolableObject
{
    public Vector3 targetPos;
    public string GetKey()
    {
        return GetType().ToString();
    }

    private void Update()
    {
        var rot = Vector3.Scale(new Vector3(1, 0, 1), targetPos);
        transform.LookAt();
        transform.Translate(Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2));
    }
}
