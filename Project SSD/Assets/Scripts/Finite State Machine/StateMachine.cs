using UnityEngine;

public class StateMachine : MonoBehaviour {
    public State currentState { get; protected set; }
    protected State nextState;

    [HideInInspector] public bool isMuted = false;

    protected virtual void Start() {
        if(currentState == null) {
            Debug.LogError($"StateMachine on '{gameObject.name}' has not any 'currentState'. Please set initial 'currentState' with SetInitialState(State state).");
        }
    }
    public void SetIntialState(State state) {
        if(currentState == null) {
            currentState = state;
        } else {
            Debug.LogError($"StateMachine on '{gameObject.name}' already has initial 'currentState' as {currentState}.");
        }
    }
    public virtual void ChangeState(State nextState, bool intoSelf=true) {
        if(isMuted) return;
        if(!intoSelf && nextState==currentState) return;
        if(currentState == null) {
            Debug.LogError($"StateMachine on '{gameObject.name}' has not any 'currentState'. Please set initial 'currentState' with SetInitialState(State state).");
        } else {
            currentState?.onInactive?.Invoke(nextState);
            nextState?.onActive?.Invoke(currentState);
            this.nextState = nextState;
            currentState = nextState;
        }
    }
    public bool Compare(State target) {
        return currentState == target;
    }
    public bool Compare(string tag) {
        return currentState.stateTag == tag;
    }
    protected virtual void Update() {
        currentState?.onStay?.Invoke();
    }
    protected virtual void FixedUpdate() {
        currentState?.onStayFixed?.Invoke();
    }
}