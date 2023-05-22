using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_MotionTrail : MonoBehaviour {
    [SerializeField] private ParticleSystem particle;
    private ObjectPooler effectsPooler;
    private Coroutine alphaDecreaseCoroutine;
    [SerializeField] private float maxDuration = 5f;
    
    private void Start() {
        effectsPooler = new ObjectPooler(particle.gameObject, OnInPoolParticle, OnOutPoolParticle, this.transform);
    }
    private void OnInPoolParticle(GameObject gobj) {
        gobj.SetActive(false);
    }
    private void OnOutPoolParticle(GameObject gobj) {
        gobj.SetActive(true);
        gobj.transform.position = transform.position;
        gobj.transform.rotation = transform.rotation;
        StartCoroutine(DelayInPoolCoroutine(gobj, maxDuration));
    }
    private IEnumerator DelayInPoolCoroutine(GameObject target, float delayTime) {
        yield return new WaitForSeconds(delayTime);
        effectsPooler.InPool(target);
    }
    public void GenerateTrail(SkinnedMeshRenderer[] skinnedMeshes) {
        for(int i=0; i<skinnedMeshes.Length; i++) {
            GameObject effect = effectsPooler.OutPool();
            ParticleSystem p;
            ParticleSystemRenderer pr;

            if(effect.TryGetComponent<ParticleSystem>(out p)
            && effect.TryGetComponent<ParticleSystemRenderer>(out pr)) {
                Mesh mesh = new Mesh();
                skinnedMeshes[i].BakeMesh(mesh);
                pr.SetMeshes(new Mesh[] { mesh });
                p.Play();
            }
        }
        
    }
}
