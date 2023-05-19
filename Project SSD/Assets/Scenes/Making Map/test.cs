using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        GetComponent<Animator>().SetTrigger("start");
    }
}
