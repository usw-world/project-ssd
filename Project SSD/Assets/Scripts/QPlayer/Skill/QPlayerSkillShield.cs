using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillShield : Skill
{
	public SkillOptionInformation[] options = new SkillOptionInformation[8];

	[SerializeField] private GameObject explosionPrefab;
	private string explosionKey;
	private float usingSp = 30f;
	/*
		작업 대기

		작업 중

		사전 작업 필요

		작업 완료
			0. 실드량 상승
			1. 쿨타임 감소
			2. sp회복량 상승
			3. 지속시간 증가
			4. 잔상
			5. 폭발
			6. 무적, 반격			
			7. 무적, 힐
	*/
	private void Awake()
	{
		options[0].name = "강력한 방어";
		options[0].info = "실드량 50%";
		options[1].name = "신속한 방어";
		options[1].info = "쿨타임 5초 감소";
		options[2].name = "자극제";
		options[2].info = "파트너의 스테미너 회복량을 30% 증가시킵니다";
		options[3].name = "과보호";
		options[3].info = "실드의 지속시간 100% 증가";
		options[4].name = "공격태세";
		options[4].info = "실드가 유지되는 동안 파트너가 일반공격을 하면 잔상이 생겨 50%의 공격력으로 추가 공격을 가합니다";
		options[5].name = "마지막 보호";
		options[5].info = "실드가 사라질때 폭발하여 주변의 적을 공격합니다";
		options[6].name = "카운터";
		options[6].info = "0.5초간 파트너가 무적이 됩니다. 무적시간이 끝나면 주변 적에게 무적시간때 받은 데미지의 100%만큼의 피해를 가합니다.";
		options[7].name = "전화위복";
		options[7].info = "0.5초간 파트너가 무적이 됩니다. 무적시간이 끝나면 파트너의 HP를 무적시간때 받은 데미지의 100%만큼 회복합니다.";
		options[1].active = true;
		//options[3].active = true;
		options[4].active = true;
		options[5].active = true;
		options[7].active = true;
	}
	private void Start()
	{
		explosionKey = explosionPrefab.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(explosionKey, explosionPrefab, false);
	}
	public override void Use(Vector3 target)
	{
		QPlayer.instance.status.sp -= usingSp;

		property.nowCoolTime = 0;
		if (options[1].active) property.nowCoolTime += 5; 

		float shieldAmunt = QPlayer.instance.GetAP() * property.skillAP;
		float shieldTime = 5f;

		if (options[0].active) shieldAmunt = shieldAmunt * 1.5f;
		if (options[3].active) shieldTime = shieldTime * 2f;

		TPlayerShield shield = new TPlayerShield(shieldAmunt);
		bool isExplosion = false;

		Attachment attachment = new Attachment(shieldTime, 0.1f, info.skillImage, EAttachmentType.shield);
		attachment.onAction = (target) => {
			TPlayer.instance.AddShield(shield);
			if (options[4].active) TPlayer.instance.SetACtionMotionTrail(true);
		};
		attachment.onStay = (target) => {
			if (shield.amount <= 0 && !isExplosion)
			{
				// 실드가 공격으로 사라질때
				TPlayer.instance.RemoveShield(shield);
				isExplosion = true;
				if (options[4].active) TPlayer.instance.SetACtionMotionTrail(false);
				if (options[5].active) Explosion(shieldAmunt);
			}
		};
		attachment.onInactive = (target) => {
			TPlayer.instance.RemoveShield(shield);
			if (!isExplosion)
			{
				// 실드가 자연스럽게 사라질때
				isExplosion = true;
				if (options[4].active) TPlayer.instance.SetACtionMotionTrail(false);
				if (options[5].active) Explosion(shieldAmunt);
			}
		};

		TPlayer.instance.AddAttachment(attachment);

		if (options[6].active || options[7].active)
		{
			Attachment Invincibility = new Attachment(0.5f, 0.1f, info.skillImage, EAttachmentType.inability);
			Invincibility.onAction = (target) => {
				if (options[6].active) TPlayer.instance.SetActiveInvincibilityAndAction(true, EInvincibilityAndActionType.Counterattack);
				if (options[7].active) TPlayer.instance.SetActiveInvincibilityAndAction(true, EInvincibilityAndActionType.Healing);
			};
			Invincibility.onStay = (target) => { };
			Invincibility.onInactive = (target) => {
				if (options[6].active) TPlayer.instance.SetActiveInvincibilityAndAction(false, EInvincibilityAndActionType.Counterattack);
				if (options[7].active) TPlayer.instance.SetActiveInvincibilityAndAction(false, EInvincibilityAndActionType.Healing);
			};
			TPlayer.instance.AddAttachment(Invincibility);
		}
		if (options[2].active) RecoverySp(shieldTime); 
	}
	private void RecoverySp(float shieldTime) 
	{
		float recoverySpAmount = 0.3f;
		Attachment attachment = new Attachment(shieldTime, 1f, info.skillImage, EAttachmentType.shield);
		attachment.onAction = (target) => {
			TPlayer.instance.status.recoverySp += recoverySpAmount;
		};
		attachment.onInactive = (target) => {
			TPlayer.instance.status.recoverySp -= recoverySpAmount;
		};
		TPlayer.instance.AddAttachment(attachment);
	}
	private void Explosion(float shieldAmunt) 
	{
		GameObject explosion = PoolerManager.instance.OutPool(explosionKey);
		explosion.transform.position = TPlayer.instance.transform.position;
		explosion.GetComponent<Effect_ShieldExplosion>().Run(shieldAmunt * 0.75f);
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
