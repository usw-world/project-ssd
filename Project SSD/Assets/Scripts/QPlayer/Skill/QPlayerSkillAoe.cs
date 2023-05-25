using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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
    public float option04_canMoveAoe;
    public GameObject option04_effect;
    public float option05_generateBlackHole;
    public GameObject option05_effect;
    public float option06_holdingAndDamage;
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
        ActiveOption00();
        ActiveOption01();
        ActiveOption02();
        ActiveOption03();
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
        AoeAttackDamage.GetInstance().damageAmount = damageAmount;
        if (options[4].active)
        {
            GameObject aoeHoldObj = PoolerManager.instance.OutPool(option04_effectKey);
            AoeHold aoeHold = aoeHoldObj.GetComponent<AoeHold>();

            aoeHold.transform.position = target;
            if (options[6].active)
            {
                AoeAttackDamage.GetInstance().isMultipleAttack = true;
            }
            else if (options[7].active)
            {
                AoeAttackDamage.GetInstance().isExplosion = true;
            }
        }
        else if (options[5].active)
        {

            GameObject aoeBlackHoleObj = PoolerManager.instance.OutPool(option05_effectKey);
            AoeBlackHole aoeBlackHole = aoeBlackHoleObj.GetComponent<AoeBlackHole>();

            aoeBlackHole.transform.position = target;
            
            if (options[6].active)
            {
                AoeAttackDamage.GetInstance().isMultipleAttack = true;
            }
            else if (options[7].active)
            {
                AoeAttackDamage.GetInstance().isExplosion = true;
            }
        }
        else
        {
            GameObject aoeObj = PoolerManager.instance.OutPool(infoEffectKey);
            Aoe aoe = aoeObj.GetComponent<Aoe>();

            aoe.transform.position = target;
        }
    }
    private void ActiveOption00()
    {
        if (options[0].active)
        {
            Attachment attachment = new Attachment(option00_buffTime, 1.0f, options[0].image, EAttachmentType.boost);
            
            if (isGiveBoostBuff == false)
            {
                attachment.onAction += (gameObject) =>
                {
                   // TPlayer.instance.apBoost = option00_increaseTPlayerAp;    // 공격력 5% 상승
                   isGiveBoostBuff = true;
                };
            }
            attachment.onInactive += (gameObject) =>
            {
                isGiveBoostBuff = false;
            };

            TPlayer.instance?.AddAttachment(attachment);
        }
    }
    private void ActiveOption01()
    {
        if (options[1].active && isUseDecreaseCoolDown == false)
        {
            property.coolTime = property.coolTime - (property.coolTime / 10);
            isUseDecreaseCoolDown = true;
        }
    }

    private void ActiveOption02()
    {
        if (options[2].active && TPlayer.instance != null)
        {
            Attachment attachment = new Attachment(option02_buffTime, 1.0f, options[2].image, EAttachmentType.shield);
            TPlayerShield shield = new TPlayerShield(TPlayer.instance.status.maxHp / 10f);

            if (isGiveShieldBuff == false)
            {
                attachment.onAction += (gameObject) =>
                {
                    TPlayer.instance?.AddShield(shield);
                    isGiveShieldBuff = true;
                };
            }
            attachment.onInactive += (gameObject) =>
            {
                TPlayer.instance.RemoveShield(shield);
                isGiveShieldBuff = false;
            };
            TPlayer.instance?.AddAttachment(attachment);
        }
    }

    private void ActiveOption03()
    {
        float spTemp = 0f;
        if (options[3].active && TPlayer.instance != null)
        {
            Attachment attachment = new Attachment(option03_buffTime, 1.0f, options[3].image, EAttachmentType.slow);

            if (isGiveRecoverySpBuff == false)
            {
                attachment.onAction += (gameObject) =>
                {
                    spTemp = TPlayer.instance.status.recoverySp;
                    TPlayer.instance.status.recoverySp = 3f;
                    isGiveRecoverySpBuff = true;
                };
            }
            attachment.onInactive += (gameObject) =>
            {
                TPlayer.instance.status.recoverySp = spTemp;
                isGiveRecoverySpBuff = false;
            };
            TPlayer.instance?.AddAttachment(attachment);
        }
    }
}
