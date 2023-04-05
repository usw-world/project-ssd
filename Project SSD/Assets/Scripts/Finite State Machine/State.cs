using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State {
    public delegate void ChangeEvent();
    public delegate void ChangeEventWithState(State state = null);
    string stateName;
    public string stateTag { get; private set; } = null;

    public State(string stateName) {
        this.stateName = stateName;
    }
    public State(string stateName, string stateTag) {
        this.stateName = stateName;
        this.stateTag = stateTag;
    }

    public ChangeEventWithState onActive;
    public ChangeEvent onStay;
    public ChangeEvent onStayFixed;
    public ChangeEventWithState onInactive;

    public override string ToString() {
        return stateName;
    }
    public bool Compare(State state) {
        return this == state;
    }
    public bool Compare(string stateTag) {
        return this.stateTag == stateTag;
    }
    /* 
        Usage Example >>

        State jumpState = new State("Jump");
        jumpState.OnActive += () => {
            playerGameObject.ownRigidbody.addForce(Vector3.up * jumpPower);
        };
    */
}