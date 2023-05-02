using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SSDSkill;


public class QPlayerSkillUnityBall : Skill
{
	public SkillOption[] options = new SkillOption[8];
	public SkillOptionVal[] optionsVal = new SkillOptionVal[8];

	private void Start()
	{
		for (int i = 0; i < 8; i++)
		{
			switch (options[i].type)
			{
				case OptionType.active:
					switch (options[i].activefType)
					{
						case QPlayerActiveType.big:
							options[i] = new SkillOptionBig(options[i], optionsVal[i].val_1);
							break;
					}
					break;
				case OptionType.buff:
					switch (options[i].buffType)
					{
						case QPlayerBuffType.healing:
							options[i] = new SkillOptionHealing(options[i], optionsVal[i].val_1, optionsVal[i].val_2);
							break;
						case QPlayerBuffType.shield:
							options[i] = new SkillOptionShield(options[i], optionsVal[i].val_1, optionsVal[i].val_2);
							break;
						case QPlayerBuffType.boost:
							options[i] = new SkillOptionAPBoost(options[i], optionsVal[i].val_1, optionsVal[i].val_2);
							break;
					}
					break;
				case OptionType.debuff:
					switch (options[i].debuffType)
					{
						case QPlayerDebuffType.damage:
							break;
						case QPlayerDebuffType.slow:
							break;
						case QPlayerDebuffType.inability:
							break;
					}
					break;
			}
		}
		for (int i = 0; i < options.Length; i++)
		{
			options[i]?.GetOnActive()?.Invoke();
		}
	}
	public override void Use()
	{
		
	}
	public override bool CanUse()
	{
		return true;
	}
}

namespace SSDSkill
{
	[Serializable]public class SkillOptionVal {
		public float val_1, val_2;
		public SkillOptionVal() {
			val_1 = -1f;
			val_2 = -1f;
		}
	}
	[Serializable]public class SkillOption
	{
		public string name;
		public OptionType type;
		public QPlayerActiveType activefType;
		public QPlayerBuffType buffType;
		public QPlayerDebuffType debuffType;
		public Action onActive;
		virtual public Action GetOnActive() { return null; }
	}
	[Serializable]public class SkillOptionBig : SkillOption {
		float amount;
		public SkillOptionBig(SkillOption origin, float amount) {
			name = origin.name;
			type = origin.type;
			activefType = origin.activefType;
			buffType = origin.buffType;
			debuffType = origin.debuffType;
			this.amount = amount;
		}
		public override Action GetOnActive(){
			onActive = () => {
				Debug.Log("특성 : " + name + " - 효과 : 크기를 " + amount * 100f + "% 크개함");
			};
			return onActive;
		}
	}
	[Serializable]public class SkillOptionAPBoost : SkillOption{
		float time, amount;
		public SkillOptionAPBoost(SkillOption origin, float time, float amount) {
			name = origin.name;
			type = origin.type;
			activefType = origin.activefType;
			buffType = origin.buffType;
			debuffType = origin.debuffType;
			this.time = time;
			this.amount = amount;
		}
		public override Action GetOnActive()
		{
			onActive = () => {
				Debug.Log("특성 : " + name + " - 효과 : " + time + "초 동안 공격력 " + amount * 100f + "% 상승");
			};
			return onActive;
		}
	}
	[Serializable]public class SkillOptionShield : SkillOption{
		float time, amount;
		public SkillOptionShield(SkillOption origin, float time, float amount)	{
			name = origin.name;
			type = origin.type;
			activefType = origin.activefType;
			buffType = origin.buffType;
			debuffType = origin.debuffType;
			this.time = time;
			this.amount = amount;
		}
		public override Action GetOnActive()
		{
			onActive = () => {
				Debug.Log("특성 : " + name + " - 효과 : " + time + "초 동안 쉴드 " + amount * 100f + "% 생김");
			};
			return onActive;
		}
	}
	[Serializable]public class SkillOptionHealing : SkillOption{
		float time, amount;
		public SkillOptionHealing(SkillOption origin, float time, float amount)	{
			name = origin.name;
			type = origin.type;
			activefType = origin.activefType;
			buffType = origin.buffType;
			debuffType = origin.debuffType;
			this.time = time;
			this.amount = amount;
		}
		public override Action GetOnActive()
		{
			onActive = () => {
				Debug.Log("특성 : " + name + " - 효과 : " + time + "초 동안 " + amount * 100f + "% 회복");
			};
			return onActive;
		}
	}
}