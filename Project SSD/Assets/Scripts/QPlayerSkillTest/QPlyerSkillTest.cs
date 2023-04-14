using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class QPlyerSkillTest : MonoBehaviour
{
	public DecalProjector skillDistanceArea;
	public DecalProjector skillRangeArea;
	public List<Skill> skills;
	public List<bool> skillQuickUse;
	bool isLookSkillTarget = false;
	Skill usingSkill;

	Vector3 mousePos, transPos, targetPos;

	private void Update()
	{
		if (isLookSkillTarget)
		{
			mousePos = Input.mousePosition;
			transPos = Camera.main.ScreenToWorldPoint(mousePos);
			targetPos = new Vector3(transPos.x, transPos.y, 3f);
			skillRangeArea.transform.position = targetPos;

		}
	}
	public void OnSkill(int num)
	{
		if (num > skills.Count) return;
		Skill selectSkill = skills[num];	// 사용하고자 하는 스킬 가져오기
		if (usingSkill == null) usingSkill = selectSkill; // 널이면 바로 넣기

		if (usingSkill == selectSkill)
		{
			if (usingSkill.property.ready)	// 조준 해야하는 스킬?
			{
				if (usingSkill.property.quickUse) // 바로 사용 ?
				{
					print(num + "번 [조준스킬] [퀵 사용] 사용!!!");
				}
				else
				{
					if (isLookSkillTarget)	// 조준 하는 중?
					{
						print(num + "번 [조준스킬] [조준 후] 사용!!!");
						SkillAreaDisable();
					}
					else SkillAreaEnable();
				}
			}
			else // 즉발 스킬
			{
				print(num + "번 [즉발스킬] 사용!!!");
			}
		}
		else
		{
			usingSkill = selectSkill;
			SkillAreaDisable();
			OnSkill(num);
		}
	}
	void SkillAreaEnable()
	{
		float distance = usingSkill.area.skillDistance * 2f;
		float range = usingSkill.area.skillRange;
		isLookSkillTarget = true;
		skillDistanceArea.size = new Vector3(distance, distance, 100f);
		skillRangeArea.size = new Vector3(range, range, 100f);
		skillDistanceArea.enabled = true;
		skillRangeArea.enabled = true;
	}
	void SkillAreaDisable()
	{
		isLookSkillTarget = false;
		skillDistanceArea.enabled = false;
		skillRangeArea.enabled = false;
	}
}
