using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillBuffering : Skill
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

	// 기본 - 1개, 2바퀴

	// 1. 속도 증가
	// 2. 반경 증가
	// 3. 체인스킬로 변경 - 발사됨
	// 4. 쿨타임 감소
	// 5. 지속 시간 2배
	// 6. 4개
	// 7. 공격 명중시 실드 
	// 8. 공격력 증가 100 % 3초

	private void Awake()
    {
        wheelCreatorKey = wheelCreator.GetComponent<IPoolableObject>().GetKey();
        PoolerManager.instance.InsertPooler(wheelCreatorKey, wheelCreator, false);
        property.nowCoolTime = 0;
        property.ready = false;
    }

    public override void Use(Vector3 tmp)
    {
        Use();
    }
    public override void Use()
    {
        Debug.Log("buffer use");
        OptionValueInitialize();
        var obj = PoolerManager.instance.OutPool(wheelCreatorKey);
        var BufferCreator = obj.GetComponent<Skill_wheel>();

        obj.SetActive(true);
        BufferCreator.target = this.transform;
        if (options[7].active)
            BufferCreator.Active_stack();
        if (options[0].active)
            BufferCreator.speed += option00_increaseSpeed;
        if (options[1].active)
            BufferCreator.distance += option01_increaseRadius;
        if (options[2].active)
            BufferCreator.maxDegree += option02_increaseDurationTime;
        if (options[3].active)
            BufferCreator.quantity += option03_addQuantity;
        if (options[4].active)
            BufferCreator.strength += option04_increaseStrength;
        if (options[5].active)
            BufferCreator.scale += option05_increaseScale;
        if (options[6].active)
            BufferCreator.Active_rolling();
        for(int i=0; i < options.Length; i++)
            Debug.Log(options[i].active);
        BufferCreator.OnActive();
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
        if (property.nowCoolTime >= property.coolTime)
        {
            Debug.Log("buffer active");
            return true;
        }

        Debug.Log("buffer in cooldown");
        return false;
    }
}
