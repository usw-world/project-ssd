using System.Collections;
using UnityEngine;

public class QPlayerSkillAoe : Skill
{
    public SkillOptionInformation[] options = new SkillOptionInformation[8];

    public float option00_increaseTPlayerAp;
    public float option00_buffTime;
    public float option01_decreaseSkillCoolDown;
    public float option02_generateTPlayerShield;
    public float option02_buffTime;
    public float option03_increaseTPlayerRecoverySp;
    public float option03_buffTime;
    [HideInInspector] public float option04_canMoveAoe;
    public GameObject option04_effect;
    [HideInInspector] public float option05_generateBlackHole;
    public GameObject option05_effect;
    public float option06_holdingAndDamage;
    public int option06_damageFrequency;
    public float option07_aoeExplosion;

    private string infoEffectKey;
    private string option04_effectKey;
    private string option05_effectKey;
    
    private bool isUseDecreaseCoolDown = false;
    private bool isGiveBoostBuff = false;
    private bool isGiveShieldBuff = false;
    private bool isGiveRecoverySpBuff = false;
    private float damageAmount
    {
        get
        {
            float amount = 10f * property.skillAP;
            return amount;
        }
        set { }
    }

    private void Awake()
    {
		//options[0].active = false;	 // 
		//options[1].active = false;	 // 
		//options[2].active = false;	 // 
		//options[3].active = false;	 // 
		//options[4].active = true;	 // 가두기
		//options[5].active = false;	 // 모으기
		//options[6].active = false;	 // 
		//options[7].active = false;	 // 폭발
	}
	private void Start()
	{
		infoEffectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
		option04_effectKey = option04_effect.GetComponent<IPoolableObject>().GetKey();
		option05_effectKey = option05_effect.GetComponent<IPoolableObject>().GetKey();

		PoolerManager.instance.InsertPooler(infoEffectKey, info.effect, false);
		PoolerManager.instance.InsertPooler(option04_effectKey, option04_effect, false);
		PoolerManager.instance.InsertPooler(option05_effectKey, option05_effect, false);
	}
	public override bool CanUse()
    {
        if (property.nowCoolTime >= property.coolTime)
        {
            return true;
        }
        return false;
    }
    public override void Use(Vector3 target)
    {
        SummonAoe(target);
    }
    private void SummonAoe(Vector3 target)
    {
        AoeAttackDamage aoeAttack = AoeAttackDamage.GetInstance();
        AoeBuffer aoeBuffer = AoeBuffer.GetInstance();

        property.nowCoolTime = 0;
        if (options[0].active)
        {
            aoeBuffer.onEnter += ActiveOption00;
        }
        else if (options[1].active && isUseDecreaseCoolDown == false)
        {
            ActiveOption01();
        }
        if (options[2].active && TPlayer.instance != null)
        {
            aoeBuffer.onEnter += ActiveOption02;
        }
        else if (options[3].active && TPlayer.instance != null)
        {
            aoeBuffer.onEnter += ActiveOption03;
        }

        aoeAttack.damageAmount = damageAmount;
        if (options[4].active)
        {
            GameObject aoeHoldObj = PoolerManager.instance.OutPool(option04_effectKey);
            AoeHold aoeHold = aoeHoldObj.GetComponent<AoeHold>();
            aoeHold.Run();

            aoeHold.transform.position = target;
            if (options[6].active)
            {
                aoeAttack.multipleAttackDamage = option06_holdingAndDamage;
                aoeAttack.damageFrequency = option06_damageFrequency;
                aoeAttack.isMultipleAttack = true;
            }
            else if (options[7].active)
            {
                aoeAttack.explosionDamage = option07_aoeExplosion;
                aoeAttack.isExplosion = true;
            }
        }
        else if (options[5].active)
        {
            GameObject aoeBlackHoleObj = PoolerManager.instance.OutPool(option05_effectKey);
            AoeBlackHole aoeBlackHole = aoeBlackHoleObj.GetComponent<AoeBlackHole>();
            aoeBlackHole.Run();

            aoeBlackHole.transform.position = target;
            
            if (options[6].active)
            {
                aoeAttack.multipleAttackDamage = option06_holdingAndDamage;
                aoeAttack.damageFrequency = option06_damageFrequency;
                aoeAttack.isMultipleAttack = true;
            }
            else if (options[7].active)
            {
                aoeAttack.explosionDamage = option07_aoeExplosion;
                aoeAttack.isExplosion = true;
            }
        }
        else
        {
            GameObject aoeObj = PoolerManager.instance.OutPool(infoEffectKey);
            Aoe aoe = aoeObj.GetComponent<Aoe>();
            aoe.Run();

            aoe.transform.position = target;
        }
    }
    private void ActiveOption00()   // 공격력 5% 상승
    {
        AoeBuffer aoeBuffer = AoeBuffer.GetInstance();
        Attachment attachment = new Attachment(option00_buffTime, 1.0f, options[0].image, EAttachmentType.boost);

        if (isGiveBoostBuff == false)
        {
            attachment.onAction = (gameObject) =>
            {
                TPlayer.instance.status.apBoost += option00_increaseTPlayerAp;    
                isGiveBoostBuff = true;
            };
        }
        attachment.onInactive = (gameObject) =>
        {
            aoeBuffer.onEnter -= ActiveOption00;
            TPlayer.instance.status.apBoost -= option00_increaseTPlayerAp;
            isGiveBoostBuff = false;
        };
        TPlayer.instance?.AddAttachment(attachment);
    }
    private void ActiveOption01()   // 스킬 쿨타임 10% 감소 
    {
        property.coolTime = property.coolTime - (property.coolTime / option01_decreaseSkillCoolDown);
        isUseDecreaseCoolDown = true;
    }
    private void ActiveOption02()   // TPlayer 쉴드 최대 체력의 10%
    {
        float shieldAmount = TPlayer.instance.status.maxHp / option02_generateTPlayerShield;
        AoeBuffer aoeBuffer = AoeBuffer.GetInstance();
        Attachment attachment = new Attachment(option02_buffTime, 1.0f, options[2].image, EAttachmentType.shield);
        TPlayerShield shield = new TPlayerShield(shieldAmount);

        if (isGiveShieldBuff == false)
        {
            attachment.onAction = (gameObject) =>
            {
                TPlayer.instance?.AddShield(shield);
                TPlayer.instance.SetACtionMotionTrail(true);
                isGiveShieldBuff = true;
            };
            attachment.onStay = (gameObject) =>
            {
                if (shield.amount <= 0)
                {
                    TPlayer.instance.RemoveShield(shield);
                    TPlayer.instance.SetACtionMotionTrail(false);
                    isGiveShieldBuff = false;
                }
            };
        }
        attachment.onInactive = (gameObject) =>
        {
            aoeBuffer.onEnter -= ActiveOption02;
            TPlayer.instance?.RemoveShield(shield);
            TPlayer.instance.SetACtionMotionTrail(false);
            isGiveShieldBuff = false;
        };
        TPlayer.instance?.AddAttachment(attachment);
    }
    private void ActiveOption03()   // TPlayer SP 회복 속도 10% 증가
    {
        AoeBuffer aoeBuffer = AoeBuffer.GetInstance();
        Attachment attachment = new Attachment(option03_buffTime, 1.0f, options[3].image, EAttachmentType.boost);

        if (isGiveRecoverySpBuff == false)
        {
            attachment.onAction = (gameObject) =>
            {
                TPlayer.instance.status.recoverySp += option03_increaseTPlayerRecoverySp;
                isGiveRecoverySpBuff = true;
            };
        }
        attachment.onInactive = (gameObject) =>
        {
            aoeBuffer.onEnter -= ActiveOption03;
            TPlayer.instance.status.recoverySp -= option03_increaseTPlayerRecoverySp;
            isGiveRecoverySpBuff = false;
        };
        TPlayer.instance?.AddAttachment(attachment);
    }

	public override AimType GetAimType()
	{
        return AimType.Area;
	}

	public override SkillSize GetAreaAmout()
	{
        //if (options[5].active || options[4].active) return new SkillSize(8f, 8f);
        return new SkillSize(6f, 6f);
    }
}
