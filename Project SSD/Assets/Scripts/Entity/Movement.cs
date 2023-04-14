using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Currently this component working right only with 'Capsule Collider' and 'Box Collider'.
public class Movement : MonoBehaviour {
    #region Front Check
    [System.NonSerialized] public int frontCheckLayer = 1 << 6;
    public Vector3 frontCheckSize;
    #endregion Front Check
    
    #region Bottom Check
    [System.NonSerialized] public bool isPullingdown = true;
    public float pulldownDistance = .2f;
    public float? colliderRadius;
    private Vector3 ColliderSize { get { return movementCollider.bounds.size; } }
    [Range(0, 1f)]
    public float pullingCoef = .2f;
    #endregion Bototm Check

    Collider movementCollider;
    private int bottomCheckLayerMask = 1 << 6;

    protected virtual void Awake() {}
    protected virtual void Start() {
        movementCollider = GetComponent<Collider>();

        CapsuleCollider ccol;
        if(TryGetComponent<CapsuleCollider>(out ccol)) {
            colliderRadius = ccol.radius;
        }
    }
    
    public void MoveToward(Vector3 direction) {
        Debug.DrawLine( movementCollider.bounds.center + (new Vector3(0, ColliderSize.y*.5f, 0)), 
                        movementCollider.bounds.center - (new Vector3(0, ColliderSize.y*.5f - pulldownDistance, 0)));
        RaycastHit[] hits;
        if(colliderRadius.HasValue) { // this means same to if collider that Gobj has is 'Capsule Collider',
            hits = Physics.CapsuleCastAll(movementCollider.bounds.center + (new Vector3(0, ColliderSize.y*.5f - colliderRadius.Value, 0)),
                                          movementCollider.bounds.center - (new Vector3(0, ColliderSize.y*.5f - colliderRadius.Value - pulldownDistance, 0)),
                                          colliderRadius.Value,
                                          direction,
                                          direction.magnitude,
                                          frontCheckLayer);
        } else { // or Box Collider
            hits = Physics.BoxCastAll(movementCollider.bounds.center + new Vector3(0, pulldownDistance*.5f, 0), 
                                      (frontCheckSize + new Vector3(0, -pulldownDistance, 0)),
                                      direction,
                                      transform.rotation,
                                      direction.magnitude, 
                                      frontCheckLayer);
        }
        if(hits.Length > 0) {
            Vector3 rdir = Vector3.zero;
            foreach(RaycastHit hit in hits) {
                rdir += hit.normal;
            }
            rdir = new Vector3(Mathf.Clamp(rdir.x, -1, 1), Mathf.Clamp(rdir.y, -1, 1), Mathf.Clamp(rdir.z, -1, 1));
            transform.Translate(direction.magnitude * (direction.normalized + rdir));
        } else {
            transform.Translate(direction);
        }
        if(isPullingdown)
            Pulldown();
    }
    private void Pulldown() {
        if(movementCollider == null) return;
        RaycastHit hit;
        bool isGround;
        if(colliderRadius.HasValue) { // this means same to if collider that Gobj has is 'Capsule Collider',
            isGround = Physics.CapsuleCast( movementCollider.bounds.center + (new Vector3(0, ColliderSize.y*.5f - colliderRadius.Value, 0)),
                                            movementCollider.bounds.center - (new Vector3(0, ColliderSize.y*.5f - colliderRadius.Value - pulldownDistance, 0)),
                                            colliderRadius.Value,
                                            Vector3.down,
                                            out hit,
                                            pulldownDistance*2,
                                            bottomCheckLayerMask);
        } else { // or Box Collider
            isGround = Physics.BoxCast(movementCollider.bounds.center,
                                        ColliderSize * .5f,
                                        Vector3.down,
                                        out hit,
                                        Quaternion.identity,
                                        pulldownDistance + ColliderSize.y*.5f,
                                        bottomCheckLayerMask);
        }
        float gap = (hit.distance + colliderRadius.Value) - ColliderSize.y*.5f;
        float pullingForce = -gap * pullingCoef;
        if(isGround && gap>.01f) {
            transform.Translate(new Vector3(0, pullingForce, 0));
        }
    }
}