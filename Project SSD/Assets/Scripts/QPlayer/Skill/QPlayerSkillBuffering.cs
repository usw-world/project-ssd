using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class QPlayerSkillBuffering : Skill
{
    public SkillOptionInformation[] options = new SkillOptionInformation[8];
    // SkillInfo 수정 예정
    public GameObject wheelCreator;
    public bool canUseChain = false;
    public SkillInfo info;
    
    
    private float option00_increaseSpeed;
    private float option01_increaseRadius;
    private int option02_Throwable;
    private float option03_ReduceCooldown;
    private int option05_increaseQuantity;
    private float option06_rolling;
    private float option07_stack;
    private string wheelCreatorKey;
    private Skill_Buffering bufferingCreator;
    private Attachment boostAttachment;


	// 기본 - 1개, 2바퀴

	// 1. 속도 증가
	// 2. 반경 증가
	// 3. 체인스킬로 변경 - 발사됨
	// 4. 쿨타임 감소
	// 5. 지속 시간 2배
	// 6. 4개
	// 7. 공격 명중시 실드 
	// 8. 공격력 증가 100 % 3초

    private void InitializeOptionInfo()
    {
        options[0].name = "속도 증가";
        options[1].name = "거리 증가";
        options[2].name = "체인 활성화";
        options[3].name = "쿨타임 감소";
        options[4].name = "지속시간 증가";
        options[5].name = "수량 증가";
        options[6].name = "실드 부여";
        options[7].name = "공증 수정중";
    }
	private void Awake()
    {
        wheelCreatorKey = wheelCreator.GetComponent<IPoolableObject>().GetKey();
        PoolerManager.instance.InsertPooler(wheelCreatorKey, wheelCreator, false);
        property.nowCoolTime = 15;
        property.coolTime = 15;
        property.ready = false;
        info = new SkillInfo();
        InitializeOptionInfo();
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
        bufferingCreator = obj.GetComponent<Skill_Buffering>();
        bufferingCreator.parent = this;
        obj.SetActive(true);
        bufferingCreator.target = this.transform;
        
        if (options[0].active)
            bufferingCreator.speed += option00_increaseSpeed;
        if (options[1].active)
            bufferingCreator.IncreaseDistance();
        if (options[2].active)
            canUseChain = true;
        if (options[3].active)
            property.coolTime = 1;
        else
            property.coolTime = 15;
        if (options[4].active)
            bufferingCreator.maxDegree *= 2;
        if (options[5].active)
            bufferingCreator.quantity += option05_increaseQuantity;
        if(options[6].active)
            bufferingCreator.AddHitShield();
        if (options[7].active)
            bufferingCreator.BoostDamage(boostAttachment);
        for(int i=0; i < options.Length; i++)
            Debug.Log(options[i].active);
        bufferingCreator.OnActive();
        
    }

    private void OptionValueInitialize()
    {
        option00_increaseSpeed = 1.25f;
        option01_increaseRadius = 2f;
        option05_increaseQuantity = 3;
        boostAttachment = new Attachment(.5f, .5f, info.skillImage, EAttachmentType.boost);
    }

    

    public override bool CanUse()
    {
        if (canUseChain)
        {
            bufferingCreator.throwable = true;
            bufferingCreator.degree = 0;
            bufferingCreator.maxDegree = 180;
            return false;
        }
        
        if (property.nowCoolTime >= property.coolTime)
        {
            Debug.Log("buffer active");
            property.nowCoolTime = 0;
            return true;
        }

        Debug.Log("buffer in cooldown :"+ (property.coolTime-property.nowCoolTime));
        return false;
    }

	public override AimType GetAimType()
	{
        return AimType.None;
	}

	public override SkillSize GetAreaAmout()
	{
        return new SkillSize();
	}
}
