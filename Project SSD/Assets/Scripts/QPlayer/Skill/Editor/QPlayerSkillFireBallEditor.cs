using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SSDSkill;

[CustomEditor(typeof(QPlayerSkillUnityBall))]
[CanEditMultipleObjects]
public class QPlayerSkillUnityBallEditor : Editor
{
	QPlayerSkillUnityBall data;
	eSkillOption option;
	OptionType typeTemp;
	QPlayerActiveType activefTypeTemp;
	QPlayerBuffType buffTypeTemp;
	QPlayerDebuffType debuffTypeTemp;
	MessageType messageType;
	string temp_name;
	string warningText;
	float temp_1, temp_2;
	bool fixOption;
	bool text = false;

	private void OnEnable()
	{
		serializedObject.Update();
		data = target as QPlayerSkillUnityBall;
		for (int i = 0; i < data.optionsVal.Length; i++) 
			if (data.optionsVal[i] == null) 
				data.optionsVal[i] = new SkillOptionVal(); 
		for (int i = 0; i < data.options.Length; i++) 
			if (data.options[i] == null) 
				data.options[i] = new SkillOption();
		Reset(0);
		serializedObject.ApplyModifiedProperties();
	}
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI(); return;
		if (text)
		{
			EditorGUILayout.HelpBox(warningText, messageType);
			if (GUILayout.Button("확인")) text = false;
		}
		else
		{
			serializedObject.Update();
			EditorGUILayout.HelpBox("QPlayer의 스킬 Unity Ball을 설정합니다.", MessageType.None);

			data.info.name = EditorGUILayout.TextField("스킬 이름", data.info.name);
			data.info.skillText = EditorGUILayout.TextField("스킬 설명", data.info.skillText);
			data.info.effect = (GameObject)EditorGUILayout.ObjectField("투사체", data.info.effect, typeof(GameObject), true);
			data.info.skillImage = (Sprite)EditorGUILayout.ObjectField("이미지", data.info.skillImage, typeof(Sprite), true);
			data.area.distance = EditorGUILayout.FloatField("사정거리", data.area.distance);
			data.area.range = EditorGUILayout.FloatField("범위", data.area.range);
			data.property.coolTime = EditorGUILayout.FloatField("쿨타임", data.property.coolTime);

			EditorGUILayout.Space();
			fixOption = EditorGUILayout.Toggle("특성 설정", fixOption);
			EditorGUILayout.Space();

			if(fixOption)
			{
				EditorGUI.BeginChangeCheck();
				option = (eSkillOption)EditorGUILayout.EnumPopup("특성", option);
				int idx = (int)option;
				if (EditorGUI.EndChangeCheck()) Reset(idx); 
				EditorGUILayout.Space();

				temp_name = EditorGUILayout.TextField("특성 이름", temp_name);
				typeTemp = (OptionType)EditorGUILayout.EnumPopup("특성 타입", typeTemp);
				switch (typeTemp)
				{
					case OptionType.active:
						activefTypeTemp = (QPlayerActiveType)EditorGUILayout.EnumPopup("Active 타입", activefTypeTemp);
						switch (activefTypeTemp)
						{
							case QPlayerActiveType.big:
								temp_1 = EditorGUILayout.FloatField("크기 상승 량", temp_1);
								if (GUILayout.Button("저장")) {
									if (temp_1 <= 0)
										Error("'크기 상승 량'을 제대로 설정하십시오"); 
									else
										Apply(idx); 
								}
								if (GUILayout.Button("초기화")) Reset(idx); 
								break;
						}
						break;
					case OptionType.buff:
						buffTypeTemp = (QPlayerBuffType)EditorGUILayout.EnumPopup("버프 타입", buffTypeTemp);
						switch (buffTypeTemp)	{
							case QPlayerBuffType.healing:
								temp_1 = EditorGUILayout.FloatField("지속 시간", temp_1);
								temp_2 = EditorGUILayout.FloatField("회복 량", temp_2);
								if (GUILayout.Button("저장")) {
									if (temp_1 < 0)
										Error("'지속 시간'을 제대로 설정하십시오");
									else if (temp_2 <= 0)
										Error("'회복 량'을 제대로 설정하십시오");
									else
										Apply(idx);
								}
								if (GUILayout.Button("초기화")) Reset(idx);
								break;
							case QPlayerBuffType.shield:
								temp_1 = EditorGUILayout.FloatField("지속 시간", temp_1);
								temp_2 = EditorGUILayout.FloatField("쉴드 량", temp_2);
								if (GUILayout.Button("저장"))
								{
									if (temp_1 < 0)
										Error("'지속 시간'을 제대로 설정하십시오");
									else if (temp_2 <= 0)
										Error("'쉴드 량'을 제대로 설정하십시오");
									else
										Apply(idx);
								}
								if (GUILayout.Button("초기화")) Reset(idx); 
								break;
							case QPlayerBuffType.boost:
								temp_1 = EditorGUILayout.FloatField("지속 시간", temp_1);
								temp_2 = EditorGUILayout.FloatField("상승 량", temp_2);
								if (GUILayout.Button("저장"))
								{
									if (temp_1 < 0)
										Error("'지속 시간'을 제대로 설정하십시오");
									else if (temp_2 <= 0)
										Error("'상승 량'을 제대로 설정하십시오");
									else
										Apply(idx);
								}
								if (GUILayout.Button("초기화")) Reset(idx); 
								break;
						}
						break;
					case OptionType.debuff:
						debuffTypeTemp = (QPlayerDebuffType)EditorGUILayout.EnumPopup("디버프 타입", debuffTypeTemp);
						switch (debuffTypeTemp)
						{
							case QPlayerDebuffType.damage:
								break;
							case QPlayerDebuffType.slow:
								break;
							case QPlayerDebuffType.inability:
								break;
							default:
								break;
						}
						break;
				}
			}
		}

		//SerializedProperty temp = serializedObject.FindProperty("options");
		//EditorGUILayout.PropertyField(temp);
		//temp = serializedObject.FindProperty("optionsVal");
		//EditorGUILayout.PropertyField(temp);

		serializedObject.ApplyModifiedProperties();
	}
	void Reset(int idx) {
		temp_name = data.options[idx].name;
		typeTemp = data.options[idx].type;
		activefTypeTemp = data.options[idx].activefType;
		buffTypeTemp = data.options[idx].buffType;
		debuffTypeTemp = data.options[idx].debuffType;
		temp_1 = data.optionsVal[idx].val_1;
		temp_2 = data.optionsVal[idx].val_2;
	}
	void Apply(int idx) {
		if (temp_name == "") { 
			Error("'특성 이름'을 제데로 입력하시오.");
			return;
		}
		Error("저장 완료", MessageType.Info);
		data.options[idx].name = temp_name;			
		data.options[idx].type = typeTemp;
		data.options[idx].activefType = activefTypeTemp;
		data.options[idx].buffType = buffTypeTemp;	
		data.options[idx].debuffType = debuffTypeTemp;
		data.optionsVal[idx].val_1 = temp_1;		
		data.optionsVal[idx].val_2 = temp_2;		
	}
	void Error(string warningText, MessageType type = MessageType.Error) {
		this.warningText = warningText;
		text = true;
		messageType = type;
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
