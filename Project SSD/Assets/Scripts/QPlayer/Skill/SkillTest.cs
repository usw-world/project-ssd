using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTest : MonoBehaviour
{
    public GameObject temp;
    private GameObject test;
    public bool option06Test = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            test = Instantiate(temp, transform);
            test.SetActive(true);
            test.GetComponent<Skill_wheel>().target = this.transform;
            if (option06Test)
                test.GetComponent<Skill_wheel>().Active_rolling();
            test.GetComponent<Skill_wheel>().OnActive();
        }

    }
}
