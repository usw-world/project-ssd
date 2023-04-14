using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager UIM;
    [SerializeField] private RectTransform cutSceneBannerTop;           // 상단 배너
    [SerializeField] private RectTransform cutSceneBannerBottom;        // 하단 배너
    [SerializeField] private float CinemaMoveSpeed = 130f;      // 배너가 움직이는 속도

	[SerializeField] private List<Image> skillImage;
	[SerializeField] private List<Image> skillCoolTimeFill;
	[SerializeField] private List<Image> skillSelect;
	List<Skill> playerSkills;

	private UIManager() { }
    private void Awake() => UIM = this;
	private void Update()
	{
		if (playerSkills != null)
		{
			for (int i = 0; i < playerSkills.Count; i++)
			{
				if (playerSkills[i] != null)
				{
					skillCoolTimeFill[i].fillAmount = 1 - playerSkills[i].property.nowCollTime / playerSkills[i].property.collTime;
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
    }
}
