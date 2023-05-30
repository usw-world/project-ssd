using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillLightning : Skill
{
	public SkillOptionInformation[] options = new SkillOptionInformation[8];
	private GameObject lightningStrikePrefab;
	private string effectKey;
	private string lightningStrikePrefabKey;
	private float usingSp = 50f;

	/*
		작업 목록
		7 - 애너지 파

		작업 대기
		1 - 선딜 감소 - QPlayer에서 참조하여 사용할 예정

		사전 작업 필요
		3 - 적 기절		- 3초 # 적 기절시키는 기능 구현

		작업 완료
		0 - 데미지 증가 - 50%
		2 - TPlayer sp 회복량 상승 - 5초 , 30%
		4 - 지속 피해	- 5초, 최종데미지 50%
		5 - 넓이 증가 - 30% 증가
		6 - 집중 공격 - 활성화된 flagit 에서 타겟방향으로 같은 공격을 함
	*/
	private void Start()
	{
		effectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(effectKey, info.effect, false);
		lightningStrikePrefabKey = lightningStrikePrefab.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(lightningStrikePrefabKey, lightningStrikePrefab, false);
	}
	public override void Use(Vector3 target)
	{
		float lastDamage = QPlayer.instance.GetAP() * property.skillAP;

		GameObject lightningObj = null;
		Effect_Lightning lightning = null;

		if (options[7].active)
		{
			lightningObj = PoolerManager.instance.OutPool(lightningStrikePrefabKey);
			lightning = lightningObj.GetComponent<Effect_LightningStrike>();
			lastDamage = lastDamage * 3f;
		}
		else
		{
			lightningObj = PoolerManager.instance.OutPool(effectKey);
			lightning = lightningObj.GetComponent<Effect_Lightning>();
		}

		lightningObj.transform.position = QPlayer.instance.transform.position;
		lightningObj.transform.LookAt(target);

		if (options[0].active){
			lastDamage = lastDamage * 1.5f;
		}

		lightning.Initialize(lastDamage);

		if (options[2].active)
		{
			float boostAmount = 0.3f;
			Attachment attachment = new Attachment(5f, 1f, info.skillImage, EAttachmentType.boost);
			attachment.onAction = (gameObject) => {
				TPlayer.instance.status.recoverySp += boostAmount;
			};
			attachment.onInactive = (gameObject) => {
				TPlayer.instance.status.recoverySp -= boostAmount;
			};
			TPlayer.instance.AddAttachment(attachment);
		}

		if (options[3].active)
		{
			Attachment attachment = new Attachment(3f, 1f, info.skillImage, EAttachmentType.inability);

			// attachment 설정해야함 - 적을 기절시키기

			lightning.activeInability(attachment);
		}
		if (options[4].active)
		{
			float damageAmount = lastDamage * 0.5f;
			Attachment attachment = new Attachment(5f, 1f, info.skillImage, EAttachmentType.damage);
			attachment.onStay = (target) => {
				IDamageable enemy = target.GetComponent<IDamageable>();
				Damage damage = new Damage(
					damageAmount * 0.2f,
					0f, 
					Vector3.zero,
					Damage.DamageType.Normal
				);
				enemy.OnDamage(damage);
			};
			lightning.activeDamage(attachment);
		}

		if (options[5].active)
			lightning.ActiveBroadAttack(true);
		else
			lightning.ActiveBroadAttack(false);

		if (options[6].active)
		{
			for (int i = 0; i < Effect_Flagit.inSceneObj.Count; i++)
			{
				GameObject subLightningObj = PoolerManager.instance.OutPool(effectKey);
				Effect_Lightning subLightning = lightningObj.GetComponent<Effect_Lightning>();
				subLightningObj.transform.position = Effect_Flagit.inSceneObj[i].lightningMuzzle.position;
				subLightningObj.transform.LookAt(target);
				subLightning.Initialize(lightning);
				subLightning.Run();
			}
		}

		lightning.Run();
	}

	public override bool CanUse()
	{
		if (
			property.nowCoolTime >= property.coolTime &&
			QPlayer.instance.status.sp >= usingSp
			)
		{
			return true;
		}
		return false;
	}
}
