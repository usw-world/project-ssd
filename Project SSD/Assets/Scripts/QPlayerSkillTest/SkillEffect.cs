using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class SkillEffect : MonoBehaviour
{
	protected SkillProperty property;
	abstract public void OnActive(SkillProperty property);
}
