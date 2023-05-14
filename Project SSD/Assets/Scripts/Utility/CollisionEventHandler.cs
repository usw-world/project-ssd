using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CollisionEventHandler : MonoBehaviour {
    public bool isActive = true;

    public UnityEngine.Events.UnityAction<Collider> onTriggerEnter;
    public UnityEngine.Events.UnityAction<Collider> onTriggerStay;
    public UnityEngine.Events.UnityAction<Collider> onTriggerExit;

    public TriggerEvent triggerEnterEvent;
    public TriggerEvent triggerStayEvent;
    public TriggerEvent triggerExitEvent;

    void OnTriggerEnter(Collider other) {
        if(!isActive) return;
        onTriggerEnter?.Invoke(other);
        triggerEnterEvent?.Invoke();
    }
    void OnTriggerStay(Collider other) {
        if(!isActive) return;
        onTriggerStay?.Invoke(other);
        triggerStayEvent?.Invoke();
    }
    void OnTriggerExit(Collider other) {
        if(!isActive) return;
        onTriggerExit?.Invoke(other);
        triggerExitEvent?.Invoke();
    }

    [System.Serializable]
    public class TriggerEvent : UnityEvent{}
}