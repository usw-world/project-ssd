using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

	[SerializeField] private List<Image> skillImage;
	[SerializeField] private List<Image> skillCoolTimeFill;
	[SerializeField] private List<Image> skillSelect;
	List<Skill> playerSkills;

	[SerializeField] private TPlayerUI tPlayerUI;
	[SerializeField] private QPlayerUI qPlayerUI;

	[SerializeField] private Canvas commonHudCanvas;
 	[SerializeField] private GameObject alertUIPrefab;
	[SerializeField] private EscapeMenu escapeMenu;

    private void Awake() {
		if(instance == null)
			instance = this;
		else
			Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);
	}
	private void Update() {
		if (playerSkills != null)
		{
			for (int i = 0; i < playerSkills.Count; i++)
			{
				if (playerSkills[i] != null)
				{
					skillCoolTimeFill[i].fillAmount = 1 - playerSkills[i].property.nowCoolTime / playerSkills[i].property.coolTime;
				}
			}
			
		}
	}
	public void SetSkillImage(List<Skill> skills)
	{
		playerSkills = skills;
		for (int i = 0; i < skillImage.Count; i++)
		{
			skillImage[i].sprite = skills[i].info.skillImage;
		}
	}
	public void SelectSkill(Skill skill)
	{
		UnSelectSkill();
		int idx = playerSkills.IndexOf(skill);
		skillSelect[idx].enabled = true;
	}
	public void UnSelectSkill()
	{
		for (int i = 0; i < skillSelect.Count; i++)
		{
			skillSelect[i].enabled = false;
		}
	}
	public void OnPressEscape() {
		escapeMenu.OnPressEscape();
	}
	public AlertUI AlertMessage(string message) {
		GameObject alertUIGobj = Instantiate(alertUIPrefab, commonHudCanvas.transform);
		AlertUI alertUI = alertUIGobj.GetComponent<AlertUI>();
		if(alertUI != null) {
			alertUI.SetMessage(message);
		} else {
			Debug.LogError("Message box for alert is loaded but it hasn't 'AlertUI' Component.");
		}
		return alertUI;
	}

	/*
	public void StartCutScene()
    {
        StartCoroutine(OnBanner());
    }
    public void EndCutScene()
    {
        StartCoroutine(OffBanner());
    }
    IEnumerator OnBanner()
    {
        while (cutSceneBannerTop.anchoredPosition.y >= -65f)
        {
            cutSceneBannerTop.anchoredPosition -= new Vector2(0, CinemaMoveSpeed * Time.deltaTime);
            cutSceneBannerBottom.anchoredPosition += new Vector2(0, CinemaMoveSpeed * Time.deltaTime);
            yield return null;
        }
        cutSceneBannerTop.anchoredPosition = new Vector2(0, -65f);
        cutSceneBannerBottom.anchoredPosition = new Vector2(0, 65f);
    }
    IEnumerator OffBanner()
    {
        while (cutSceneBannerTop.anchoredPosition.y <= 65f)
        {
            cutSceneBannerTop.anchoredPosition += new Vector2(0, CinemaMoveSpeed * Time.deltaTime);
            cutSceneBannerBottom.anchoredPosition -= new Vector2(0, CinemaMoveSpeed * Time.deltaTime);
            yield return null;
        }
        cutSceneBannerTop.anchoredPosition = new Vector2(0, 65f);
        cutSceneBannerBottom.anchoredPosition = new Vector2(0, -65f);
    }*/
}
