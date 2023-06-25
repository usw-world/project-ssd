
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPlayerUI : MonoBehaviour {
    [SerializeField] private Image fadeInOutImg;
	[SerializeField] private Slider sliderHP;
	[SerializeField] private Slider sliderSP;
	[SerializeField] private Slider sliderSlide;
	[SerializeField] private Slider sliderCharging;
	[SerializeField] private List<Image> skillFills;
	[SerializeField] private GameObject skillTime;
	public Image imgCharging;
	public Image imgChargingBackground;
	public Image imgCutSceneSwordTrail;
	private Color red = new Color(1f, 0f, 0f);
	private Color orange = new Color(1f, 0.65f, 0f);
	private Color yellow = new Color(1f, 1f, 0f);
	private Color darkYellow = new Color(0.8f, 0.8f, 0f);
	private TPlayerSkill[] skills = new TPlayerSkill[5];
	private void Awake()
	{
		fadeInOutImg.gameObject.SetActive(false);
		sliderCharging.gameObject.SetActive(false);
		imgCutSceneSwordTrail.gameObject.SetActive(false);
	}
	public void Initialize(){
		sliderHP.maxValue = TPlayer.instance.status.maxHp;
		sliderSP.maxValue = TPlayer.instance.status.maxSp;
		sliderHP.value = TPlayer.instance.status.hp;
		sliderSP.value = TPlayer.instance.status.sp;
		skills[0] = TPlayer.instance.skill.combo_1[0] as TPlayerSkill;
		skills[1] = TPlayer.instance.skill.charging_DrawSwordAttack_7time[0] as TPlayerSkill;
		skills[2] = TPlayer.instance.skill.charging as TPlayerSkill;
		skills[3] = TPlayer.instance.skill.powerUp as TPlayerSkill;
		skills[4] = TPlayer.instance.skill.dodge as TPlayerSkill;
		skillFills[3].transform.parent.gameObject.SetActive(TPlayer.instance.options[17].active);
	}
	private void Update()
	{
		if (TPlayer.instance == null) return;
		for (int i = 0; i < skills.Length; i++)
		{
			if (skills[i].usingSP < TPlayer.instance.status.sp)
			{
				skillFills[i].fillAmount = 1f - skills[i].property.nowCoolTime / skills[i].property.coolTime;
			}
			else
			{
				skillFills[i].fillAmount = 1f;
			}
		}
		sliderSlide.value = TPlayer.instance.GetAllShieldAmount();
	}
	public void StartCutScene() {
		sliderHP.gameObject.SetActive(false);
		sliderSP.gameObject.SetActive(false);
		sliderSlide.gameObject.SetActive(false);
		skillTime.SetActive(false);
	}
	public void EndCutScene()
	{
		sliderHP.gameObject.SetActive(true);
		sliderSP.gameObject.SetActive(true);
		sliderSlide.gameObject.SetActive(true);
		skillTime.SetActive(true);
	}
	public void SetChargingLevel(int level, float maxValue)
	{
		sliderCharging.maxValue = maxValue;
		switch (level)
		{
			case 0:
				imgCharging.color = yellow;
				imgChargingBackground.color = darkYellow;
				break;
			case 1:
				imgCharging.color = orange;
				imgChargingBackground.color = yellow;
				break;
			case 2:
				imgCharging.color = red;
				imgChargingBackground.color = orange;
				break;
			case 3:
				imgCharging.color = red;
				imgChargingBackground.color = red;
				break;
		}
	}
	public void SetActiveCharging(bool active){
		sliderCharging.gameObject.SetActive(active);
	}
	public void SetChargingValue(float value)
	{
		sliderCharging.value = value;
	}
	public void RefreshHp(float value) {
		sliderHP.value = value;
	}
	public void ReFreshStamina(float value)
	{
		sliderSP.value = value;
	}
	public IEnumerator FadeIn(float inTime)
	{
		fadeInOutImg.gameObject.SetActive(true);
		float increasingValue = 1f / inTime;
		float value = 0;
		float time = 0;
		while (time < inTime)
		{
			time += Time.deltaTime;
			value += increasingValue * Time.deltaTime;
			fadeInOutImg.color = new Color(1f, 1f, 1f, value);
			yield return null;
		}
		fadeInOutImg.gameObject.SetActive(false);
	}
	public IEnumerator FadeOut(float outTime)
	{
		fadeInOutImg.gameObject.SetActive(true);
		float decreasingValue = 1f / outTime;
		float value = 1;
		float time = 0;
		while (time < outTime)
		{
			time += Time.deltaTime;
			value -= decreasingValue * Time.deltaTime;
			fadeInOutImg.color = new Color(1f, 1f, 1f, value);
			yield return null;
		}
		fadeInOutImg.gameObject.SetActive(false);
	}
	public IEnumerator FaidInAndMove()
	{
		float numTime = 0;
		Vector3 targetPoint = Vector3.forward;
		imgCutSceneSwordTrail.gameObject.SetActive(true);
		imgCutSceneSwordTrail.rectTransform.localScale = new Vector3(0.5f, 0.5f);
		imgCutSceneSwordTrail.color = new Color(1f, 1f, 1f, 1f);
		while (imgCutSceneSwordTrail.rectTransform.localScale.y < 21f)
		{
			numTime += Time.deltaTime * 2f;
			float s = 1f - Mathf.Cos(numTime);
			imgCutSceneSwordTrail.rectTransform.localScale += new Vector3(s, s * s);
			Vector3 d = Vector3.Lerp(Vector3.zero, targetPoint, Mathf.Sin(Mathf.PI * .5f + numTime * .5f * Mathf.PI));
			d.y = 0;
			TPlayer.instance.movement.MoveToward(d * Time.deltaTime * 80f);
			yield return null;
		}
	}
	public IEnumerator FaidOut()
	{
		float colorA = 1f;
		while (imgCutSceneSwordTrail.color.a != 0)
		{
			colorA = (colorA > 0) ? colorA - Time.deltaTime : 0;
			imgCutSceneSwordTrail.color = new Color(1f, 1f, 1f, colorA);
			yield return null;
		}
		imgCutSceneSwordTrail.gameObject.SetActive(false);
	}
}