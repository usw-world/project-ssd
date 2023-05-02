using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QPlayerSkillUnityBall))]
[CanEditMultipleObjects]
public class QPlayerSkillUnityBallEditor : Editor
{
	QPlayerSkillUnityBall data;
	eSkillOption option;
	bool fixOption;

	private void OnEnable()
	{
		data = target as QPlayerSkillUnityBall;

		for (int i = 0; i < data.options.Length; i++)
		{
			if (data.options[i] == null)
			{
				data.options[i] = new SkillOptionInformation();
			}
		}
	}
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI(); return;
		//serializedObject.Update();
		EditorGUILayout.HelpBox("QPlayer의 스킬 Unity Ball을 설정합니다.", MessageType.None);

		data.info.name = EditorGUILayout.TextField("스킬 이름", data.info.name);
		data.info.skillText = EditorGUILayout.TextField("스킬 설명", data.info.skillText);
		data.info.effect = (GameObject)EditorGUILayout.ObjectField("투사체", data.info.effect, typeof(GameObject), true);
		data.info.skillImage = (Sprite)EditorGUILayout.ObjectField("이미지", data.info.skillImage, typeof(Sprite), true);
		data.area.distance = EditorGUILayout.FloatField("사정거리", data.area.distance);
		//data.area.range = EditorGUILayout.FloatField("범위", data.area.range);
		data.property.coolTime = EditorGUILayout.FloatField("쿨타임", data.property.coolTime);
		data.property.skillAP = EditorGUILayout.FloatField("스킬 피해량(%)", data.property.skillAP);
		data.speed = EditorGUILayout.FloatField("투사체 속도", data.speed);
		data.aimType = (AimType)EditorGUILayout.EnumPopup("조준 타입", data.aimType);

		EditorGUILayout.Space();
		fixOption = EditorGUILayout.Toggle("특성 설정", fixOption);
		EditorGUILayout.Space();

		if(fixOption)
		{
			option = (eSkillOption)EditorGUILayout.EnumPopup("특성", option);
			int idx = (int)option;

			data.options[idx].name = EditorGUILayout.TextField("이름", data.options[idx].name);
			data.options[idx].info = EditorGUILayout.TextField("설명", data.options[idx].info);
			data.options[idx].image = (Sprite)EditorGUILayout.ObjectField("이미지", data.options[idx].image, typeof(Sprite), true);
			if (data.options[idx].name == "") Error("'이름'을 설정하십시오");
			if (data.options[idx].info == "") Error("'정보'을 설정하십시오");
			if (data.options[idx].image == null) Error("'이미지'을 설정하십시오");

			switch (option)
			{
				case eSkillOption.option_1:
					data.option01_increasingSkillPower = EditorGUILayout.FloatField("데미지 상승량(%)", data.option01_increasingSkillPower);
					if (data.option01_increasingSkillPower <= 0) Error("'데미지 상승량(%)'을 설정하십시오");
					break;
				case eSkillOption.option_2:
					data.option02_increasingSpeed = EditorGUILayout.FloatField("속도 상승량(%)", data.option02_increasingSpeed);
					if (data.option02_increasingSpeed <= 0) Error("'속도 상승량(%)'을 설정하십시오");
					break;
				case eSkillOption.option_3:
					data.option03_buffTime = EditorGUILayout.FloatField("버프 유지 시간", data.option03_buffTime);
					data.option03_healingAmount = EditorGUILayout.FloatField("총 회복량(%)", data.option03_healingAmount);
					if (data.option03_buffTime <= 0) Error("'버프 유지 시간'을 설정하십시오");
					if (data.option03_healingAmount <= 0) Error("'총 회복량(%)'을 설정하십시오");
					break;
				case eSkillOption.option_4:
					data.option04_debuffTime = EditorGUILayout.FloatField("디버프 유지시간", data.option04_debuffTime);
					data.option04_damageAmount = EditorGUILayout.FloatField("총 데미지 량(%)", data.option04_damageAmount);
					if (data.option04_debuffTime <= 0) Error("'디버프 유지시간'을 설정하십시오");
					if (data.option04_damageAmount <= 0) Error("'총 데미지 량(%)'을 설정하십시오");
					break;
				case eSkillOption.option_5:
					break;
				case eSkillOption.option_6:
					break;
				case eSkillOption.option_7:
					break;
				case eSkillOption.option_8:
					break;
			}
			 //data.options[idx].amount = EditorGUILayout.FloatField("크기 상승량", data.options[idx].amount);
			 //if (data.options[idx].amount <= 0) Error("'크기 상승량'을 설정하십시오");
		}

		if (GUI.changed) EditorUtility.SetDirty(target);
	}
	void Error(string warningText, MessageType type = MessageType.Error) {
		EditorGUILayout.HelpBox(warningText,type);
	}
}
public enum eSkillOption {
	option_1,
	option_2,
	option_3,
	option_4,
	option_5,
	option_6,
	option_7,
	option_8
}
