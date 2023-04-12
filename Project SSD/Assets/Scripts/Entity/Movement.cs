using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CapsuleCollider))]
// Currently this component working right only with 'Capsule Collider' and 'Box Collider'.
public class Movement : MonoBehaviour {
    #region Collider Information
    [SerializeField] private float height = 1f;
    [SerializeField] private float radius = .5f;
    [SerializeField] private float distanceFromBottom = .4f;
    private CapsuleBounds colliderBounds {
        get {
            if(ownCollider == null) return new CapsuleBounds();
            return new CapsuleBounds(transform.position - Vector3.up*(height*.5f - distanceFromBottom - radius),
                                     transform.position + Vector3.up*(height*.5f + distanceFromBottom + radius),
                                     radius);
        }
    }
    private CapsuleCollider ownCollider;
    [System.NonSerialized] public int blockLayer = 1 << 6;
    public float pullingCoef = .2f;
    // [System.NonSerialized] public bool isPullingdown = true;
    #endregion Collider Information

    protected virtual void Awake() {}
    protected virtual void Start() {
        if(!TryGetComponent<CapsuleCollider>(out ownCollider)) {
            Debug.LogError("Movement Component should be with 'CapsuleCollider' in same GameObject.");
        }
    }
    
    public void MoveToward(Vector3 direction) {
        (Vector3 point1, Vector3 point2, float radius) = colliderBounds.RaycastComponent;
        // Debug.DrawLine(colliderBounds.center + (new Vector3(0, colliderBounds.height*.5f, 0)), 
        //                colliderBounds.center - (new Vector3(0, colliderBounds.height*.5f - distanceFromBottom, 0)),
        //                Color.green);
        direction = transform.localToWorldMatrix * direction;
        RaycastHit[] hits;
        Debug.DrawLine(point1, point2+ direction + Vector3.down*radius, Color.green);
        Debug.DrawLine(point1+ direction, point2 + Vector3.down*radius, Color.green);
        hits = Physics.CapsuleCastAll(point1, point2, radius, direction, direction.magnitude, blockLayer);
        print(direction.normalized);
        foreach(RaycastHit hit in hits) {
            // print(hit.distance);
        }
        if(hits.Length > 0) {
            Vector3 rdir = Vector3.zero;
            foreach(RaycastHit hit in hits) {
                // print(hit.transform.name);
                print(hit.normal);
                rdir += hit.normal;
            }
            rdir = new Vector3(Mathf.Clamp(rdir.x, -1, 1), Mathf.Clamp(rdir.y, -1, 1), Mathf.Clamp(rdir.z, -1, 1));
            transform.Translate(direction.magnitude * (direction.normalized + rdir).normalized, Space.World);
        } else {
            transform.Translate(direction, Space.World);
        }

        Pulldown();
    }
    private void Pulldown() {
        (Vector3 point1, Vector3 point2, float radius) = colliderBounds.RaycastComponent;
        RaycastHit hit;
        bool isGround = Physics.CapsuleCast(point1, point2, radius, Vector3.down, out hit, distanceFromBottom, blockLayer);
        float gap = (hit.distance);
        float pullingForce = -gap * pullingCoef;
        if(isGround && gap>.01f) {
            transform.Translate(new Vector3(0, pullingForce, 0));
        }
    }
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if(ownCollider != null)
            Gizmos.DrawWireCube(colliderBounds.center, new Vector3(radius*2, height+radius, radius*2));
    }
}