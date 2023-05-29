using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillWheel : Skill
{
    public SkillOptionInformation[] options = new SkillOptionInformation[8];
    // SkillInfo 수정 예정
    public GameObject wheelCreator;
    public float speed;
    public float option00_increaseSpeed;
    public float option01_increaseRadius;
    public float option02_increaseDurationTime;
    public int option03_addQuantity;
    public float option04_increaseStrength;
    public float option05_increaseScale;
    public float option06_rolling;
    public float option07_stack;
    private string wheelCreatorKey;


    private void Awake()
    {
        wheelCreatorKey = wheelCreator.GetComponent<IPoolableObject>().GetKey();
        PoolerManager.instance.InsertPooler(wheelCreatorKey, wheelCreator, false);
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Use();
    }

    public override void Use()
    {
        OptionValueInitialize();
        var obj = PoolerManager.instance.OutPool(wheelCreatorKey);
        var test = obj.GetComponent<Skill_wheel>();

        obj.SetActive(true);
        test.target = this.transform;

        if (options[7].active)
            test.Active_stack();
        if (options[0].active)
            test.speed += option00_increaseSpeed;
        if (options[1].active)
            test.distance += option01_increaseRadius;
        if (options[2].active)
            test.maxDegree += option02_increaseDurationTime;
        if (options[3].active)
            test.quantity += option03_addQuantity;
        if (options[4].active)
            test.strength += option04_increaseStrength;
        if (options[5].active)
            test.scale += option05_increaseScale;
        if (options[6].active)
            test.Active_rolling();
        for(int i=0; i < options.Length; i++)
            Debug.Log(options[i].active);
        test.OnActive();
    }

    private void OptionValueInitialize()
    {
        option00_increaseSpeed = 100f;
        option01_increaseRadius = 0.5f;
        option02_increaseDurationTime = 180f;
        option03_addQuantity = 1;
        option04_increaseStrength = 0.5f;
        option05_increaseScale = 0.8f;

    }

    

    public override bool CanUse()
    {
        throw new System.NotImplementedException();
    }
}
