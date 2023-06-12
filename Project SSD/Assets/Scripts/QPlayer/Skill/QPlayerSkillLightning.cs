using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QPlayerSkillLightning : Skill
{
	public SkillOptionInformation[] options = new SkillOptionInformation[8];
	[SerializeField] private GameObject lightningStrikePrefab;
	private string effectKey;
	private string lightningStrikePrefabKey;
	private float usingSp = 50f;

	/*
		작업 목록

		작업 대기

		사전 작업 필요
		3 - 적 기절		- 3초 # 적 기절시키는 기능 구현

		작업 완료
		0 - 데미지 증가 - 50%
		1 - 선딜 감소 - QPlayer에서 참조하여 사용할 예정
		2 - TPlayer sp 회복량 상승 - 5초 , 30%
		4 - 지속 피해	- 5초, 최종데미지 50%
		5 - 넓이 증가 - 30% 증가
		6 - 집중 공격 - 활성화된 flagit 에서 타겟방향으로 같은 공격을 함
		7 - 애너지 파
	*/
	private void Awake()
	{
		options[0].name = "고전압";
		options[0].info = "데미지 증가 50%";
		options[1].name = "빠른 공격";
		options[1].info = "선딜 감소";
		options[2].name = "자극제";
		options[2].info = "파트너의 스테미너 회복량을 30% 증가시킵니다";
		options[3].name = "감전";
		options[3].info = "적을 3초간 기절 시킵니다";
		options[4].name = "잔류";
		options[4].info = "5초동안 적에게 데미지의 50%만큼 지속 피해를 가합니다";
		options[5].name = "과전류";
		options[5].info = "폭이 증가합니다";
		options[6].name = "자성";
		options[6].info = "근처의 Flagit에서 목표지점으로 추가 공격을 가합니다";
		options[7].name = "레일건";
		options[7].info = "아주 강한 공격으로 바뀝니다";
		//options[0].active = true;
		//options[1].active = true;
		//options[2].active = true;
		//options[3].active = true;
		//options[4].active = true;
		//options[5].active = true;
		options[6].active = true;
		//options[7].active = true;
	}
	private void Start()
	{
		effectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
		lightningStrikePrefabKey = lightningStrikePrefab.GetComponent<IPoolableObject>().GetKey();

		PoolerManager.instance.InsertPooler(effectKey, info.effect, false);
		PoolerManager.instance.InsertPooler(lightningStrikePrefabKey, lightningStrikePrefab, false);
	}
	public override void Use(Vector3 target)
	{
		target += Vector3.up;
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

		lightningObj.transform.position = QPlayer.instance.transform.position + Vector3.up;
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
				Effect_Lightning subLightning = subLightningObj.GetComponent<Effect_Lightning>();
				subLightningObj.transform.position = Effect_Flagit.inSceneObj[i].lightningMuzzle.position;

				subLightningObj.transform.position = new Vector3(subLightningObj.transform.position.x, lightning.transform.position.y, subLightningObj.transform.position.z);

                subLightningObj.transform.LookAt(target);
				Vector3 subLightningRot = subLightningObj.transform.eulerAngles;
				subLightningRot.x = 0;
				subLightningRot.z = 0;
				subLightningObj.transform.eulerAngles = subLightningRot;
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
