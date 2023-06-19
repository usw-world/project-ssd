using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QPlayerUI : MonoBehaviour
{
	[SerializeField] private Slider spSlider;
	public void Initialize(PlayerStatus status)
	{
		spSlider.maxValue = status.maxHp;
		spSlider.value = status.sp;
	}
	public void ReFreshStamina(float value)
	{
		spSlider.value = value;
	}
}  