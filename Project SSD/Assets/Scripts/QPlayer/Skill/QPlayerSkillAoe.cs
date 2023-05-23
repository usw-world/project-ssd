using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class QPlayerSkillAoe : Skill
{
    public SkillOptionInformation[] options = new SkillOptionInformation[8];

    public GameObject cosmic;
    public float option00_increaseTPlayerAp;
    public float option00_buffTime;
    public float option01_decreaseSkillCoolDown;
    public float option02_increaseAoeAp;
    public float option03_decreaseEnemyMoveSpeed; // 미정
    public float option04_canMoveAoe;
    public GameObject option04_effect;
    public float option05_generateBlackHole;
    public float option06_holdingAndDamage;
    public float option07_throwBlackHoleAndExplosion;

    private string infoEffectKey;
    private string option04_effectKey;
    
    private bool isUseDecreaseCoolDown = false;

    private float basicDamage = 10f;
    private float damageAmout
    {
        get
        {
            float amount = basicDamage * property.skillAP;
            return amount;
        }
        set
        {
            property.skillAP = value;
        }
    }
    private void Start()
    {
        infoEffectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
        option04_effectKey = option04_effect.GetComponent<IPoolableObject>().GetKey();

        PoolerManager.instance.InsertPooler(infoEffectKey, info.effect, false);
        PoolerManager.instance.InsertPooler(option04_effectKey, option04_effect, false);
    }

    public override bool CanUse()
    {
        return true;
    }

    public override void Use(Vector3 target)
    {
        ActiveOption01();
        ActiveOption02();
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
        if (options[4].active)
        {
            GameObject aoeHoldObj = PoolerManager.instance.OutPool(option04_effectKey);
            AoeHold aoeHold = aoeHoldObj.GetComponent<AoeHold>();

            aoeHold.transform.position = target;
            aoeHold.OnActive(damageAmout);
        }
        else if (options[5].active)
        {

        }
        else
        {
            GameObject aoeObj = PoolerManager.instance.OutPool(infoEffectKey);
            Aoe aoe = aoeObj.GetComponent<Aoe>();

            ActiveOption00(aoe);

            aoe.transform.position = target;
            aoe.OnActive(damageAmout);
        }
    }
    private void ActiveOption00(Aoe aoe)
    {
        if (options[0].active && aoe.isTPlayerOnAoe == false)
        {
            Attachment attachment = new Attachment(option00_buffTime, 1.0f, options[0].image);
            
            attachment.onStay += (gameObject) => 
            {
                //TPlayer.instance.apBoost = option00_increaseTPlayerAp;    // 공격력 5% 상승
            };
            attachment.onInactive += (gameObject) =>
            {
                aoe.isTPlayerOnAoe = true;
            };
            TPlayer.instance.AddAttachment(attachment);
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
        if (options[2].active)
        {
            damageAmout = 7.5f;
        }
    }

    private void ActiveOption03()
    {

    }

    public override string GetAnimationTigger()
    {
        return "1H Casting";
    }
}
