using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlyerSkill : Skill
{
	// type = 투사체
	public float speed;

	public bool isMultiple;			// 다수 발사 여부
	public int multipleCount;		// 다수 발사 회수
	public float multipleInterval;	// 다수 발사 간격

	public bool isPenetration;		// 관통 여부
	public int penetrationCount;    // 관통 횟수

	// type = 장판
	public bool isContinuation; // 지속 장판
	public float continuationTime; // 지속 시간


	// 디버프
	public bool isDebuff;                   // 디버프
	public QPlayerDebuffType debuffType;    // 디버프 타입
	public float debuffTime;                // 디버프 시간
	public float debuffAmount;              // 디버프 량

	/////////////////////////////////////////////////
	// 체인
	public bool isChain;    // 체인 스킬
	public int chainCount;  // 체인 스킬 횟수
	public float chainInterval; // 다음 사용 대기 시간

	// 캐스팅
	public bool isCasting;				// 캐스팅
	public GameObject castingEffect;	// 캐스팅 이팩트
	public float castingTime;			// 캐스팅 시간

	// 버프
	public bool isBuff;					// 버프
	public QPlayerBuffType buffType;	// 버프 타입
	public float buffTime;				// 버프 시간
	public float buffAmount;            // 버프 량

	



	[SerializeField] float usingSp = 0;
	public override void Use(Vector3 target)
	{
		QPlayer.instance.status.sp -= usingSp;

		GameObject effect = Instantiate(info.effect);
		effect.transform.position = target;
		effect.GetComponent<SkillEffect>()?.OnActive(property);
	}
	public override bool CanUse()
	{
		if (QPlayer.instance.status.sp >= usingSp) {
			if (property.nowCoolTime >= property.coolTime)
			{
				return true;
			}
		}
		return false;
	}
}
public enum OptionType {
	none,
	active,
	buff,
	debuff
}
public enum QPlayerActiveType
{
	none,       // 없음
	big
}
public enum QPlayerBuffType
{
	none,		// 없음
	healing,	// 힐
	shield,		// 쉴드
	boost		// 공격력 상승
}
public enum QPlayerDebuffType
{
	none,		// 없음
	damage,		// 데미지 (지속)
	slow,		// 슬로우
	inability	// 기절
}