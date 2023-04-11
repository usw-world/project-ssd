using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager UIM;
    [SerializeField] private RectTransform cutSceneBannerTop;           // ��� ���
    [SerializeField] private RectTransform cutSceneBannerBottom;        // �ϴ� ���
    [SerializeField] private float CinemaMoveSpeed = 130f;      // ��ʰ� �����̴� �ӵ�
    private UIManager() { }
    private void Awake() => UIM = this;
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
