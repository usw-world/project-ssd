using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QPlayerSkillUnityBall))]
public class QPlayerSkillUnityBallEditor : Editor
{
	bool fixOrigin = true;
	SkillOption option = SkillOption.option_1;

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();

		var data = (QPlayerSkillUnityBall)target;

		fixOrigin = EditorGUILayout.Toggle("기본 정보 수정", fixOrigin);
		EditorGUILayout.Space();

		if (fixOrigin)
		{
			EditorGUILayout.HelpBox("QPlayer의 스킬 Unity Ball을 설정합니다.", MessageType.None);

			data.info.name = EditorGUILayout.TextField("스킬 이름", data.info.name);
			data.info.skillText = EditorGUILayout.TextField("스킬 설명", data.info.skillText);

			data.info.effect = (GameObject)EditorGUILayout.ObjectField("투사체", data.info.effect, typeof(GameObject), true);
			data.info.skillImage = (Sprite)EditorGUILayout.ObjectField("이미지", data.info.skillImage, typeof(Sprite), true);
			data.area.distance = EditorGUILayout.FloatField("사정거리", data.area.distance);
			data.area.range = EditorGUILayout.FloatField("범위", data.area.range);
			data.property.coolTime = EditorGUILayout.FloatField("쿨타임", data.property.coolTime);
		}
		else
		{
			option = (SkillOption)EditorGUILayout.EnumPopup("옵션", option);
			EditorGUILayout.Space();

			switch (option)
			{
				case SkillOption.option_1:
					data.str1 = EditorGUILayout.TextField("1번째 특성 이름", data.str1);
					data.op1 = (OptionType)EditorGUILayout.EnumPopup("타입", data.op1);
					break;
				case SkillOption.option_2:
					data.str2 = EditorGUILayout.TextField("2번째 특성 이름", data.str2);
					data.op2 = (OptionType)EditorGUILayout.EnumPopup("타입", data.op2);
					break;
				case SkillOption.option_3:
					data.str3 = EditorGUILayout.TextField("3번째 특성 이름", data.str3);
					data.op3 = (OptionType)EditorGUILayout.EnumPopup("타입", data.op3);
					break;
				case SkillOption.option_4:
					data.str4 = EditorGUILayout.TextField("4번째 특성 이름", data.str4);
					data.op4 = (OptionType)EditorGUILayout.EnumPopup("타입", data.op4);
					break;
				case SkillOption.option_5:
					data.str5 = EditorGUILayout.TextField("5번째 특성 이름", data.str5);
					data.op5 = (OptionType)EditorGUILayout.EnumPopup("타입", data.op5);
					break;
				case SkillOption.option_6:
					data.str6 = EditorGUILayout.TextField("6번째 특성 이름", data.str6);
					data.op6 = (OptionType)EditorGUILayout.EnumPopup("타입", data.op6);
					break;
				case SkillOption.option_7:
					data.str7 = EditorGUILayout.TextField("7번째 특성 이름", data.str7);
					data.op7 = (OptionType)EditorGUILayout.EnumPopup("타입", data.op7);
					break;
				case SkillOption.option_8:
					data.str8 = EditorGUILayout.TextField("8번째 특성 이름", data.str8);
					data.op8 = (OptionType)EditorGUILayout.EnumPopup("타입", data.op8);
					break;
			}
		}

		


		/*
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
		*/
	}
}
public enum SkillOption {
	option_1,
	option_2,
	option_3,
	option_4,
	option_5,
	option_6,
	option_7,
	option_8
}
