using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Drawing.Printing;

[CustomEditor(typeof(QPlayerSkillAoe))]
[CanEditMultipleObjects]
public class QPlayerSkillAoeEditor : Editor
{
    QPlayerSkillAoe data;
	eSkillOption option;
	bool fixOption;

	private void OnEnable()
	{
		data = target as QPlayerSkillAoe;

		for (int i = 0; i < data.options.Length; i++)
		{
            if (data.options[i] == null)
                data.options[i] = new SkillOptionInformation();
        }
			
	}
	public override void OnInspectorGUI()
	{
		// base.OnInspectorGUI(); return;
		//serializedObject.Update();
		EditorGUILayout.HelpBox("QPlayer의 스킬 Gather Enemies을 설정합니다.", MessageType.None);

		EditorGUILayout.FloatField("테스트", data.options.Length);
		data.info.name = EditorGUILayout.TextField("스킬 이름", data.info.name);
		data.info.skillText = EditorGUILayout.TextField("스킬 설명", data.info.skillText);
		data.info.effect = (GameObject)EditorGUILayout.ObjectField("장판 오브젝트", data.info.effect, typeof(GameObject), true);
		data.cosmic = (GameObject)EditorGUILayout.ObjectField("코스믹 오브젝트", data.cosmic, typeof(GameObject), true);
		data.info.skillImage = (Sprite)EditorGUILayout.ObjectField("이미지", data.info.skillImage, typeof(Sprite), true);
		data.area.distance = EditorGUILayout.FloatField("사정거리", data.area.distance);
        data.info.effect.GetComponent<SphereCollider>().radius = EditorGUILayout.FloatField("공격 범위", data.info.effect.GetComponent<SphereCollider>().radius);
		data.cosmic.GetComponent<CapsuleCollider>().radius = EditorGUILayout.FloatField("당기는 범위", data.cosmic.GetComponent<CapsuleCollider>().radius);
        data.property.coolTime = EditorGUILayout.FloatField("쿨타임", data.property.coolTime);
		data.property.skillAP = EditorGUILayout.FloatField("스킬 피해량(%)", data.property.skillAP);
		data.aimType = (AimType)EditorGUILayout.EnumPopup("조준 타입", data.aimType);
		data.property.ready = EditorGUILayout.Toggle("범위 스킬 활성화", data.property.ready);

		EditorGUILayout.Space();
		fixOption = EditorGUILayout.Toggle("특성 설정", fixOption);
		EditorGUILayout.Space();

		data.options[0].active = EditorGUILayout.Toggle("특성 1 활성화", data.options[0].active);
		data.options[1].active = EditorGUILayout.Toggle("특성 2 활성화", data.options[1].active);
		data.options[2].active = EditorGUILayout.Toggle("특성 3 활성화", data.options[2].active);
		data.options[3].active = EditorGUILayout.Toggle("특성 4 활성화", data.options[3].active);


        if (fixOption)
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
					data.option00_increaseCastingSpeed = EditorGUILayout.FloatField("데미지 상승량(%)", data.option00_increaseCastingSpeed);
					if (data.option00_increaseCastingSpeed <= 0) Error("'캐스팅 속도 증가량(%)'를 설정하십시오");
					break;
				case eSkillOption.option_2:
					data.option01_decreaseSkillCoolDown = EditorGUILayout.FloatField("속도 상승량(%)", data.option01_decreaseSkillCoolDown);
					if (data.option01_decreaseSkillCoolDown <= 0) Error("'스킬 쿨타임 감소량(%)'을 설정하십시오");
					break;
				case eSkillOption.option_3:

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
