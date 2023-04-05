using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {
    public State currentState { get; private set; }

    private void Start() {
        if(currentState == null) {
            Debug.LogError($"StateMachine on '{gameObject.name}' has not any 'currentState'. Please set initial 'currentState' with SetInitialState(State state).");
        }
    }
    public void SetIntialState(State state) {
        if(currentState == null) {
            currentState = state;
        } else {
            Debug.LogError($"StateMachine on '{gameObject.name}' already has initial 'currentState'.");
        }
    }
    public void ChangeState(State nextState, bool intoSelf=true) { 
        if(!intoSelf && nextState==currentState) return;
        if(currentState == null) {
            Debug.LogError($"StateMachine on '{gameObject.name}' has not any 'currentState'. Please set initial 'currentState' with SetInitialState(State state).");
        } else {
            currentState?.onInactive?.Invoke(nextState);
            nextState?.onActive?.Invoke(currentState);
            currentState = nextState;
        }
    }
    public bool Compare(State target) {
        return currentState == target;
    }
    public bool Compare(string tag) {
        return currentState.stateTag == tag;
    }
    private void Update() {
        currentState?.onStay?.Invoke();
    }
    private void FixedUpdate() {
        currentState?.onStayFixed?.Invoke();
    }
}