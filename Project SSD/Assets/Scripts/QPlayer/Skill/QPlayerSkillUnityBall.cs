using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QPlayerSkillUnityBall : Skill
{
	public string str1;
	public string str2;
	public string str3;
	public string str4;
	public string str5;
	public string str6;
	public string str7;
	public string str8;
	public OptionType op1;
	public OptionType op2;
	public OptionType op3;
	public OptionType op4;
	public OptionType op5;
	public OptionType op6;
	public OptionType op7;
	public OptionType op8;
	private void Start()
	{
		info.name = "유니티볼";
	}
	public override bool CanUse()
	{
		return true;
	}
}