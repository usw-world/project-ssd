using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerFiySwrodTrail : MonoBehaviour
{
    private float damageAmount;
    private List<GameObject> target = new List<GameObject>();

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 15f);
    }
    public void SetDamage(float damageAmount) {
        this.damageAmount = damageAmount;
        target.Clear();
    }
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8 && target.Contains(other.gameObject) == false)
		{
            target.Add(other.gameObject);
            Damage damage = new Damage(
                damageAmount,
                1f,
                Vector3.zero,
                Damage.DamageType.Down
            );
            other.GetComponent<IDamageable>()?.OnDamage(damage);
        }
	}
}
