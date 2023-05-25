using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour, IPoolableObject
{
    public float strength;
    public Skill_wheel skill_Wheel = null;

    Wheel(float strength)
    {
        this.strength = strength;
    }
    public Wheel(float strength, Skill_wheel skill_Wheel)
    {
        this.strength= strength;
        this.skill_Wheel = skill_Wheel;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Environment"))
        {
            other.transform.Translate(-other.transform.forward * strength * 0.02f);
            // 문제생기면 addForce로 변경 예정
            if (skill_Wheel != null)
                this.gameObject.SetActive(false);
        }
    }

    private void Update()
    {

        transform.Rotate((new Vector3(0, 5, 0)));
    }

    public string GetKey()
    {
        return GetType().ToString();
    }
}
