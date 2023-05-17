using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class QPlayerSkillAoe : Skill
{
    public SkillOptionInformation[] options = new SkillOptionInformation[4];

    public float AoeRadius;
    public GameObject cosmic;
    public float option00_increaseCastingSpeed;
    public float option01_decreaseSkillCoolDown;
    public float option02_increaseAoeAtk;
    public float option03_decreaseEnemyMoveSpeed;
    public float option04_canMoveAoe;
    public float option05_generateBlackHole;
    public float option06_holdingAndDamage;
    public float option07_throwBlackHoleAndExplosion;

    private string infoEffectKey;
    private string cosmicEffectKey;


    private void Start()
    {
        infoEffectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
        cosmicEffectKey = cosmic.GetComponent<IPoolableObject>().GetKey();
        PoolerManager.instance.InsertPooler(infoEffectKey, info.effect, false);
        PoolerManager.instance.InsertPooler(cosmicEffectKey, cosmic, false);
    }
    public override bool CanUse()
    {
        return true;
    }

    public override void Use(Vector3 target)
    {
        SummonAoe(target);

        if (options[0].active)
        {

        }
    }

    private void SummonAoe(Vector3 target)
    {
        GameObject aoe = PoolerManager.instance.OutPool(infoEffectKey);
        GameObject cosmic = PoolerManager.instance.OutPool(cosmicEffectKey);
        aoe.transform.position = target;
        cosmic.transform.position = target + new Vector3(0, 2f);
    }

    public override string GetAnimationTigger()
    {
        return "1H Casting";
    }
}
