using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class QPlyerSkillTest : MonoBehaviour
{
	public DecalProjector skillDistanceArea;
	public DecalProjector skillRangeArea;
	public List<Skill> skills;
	bool isLookSkillTarget = false;
	Skill usingSkill;
	Vector3 targetPos;
	
	private void Update()
	{
		if (isLookSkillTarget)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit, 10000f, 1 << LayerMask.NameToLayer("Block")))
			{
				targetPos = hit.point;
				Vector3 temp = targetPos;
				temp.y = skillDistanceArea.transform.position.y;

				float dist = Vector3.Distance(temp, skillDistanceArea.transform.position);
				if (dist > usingSkill.area.distance)
				{
					temp = temp - skillDistanceArea.transform.position;
					temp = temp.normalized * usingSkill.area.distance;
					temp.y += 3f;
					skillRangeArea.transform.localPosition = temp;

					RaycastHit hit2;
					if (Physics.Raycast(skillRangeArea.transform.position, skillRangeArea.transform.forward, out hit2, 1 << LayerMask.NameToLayer("Block")))
					{
						targetPos = hit2.point;
					}
				}
				else
					skillRangeArea.transform.position = temp;
			}
		}
	}
	public void OnSkill(int num)
	{
		if (num > skills.Count) return;
		Skill selectSkill = skills[num];    // 사용하고자 하는 스킬 가져오기
		if (!selectSkill.CanUse()) return;
		if (usingSkill == null) usingSkill = selectSkill; // 널이면 바로 넣기

		if (usingSkill == selectSkill)
		{
			if (usingSkill.property.ready)	// 조준 해야하는 스킬?
			{
				if (usingSkill.property.quickUse) // 바로 사용 ?
				{
					UseSkill(); // [조준스킬] [퀵 사용] 사용
				}
				else
				{
					if (isLookSkillTarget)	// 조준 하는 중?
					{
						UseSkill(); // [조준스킬] [조준 후 사용] 사용
					}
					else SkillAreaEnable();
				}
			}
			else // 즉발 스킬
			{
				UseSkill(); // [즉발스킬] 사용
			}
		}
		else
		{
			usingSkill = selectSkill;
			SkillAreaDisable();
			OnSkill(num);
		}
	}
	public void OnLB()
	{
		if (isLookSkillTarget)  // 조준 하는 중?
		{
			UseSkill(); // [조준스킬] [조준 후 사용] 사용
		}
	}
	public void OnRB()
	{
		SkillAreaDisable();
	}
	public void SetSkillUI()
	{
		UIManager.instance.SetSkillImage(skills);
	}
	void SkillAreaEnable()
	{
		float distance = usingSkill.area.distance * 2f;
		float range = usingSkill.area.range;
		isLookSkillTarget = true;
		skillDistanceArea.size = new Vector3(distance, distance, 100f);
		skillRangeArea.size = new Vector3(range, range, 100f);
		skillDistanceArea.enabled = true;
		skillRangeArea.enabled = true;
		UIManager.instance.SelectSkill(usingSkill);
	}
	void SkillAreaDisable()
	{
		usingSkill = null;
		isLookSkillTarget = false;
		skillDistanceArea.enabled = false;
		skillRangeArea.enabled = false;
		UIManager.instance.UnSelectSkill();
	}
	public float GetAP() 
	{
		return 10f;
	}
	void UseSkill()
	{
		usingSkill.Use(targetPos); // [조준스킬] [조준 후] 사용
		usingSkill = null;
		SkillAreaDisable();
	}
}
