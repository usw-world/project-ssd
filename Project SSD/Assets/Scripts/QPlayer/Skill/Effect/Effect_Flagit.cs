using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Flagit : MonoBehaviour, IPoolableObject
{
	public static List<Effect_Flagit> inSceneObj = new List<Effect_Flagit>();
	[SerializeField] private GameObject dotDamagePrefab;
	public Transform lightningMuzzle;
	[SerializeField] private GameObject lightningEffect;
	[SerializeField] private GameObject chargingEffect;

	[SerializeField] private bool previewDamageZone = false;
	[SerializeField] private bool previewDotDamageZone = false;
	private GameObject previewSpherePrefab;
	private Animator animator;
	private Attachment shield;
	private string animationTrigger;
	private string dotDamageEffectKey;
	private float damageAmount;
	private float dotDamageAmount;
	private bool isDotDamage = false;
	private bool isShield = false;
	private bool isFlinching4s = false;
	private bool isAreaTwice = false;
	private bool isBig = false;

	private void Awake()
	{
		previewSpherePrefab = Resources.Load("previewSpherePrefab") as GameObject;
		animator = GetComponent<Animator>();
	}
	private void Start()
	{
		dotDamageEffectKey = dotDamagePrefab.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(dotDamageEffectKey, dotDamagePrefab, false);
	}
	public void Run() // 이펙트 시작할때 실행
	{
		animator.SetTrigger(animationTrigger);
		if (isBig) transform.localScale = Vector3.one * 3f;
		else transform.localScale = Vector3.one;
		lightningEffect.SetActive(false);
		chargingEffect.SetActive(true);
	}
	public void ActiveDamageZone() // 데미지 시작할때 함수
	{
		if (isBig) CameraManager.instance.MakeNoise(2f, 0.3f);
		lightningEffect.SetActive(true);
		chargingEffect.SetActive(false);
		Collider[] hit = null;
		float size = 1f;
		float flinching = 1f;
		if (isAreaTwice) size = 2f;
		if (isBig) size += 1f;
		if (isFlinching4s) flinching = 4f;
		hit = Physics.OverlapSphere(transform.position, size, 1 << 8 );
		for (int i = 0; i < hit.Length; i++)
		{
			IDamageable target = hit[i].GetComponent<IDamageable>();
			Damage damage = new Damage(
				GetLastDamage(),
				flinching,
				(hit[i].transform.position - transform.position).normalized * 5f,
				Damage.DamageType.Normal
			);
			target.OnDamage(damage);
		}
		if (isShield) RunShield();
		if (isDotDamage) StartCoroutine(RunDotDamage());
		StartCoroutine(InPool());

		if (previewDamageZone)
		{
			GameObject previewSphere = Instantiate(previewSpherePrefab, transform.position, transform.rotation);
			previewSphere.transform.localScale = Vector3.one * size * 1.5f;
			print(previewSphere.transform.localScale);
			Destroy(previewSphere, 2f);
		}
	}
	public void Initialize(float damageAmount)
	{
		this.damageAmount = damageAmount;
		isDotDamage = false;
		isShield = false;
		isFlinching4s = false;
		isAreaTwice = false;
		isBig = false;
		inSceneObj.Add(this);
	}
	private float GetLastDamage()
	{
		float amount = damageAmount;
		if (isBig) amount *= 3f;
		return amount;
	}
	IEnumerator InPool()
	{
		yield return new WaitForSeconds(5.2f);
		PoolerManager.instance.InPool(GetKey(), gameObject);
		inSceneObj.Remove(this);
	}

	#region options[0] 지속데미지
	IEnumerator RunDotDamage()
	{
		Collider[] hit = null;
		
		for (int i = 0; i < 5; i++)
		{
			GameObject dotDamageObj = PoolerManager.instance.OutPool(dotDamageEffectKey);
			dotDamageObj.transform.position = transform.position;
			dotDamageObj.transform.position += new Vector3(0, 0.25f, 0);
			StartCoroutine(InPoolEffect(dotDamageObj, dotDamageEffectKey, 1.1f));
			float size = 2f;
			if (isAreaTwice)
			{
				size = 4f;
				dotDamageObj.transform.localScale = new Vector3(2f, 0.2f, 2f);
			}
			else
			{
				dotDamageObj.transform.localScale = new Vector3(1f, 0.2f, 1f);
			}
			if (isBig) {
				size += 2f;
				dotDamageObj.transform.localScale += new Vector3(1f, 0, 1f);
			}
			hit = Physics.OverlapSphere(transform.position, size, 1 << 8);
			for (int j = 0; j < hit.Length; j++)
			{
				IDamageable target = hit[j].GetComponent<IDamageable>();
				if (target != null)
				{
					Damage damage = new Damage(
						dotDamageAmount, // 데미지
						0f,				 // 경직시간
						Vector3.zero,	 // 경직방향 vector
						Damage.DamageType.Normal
					);
					target.OnDamage(damage);
				}
			}
			if (previewDotDamageZone)
			{
				GameObject previewSphere = Instantiate(previewSpherePrefab, transform.position, transform.rotation);
				previewSphere.transform.localScale = Vector3.one * size * 1.5f;
				print(previewSphere.transform.localScale);
				Destroy(previewSphere, 0.5f);
			}
			yield return new WaitForSeconds(1f);
		}
	}
	IEnumerator InPoolEffect(GameObject obj, string key, float time)
	{
		yield return new WaitForSeconds(time);
		PoolerManager.instance.InPool(key, obj);
	}
	public void ActiveDotDamage(float dotDamageAmount)
	{
		isDotDamage = true;
		this.dotDamageAmount = dotDamageAmount;
	}
	#endregion options[0] 지속데미지

	#region options[1] 쉴드 주기
	public void ActiveShield(Attachment shield)
	{
		isShield = true;
		this.shield = shield;
	}
	private void RunShield()
	{
		Collider[] hit = null;
		float size = 1f;
		if (isAreaTwice) size = 2f;
		if (isBig) size += 1f;
		hit = Physics.OverlapSphere(transform.position, size, 1 << 7);
		if (hit.Length != 0)
		{
			for (int i = 0; i < hit.Length; i++)
			{
				TPlayer player = hit[i].GetComponent<TPlayer>();
				if (player != null)
				{
					player.AddAttachment(shield);
				}
			}
		}
	}
	#endregion options[1] 쉴드 주기

	#region  options[3] 경직 시간 4초
	public void ActiveFlinching4s()
	{
		isFlinching4s = true;
	}
	#endregion  options[3] 경직 시간 4초

	#region options[4] 범위 2배
	public void ActiveAreaTwice()
	{
		isAreaTwice = true;
	}
	#endregion options[4] 범위 2배

	#region options[5] 낙하속도
	public void SetSpeedNormal()
	{
		animationTrigger = "Normal";
	}
	public void SetSpeedFast()
	{
		animationTrigger = "Fast";
	}
	#endregion options[5]

	#region  options[6] 강한 1개
	public void ActiveBig()
	{
		isBig = true;
	}
	#endregion  options[6] 강한 1개
	public string GetKey()
	{
		return GetType().ToString();
	}
}
