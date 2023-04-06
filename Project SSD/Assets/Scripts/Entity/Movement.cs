using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour {
    #region Front Check
    [System.NonSerialized] public float blockForwardDistance = 0.05f;
    [System.NonSerialized] public int blockLayerMask = 1 << 6;
    public Vector3 frontCheckSize = new Vector3(0.15f, 0.1f, 0.05f);
    public Vector3 frontCheckStartPoint = new Vector3(0, 0, 0.5f);
    #endregion Front Check
    
    #region Bottom Check
    public float pulldownDistance = 1f;
    public float? colliderRadius;
    private Vector3 colliderSize;
    [Range(0, 10f)]
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
        bool canMove = !Physics.BoxCast(transform.position + frontCheckStartPoint, 
                                        frontCheckSize, direction, 
                                        Quaternion.identity, 
                                        blockForwardDistance, 
                                        blockLayerMask);
        if(canMove) {
            transform.Translate(direction);
        }
        Pulldown();
    }
    private void Pulldown() {
        if(movementCollider == null) return;
        RaycastHit hit;
        bool isGround;
        Bounds bounds = movementCollider.bounds;
        if(colliderRadius.HasValue) { // if collider that Gobj has is 'Capsule Collider',
            isGround = Physics.SphereCast(bounds.center,
                                        colliderRadius.Value,
                                        Vector3.down,
                                        out hit,
                                        pulldownDistance + bounds.size.y*.5f,
                                        bottomCheckLayerMask);
        } else { // or Box Collider
            isGround = Physics.BoxCast(bounds.center,
                                        colliderSize * .5f,
                                        Vector3.down,
                                        out hit,
                                        Quaternion.identity,
                                        pulldownDistance + bounds.size.y*.5f,
                                        bottomCheckLayerMask);
        }
        float gap = (hit.distance + colliderRadius.Value) - bounds.size.y*.5f;
        float pullingForce = -gap * pullingCoef;
        Debug.DrawLine(bounds.center, bounds.center + Vector3.right, Color.red);
        Debug.DrawLine(hit.point, hit.point + Vector3.right);
        if(isGround && gap>.01f) {
            transform.Translate(new Vector3(0, pullingForce, 0));
        }
    }
}