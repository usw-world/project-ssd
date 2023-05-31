using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillShield : Skill
{
	public SkillOptionInformation[] options = new SkillOptionInformation[8];

	private GameObject explosionPrefab;
	private string explosionKey;
	private float usingSp = 30f;
	private bool bigShield = false;
	private bool reductionCoolTime = false;
	private bool recoverySp = false;
	private bool IncreasingTime = false;
	/*
		작업 대기
			5. 잔상 or 공격력 상승	
			7. 무적, 반격			8. 무적, 힐

		작업 중

		사전 작업 필요

		작업 완료
			1. 실드량 상승
			2. 쿨타임 감소
			3. sp회복량 상승
			4. 지속시간 증가
			6. 폭발
	*/
	private void Start()
	{
		explosionKey = explosionPrefab.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(explosionKey, explosionPrefab, false);
	}
	public override void Use()
	{
		QPlayer.instance.status.sp -= usingSp;

		property.nowCoolTime = 0;
		if (reductionCoolTime) property.nowCoolTime += 5; 

		float shieldAmunt = QPlayer.instance.GetAP() * property.skillAP;
		float shieldTime = 5f;

		if (bigShield) shieldAmunt = shieldAmunt * 1.5f;
		if (IncreasingTime) shieldTime = shieldTime * 2f;

		TPlayerShield shield = new TPlayerShield(shieldAmunt);
		bool isExplosion = false;

		Attachment attachment = new Attachment(shieldTime, 0.1f, info.skillImage, EAttachmentType.shield);
		attachment.onAction = (target) => {
			TPlayer.instance.AddShield(shield);
		};
		attachment.onStay = (target) => {
			if (shield.amount <= 0 && !isExplosion)
			{
				GameObject explosion = PoolerManager.instance.OutPool(explosionKey);
				explosion.transform.position = TPlayer.instance.transform.position;
				explosion.GetComponent<Effect_ShieldExplosion>().Run(shieldAmunt * 0.75f);
				isExplosion = true;
			}
		};
		attachment.onInactive = (target) => {
			TPlayer.instance.RemoveShield(shield);
			if (!isExplosion)
			{
				GameObject explosion = PoolerManager.instance.OutPool(explosionKey);
				explosion.transform.position = TPlayer.instance.transform.position;
				explosion.GetComponent<Effect_ShieldExplosion>().Run(shieldAmunt * 0.75f);
				isExplosion = true;
			}
		};
		TPlayer.instance.AddAttachment(attachment);

		if (recoverySp)
		{
			float recoverySpAmount = 0.3f;
			attachment = new Attachment(shieldTime, 1f, info.skillImage, EAttachmentType.shield);
			attachment.onAction = (target) => {
				TPlayer.instance.status.recoverySp += recoverySpAmount;
			};
			attachment.onInactive = (target) => {
				TPlayer.instance.status.recoverySp -= recoverySpAmount;
			};
			TPlayer.instance.AddAttachment(attachment);
		}
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
