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
        infoEffectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
        option04_effectKey = option04_effect.GetComponent<IPoolableObject>().GetKey();
        option05_effectKey = option05_effect.GetComponent<IPoolableObject>().GetKey();

        PoolerManager.instance.InsertPooler(infoEffectKey, info.effect, false);
        PoolerManager.instance.InsertPooler(option04_effectKey, option04_effect, false);
        PoolerManager.instance.InsertPooler(option05_effectKey, option05_effect, false);
    }

    public override bool CanUse()
    {
        return true;
    }

    public override void Use(Vector3 target)
    {
        SkillCoolDown(target);
    }

    private void SkillCoolDown(Vector3 target)
    {
        if (property.isUseSkill == false)
        {
            property.isUseSkill = true;
            SummonAoe(target);
            StartCoroutine(SkillCoolDownCoroutine(property.coolTime + 100f));
        }
        else
        {
            Debug.Log(name + "스킬이 " + (100f + property.coolTime - property.nowCoolTime) + "초 남았습니다.");
        }
    }

    IEnumerator SkillCoolDownCoroutine(float coolDown)
    {
        while (property.nowCoolTime < coolDown)
        {
            property.nowCoolTime += Time.deltaTime;
            yield return null;
        }
        property.nowCoolTime = 100f;
        property.isUseSkill = false;
    }

    private void SummonAoe(Vector3 target)
    {
        AoeAttackDamage aoeAttack = AoeAttackDamage.GetInstance();
        AoeBuffer aoeBuffer = AoeBuffer.GetInstance();

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

            aoe.transform.position = target;
        }
    }
    private void ActiveOption00()   // 공격력 5% 상승
    {
        float apBoostTemp = 0f;
        AoeBuffer aoeBuffer = AoeBuffer.GetInstance();
        Attachment attachment = new Attachment(option00_buffTime, 1.0f, options[0].image, EAttachmentType.boost);

        if (isGiveBoostBuff == false)
        {
            attachment.onAction = (gameObject) =>
            {
                apBoostTemp = TPlayer.instance.status.apBoost;
                TPlayer.instance.status.apBoost = option00_increaseTPlayerAp;    
                isGiveBoostBuff = true;
            };
        }
        attachment.onInactive = (gameObject) =>
        {
            aoeBuffer.onEnter -= ActiveOption00;
            TPlayer.instance.status.apBoost = apBoostTemp;
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
        AoeBuffer aoeBuffer = AoeBuffer.GetInstance();
        Attachment attachment = new Attachment(option02_buffTime, 1.0f, options[2].image, EAttachmentType.shield);
        TPlayerShield shield = new TPlayerShield(TPlayer.instance.status.maxHp / option02_generateTPlayerShield);

        if (isGiveShieldBuff == false)
        {
            attachment.onAction = (gameObject) =>
            {
                TPlayer.instance?.AddShield(shield);
                isGiveShieldBuff = true;
            };
        }
        attachment.onInactive = (gameObject) =>
        {
            aoeBuffer.onEnter -= ActiveOption02;
            TPlayer.instance?.RemoveShield(shield);
            isGiveShieldBuff = false;
        };
        TPlayer.instance?.AddAttachment(attachment);
    }

    private void ActiveOption03()   // TPlayer SP 회복 속도 10% 증가
    {
        float spTemp = 0f;
        AoeBuffer aoeBuffer = AoeBuffer.GetInstance();
        Attachment attachment = new Attachment(option03_buffTime, 1.0f, options[3].image, EAttachmentType.boost);

        if (isGiveRecoverySpBuff == false)
        {
            attachment.onAction = (gameObject) =>
            {
                spTemp = TPlayer.instance.status.recoverySp;
                TPlayer.instance.status.recoverySp = option03_increaseTPlayerRecoverySp;
                isGiveRecoverySpBuff = true;
            };
        }
        attachment.onInactive = (gameObject) =>
        {
            aoeBuffer.onEnter -= ActiveOption03;
            TPlayer.instance.status.recoverySp = spTemp;
            isGiveRecoverySpBuff = false;
        };
        TPlayer.instance?.AddAttachment(attachment);
    }
}
