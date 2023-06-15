using System;
using UnityEngine;

public class DestroyableObject: MonoBehaviour, IDamageable {
    public static int nextNetworkId = 0;
    private int networkId = -1;

    private bool isDestroyed = false;
    Vector3[] originChildrenPositions;
    Quaternion[] originChildrenRotations;
    Transform[] children;
    Rigidbody[] childrenRigidbodies;

    public void Awake() {
        children = GetComponentsInChildren<Transform>();
        originChildrenPositions = new Vector3[children.Length];
        originChildrenRotations = new Quaternion[children.Length];
        for(int i=0; i<children.Length; i++) {
            originChildrenPositions[i] = children[i].position;
            originChildrenRotations[i] = children[i].rotation;
        }
        childrenRigidbodies = GetComponentsInChildren<Rigidbody>();

        networkId = nextNetworkId ++;
        SSDNetworkManager.instance.RegisterHandler<ObjectDamageMessage>(TakeDamage);
    }

    public void OnDamage(Damage damage) {
        Mirror.NetworkServer.SendToAll<ObjectDamageMessage>(new ObjectDamageMessage(this.networkId, damage));
    }
    public void TakeDamage(Mirror.NetworkMessage message) {
        try {
            ObjectDamageMessage _message = (ObjectDamageMessage) message;
            int targetId = _message.targetId;
            if(networkId != targetId)
                return;

            Damage damage = _message.damage;

            if(!isDestroyed) {
                isDestroyed = true;
                foreach(Rigidbody rb in childrenRigidbodies) {
                    rb.isKinematic = false;
                    rb.AddForce(damage.forceVector, ForceMode.VelocityChange);
                }
                gameObject.layer = 9;
                GetComponent<Collider>().enabled = false;
            }
        } catch(System.InvalidCastException e) {
            Debug.LogError(e);
        }
    }
    public void Repair() {
        if(isDestroyed) {
            GetComponent<Collider>().enabled = true;
            gameObject.layer = 8;
            isDestroyed = false;
            for(int i=0; i<childrenRigidbodies.Length; i++) {
                childrenRigidbodies[i].isKinematic = true;
            }
            for(int i=0; i<children.Length; i++) {
                children[i].position = originChildrenPositions[i];
                children[i].rotation = originChildrenRotations[i];
            }
        }
    }
}
public struct ObjectDamageMessage : Mirror.NetworkMessage {
    public int targetId;
    public Damage damage;
    public ObjectDamageMessage(int networkId, Damage damage) {
        this.targetId = networkId;
        this.damage = damage;
    }
}