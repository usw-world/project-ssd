using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QPlyerSkill))]
public class QPlayerScriptEdit : Editor
{
	bool typeBuff = false;
	public override void OnInspectorGUI()
	{
		var data = (QPlyerSkill)target;

		data.type = (QPlayerSkillType)EditorGUILayout.EnumPopup("스킬타입", data.type);

		EditorGUILayout.Space();

		switch (data.type)
		{
			case QPlayerSkillType.projectile:
				typeBuff = false;
				data.speed = EditorGUILayout.FloatField("투사체 스피드" , data.speed);
				EditorGUILayout.Space();
				data.isMultiple = EditorGUILayout.Toggle("여러번 발사", data.isMultiple);
				if (data.isMultiple)
				{
					data.multipleCount = EditorGUILayout.IntField("발사 횟수" , data.multipleCount);
					data.multipleInterval = EditorGUILayout.FloatField("발사 지연", data.multipleInterval);
				}
				EditorGUILayout.Space();
				data.isPenetration = EditorGUILayout.Toggle("관통", data.isPenetration);
				if (data.isPenetration)
				{
					data.penetrationCount = EditorGUILayout.IntField("관통 횟수", data.penetrationCount);
				}
				EditorGUILayout.Space();
				data.isDebuff = EditorGUILayout.Toggle("디버프", data.isDebuff);
				if (data.isDebuff)
				{
					data.debuffType = (QPlayerDebuffType)EditorGUILayout.EnumPopup("디버프 타입", data.debuffType);
					data.debuffTime = EditorGUILayout.FloatField("지속 시간", data.debuffTime);
					data.debuffAmount = EditorGUILayout.FloatField("디버프 량", data.debuffAmount);
				}
				EditorGUILayout.Space();
				break;
			case QPlayerSkillType.area:
				typeBuff = false;
				data.isContinuation = EditorGUILayout.Toggle("지속", data.isContinuation);
				if (data.isContinuation)
				{
					data.continuationTime = EditorGUILayout.FloatField("지속 시간", data.continuationTime);
				}
				EditorGUILayout.Space();
				data.isDebuff = EditorGUILayout.Toggle("디버프", data.isDebuff);
				if (data.isDebuff)
				{
					data.debuffType = (QPlayerDebuffType)EditorGUILayout.EnumPopup("디버프 타입", data.debuffType);
					data.debuffTime = EditorGUILayout.FloatField("지속 시간", data.debuffTime);
					data.debuffAmount = EditorGUILayout.FloatField("디버프 량", data.debuffAmount);
				}
				EditorGUILayout.Space();
				break;
			case QPlayerSkillType.buff:
				typeBuff = true;
				break;
		}

		data.isChain = EditorGUILayout.Toggle("체인 스킬", data.isChain);
		if (data.isChain)
		{
			data.chainCount = EditorGUILayout.IntField("횟수", data.chainCount);
			data.chainInterval = EditorGUILayout.FloatField("체인 스킬 시간", data.chainInterval);
		}
		EditorGUILayout.Space();
		data.isCasting = EditorGUILayout.Toggle("케스팅", data.isCasting);
		if (data.isCasting)
		{
			data.castingEffect = (GameObject)EditorGUILayout.ObjectField("캐스팅 이펙트", data.castingEffect, typeof(GameObject), true);
			data.castingTime = EditorGUILayout.FloatField("캐스팅 시간", data.castingTime);
		}
		EditorGUILayout.Space();
		if (typeBuff)
		{
			data.buffType = (QPlayerBuffType)EditorGUILayout.EnumPopup("버프 타입", data.buffType);
			data.buffTime = EditorGUILayout.FloatField("지속 시간", data.buffTime);
			data.buffAmount = EditorGUILayout.FloatField("디버프 량", data.buffAmount);
		}
		else
		{
			data.isBuff = EditorGUILayout.Toggle("버프", data.isBuff);
			if (data.isBuff)
			{
				data.buffType = (QPlayerBuffType)EditorGUILayout.EnumPopup("버프 타입", data.buffType);
				data.buffTime = EditorGUILayout.FloatField("지속 시간", data.buffTime);
				data.buffAmount = EditorGUILayout.FloatField("디버프 량", data.buffAmount);
			}
		}
	}
}
