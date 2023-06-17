using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Enemy_Flower_throwObj : MonoBehaviour, IPoolableObject
{
    public Vector3 targetPos;
    public float speed = 2;
    private float timer = 0;
    public Transform parent;
    public string GetKey()
    {
        return GetType().ToString();
    }

    private void Start()
    {
        // parent = GetComponentInParent<Transform>();
    }

    private void Update()
    {
        transform.Translate(transform.forward * (10 * Time.deltaTime));
        timer += Time.deltaTime;
        if(timer > 5)
            Destroy(this.gameObject);
    }
}
