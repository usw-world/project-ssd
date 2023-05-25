using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Flagit : MonoBehaviour, IPoolableObject
{
	[SerializeField] private GameObject dotDamageObj;

	private Animator animator;
	private Attachment shield;
	private string animationTrigger;

	private float damageAmount;
	private float dotDamageAmount;

	private bool activeDamageZone = false;
	private bool isDotDamage = false;
	private bool isShield = false;
	private bool isFlinching4s = false;
	private bool isAreaTwice = false;
	private bool isBig = false;
	private void Awake()
	{
		if (isBig) transform.localScale = Vector3.zero * 2f;
		animator = GetComponent<Animator>();
	}
	public void Run() // 이펙트 시작할때 실행
	{
		animator.SetTrigger(animationTrigger);
	}
	public void ActiveDamageZone() // 데미지 시작할때 함수
	{
		activeDamageZone = true;
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
	}
	public void Initialize(float damageAmount)
	{
		this.damageAmount = damageAmount;
		activeDamageZone = false;
		isDotDamage = false;
		isShield = false;
		isFlinching4s = false;
		isAreaTwice = false;
		isBig = false;
}
	private float GetLastDamage()
	{
		float amount = damageAmount;
		if (isBig) amount *= 3f;
		return amount;
	}

	#region options[0] 지속데미지
	IEnumerator RunDotDamage()
	{
		dotDamageObj.SetActive(true);
		Collider[] hit = null;
		float size = 2f;
		if (isBig) size += 1f;
		if (isAreaTwice) {
			size = 4f;
			dotDamageObj.transform.localScale = Vector3.zero * 2f;
		}else{
			dotDamageObj.transform.localScale = Vector3.zero;
		}
		for (int i = 0; i < 5; i++)
		{
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
			yield return new WaitForSeconds(1f);
		}
		dotDamageObj.SetActive(false);
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
