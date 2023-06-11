using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Effect_YBotMineSpell : MonoBehaviour, IPoolableObject {
	#region Network
	private static int nextNetworkId = 0;

	private bool onHost;
	private int networkId = -1;
	#endregion Network

	public bool isActive = false;
	[SerializeField] private float explosionRadius = 3f;
	[SerializeField] private float explosionDelay = .25f;
	[SerializeField] private float setUpDelay = 1f;
	private float currentSetUpDelay = 0;
	private float triggerTime = 0;

	[SerializeField] private float damageAmount = 35f;
	[SerializeField] private float forceScalar = 10f;
	[SerializeField] private float hittingDuration = .75f;

	[SerializeField] private CapsuleCollider capsuleCollider;
	[SerializeField] private DecalProjector delayProjector;
	[SerializeField] private DecalProjector rangeProjector;
	private float duration = 15f;
	private float lifetime = 0;
	private bool hasTriggered = false;
	private bool hasExplosion = false;
	
	[SerializeField] private ParticleSystem apearParticle;
	[SerializeField] private ParticleSystem idleParticle;
	[SerializeField] private ParticleSystem explosionParticle;
	private Coroutine inPoolCoroutine;
	
    public string GetKey() {
        return GetType().ToString();
    }

	private void Awake() {
		onHost = SSDNetworkManager.instance.isHost;
		networkId = nextNetworkId++;
	}
	private void OnEnable() {
		if(inPoolCoroutine != null)
			StopCoroutine(inPoolCoroutine);
		apearParticle.Play();
		idleParticle.Play();
		lifetime = 0;
		triggerTime = 0;
		hasTriggered = false;

		rangeProjector.enabled = true;
		delayProjector.enabled = true;
		rangeProjector.size = new Vector3(0, 0, rangeProjector.size.z);
		delayProjector.size = new Vector3(0, 0, delayProjector.size.z);
		explosionParticle.transform.localScale = Vector3.one * 1/2 * explosionRadius;

		capsuleCollider.radius = explosionRadius;
		capsuleCollider.height = explosionRadius*2 + 5;

		Mirror.NetworkClient.RegisterHandler<TriggerEventMessage>(TriggerMine);
	}
	private void OnDisable() {
		Mirror.NetworkClient.UnregisterHandler<TriggerEventMessage>();
	}
	private void Update() {
		if(hasExplosion)
			return;
			
		if(!isActive) {
			currentSetUpDelay = Mathf.Min(setUpDelay, currentSetUpDelay+Time.deltaTime);
			float projectorRadius = explosionRadius*2 * 1/setUpDelay * currentSetUpDelay;
			rangeProjector.size = new Vector3(projectorRadius, projectorRadius, rangeProjector.size.z);
			if(currentSetUpDelay >= setUpDelay)
				isActive = true;
			return;
		}
			
		lifetime += Time.deltaTime;
		if(lifetime >= duration) {
			rangeProjector.enabled = false;
			delayProjector.enabled = false;
			Disapear();
		}

		if(hasTriggered) {
			triggerTime = Mathf.Min(explosionDelay, triggerTime+Time.deltaTime);
			float projectorRadius = explosionRadius*2 * 1/explosionDelay * triggerTime;
			delayProjector.size = new Vector3(projectorRadius, projectorRadius, delayProjector.size.z);
			if(triggerTime >= explosionDelay) {
				Explosion();
			}
		}
	}
	private void OnTriggerStay(Collider other) {
        if(isActive
		&& onHost
		&& !hasTriggered
		&& other.gameObject.layer == 7) { // Player Layer
			var message = new TriggerEventMessage(this.networkId);
			Mirror.NetworkServer.SendToAll<TriggerEventMessage>(message);
        }
    }
	private void TriggerMine(TriggerEventMessage message) {
		if(this.networkId == message.networkId) {
			Disapear();
			hasTriggered = true;
		}
	}
	private void Disapear() {
		idleParticle.Stop();
		if(inPoolCoroutine != null)
			StopCoroutine(inPoolCoroutine);
		inPoolCoroutine = StartCoroutine(InPoolCoroutine());
	}
	private void Explosion() {
		hasExplosion = true;
		Vector3 point1 = transform.position + Vector3.up*5;
		Vector3 point2 = transform.position - Vector3.up*5;
		Collider[] inners = Physics.OverlapCapsule(point1, point2, explosionRadius, 1<<7);
		for (int i=0; i<inners.Length; i++) {
			TPlayer player = inners[i].GetComponent<TPlayer>();
			if(player != null) {
				Vector3 forceVector = (inners[i].transform.position - this.transform.position).normalized * forceScalar;
				Damage damage = new Damage(
					this.damageAmount,
					this.hittingDuration,
					forceVector,
					Damage.DamageType.Normal
				);
				player.OnDamage(damage);
			}
		}
		rangeProjector.enabled = false;
		delayProjector.enabled = false;
		explosionParticle.Play();
		Disapear();
	}
	private IEnumerator InPoolCoroutine() {
		yield return new WaitForSeconds(5f);
		rangeProjector.enabled = false;
		delayProjector.enabled = false;
		explosionParticle.Stop();
		PoolerManager.instance.InPool(this.GetKey(), this.gameObject);
	}

	private struct TriggerEventMessage : Mirror.NetworkMessage {
		public int networkId;
		public TriggerEventMessage(int networkId) {
			this.networkId = networkId;
		}
	}
}
