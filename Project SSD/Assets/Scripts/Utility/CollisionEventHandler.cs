using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionEventHandler : MonoBehaviour {
    public bool isActive = true;

    public UnityEngine.Events.UnityAction<Collider> onTriggerEnter;
    public UnityEngine.Events.UnityAction<Collider> onTriggerStay;
    public UnityEngine.Events.UnityAction<Collider> onTriggerExit;

    void OnTriggerEnter(Collider other) {
        if(!isActive) return;
        onTriggerEnter?.Invoke(other);
    }
    void OnTriggerStay(Collider other) {
        if(!isActive) return;
        onTriggerStay?.Invoke(other);
    }
    void OnTriggerExit(Collider other) {
        if(!isActive) return;
        onTriggerExit?.Invoke(other);
    }
}