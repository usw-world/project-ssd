using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CapsuleCollider))]
// Currently this component working right only with 'Capsule Collider' and 'Box Collider'.
public class Movement : MonoBehaviour {
    #region Collider Information
    [SerializeField] private float cylinderHeight = 1f;
    [SerializeField] private float radius = .5f;
    [SerializeField] private float distanceFromBottom = .4f;
    [SerializeField] private float pullingDistance = .4f;
    [SerializeField] private bool useGravity;

    private float fallingSpeed = 0f;
    private CapsuleBounds colliderBounds {
        get {
            if(ownCollider == null) return new CapsuleBounds();
            return new CapsuleBounds(transform.position + Vector3.up*(distanceFromBottom + radius),
                                     transform.position + Vector3.up*(distanceFromBottom + radius + cylinderHeight),
                                     radius);
        }
    }
    private CapsuleCollider ownCollider;
    [System.NonSerialized] public int blockLayer = 1 << 6;
    public float pullingCoef = .2f;
    // [System.NonSerialized] public bool isPullingdown = true;
    #endregion Collider Information

    protected virtual void Awake() {
        if(!TryGetComponent<CapsuleCollider>(out ownCollider)) {
            Debug.LogError("Movement Component should be with 'CapsuleCollider' in same GameObject.");
        } else {
            cylinderHeight = Mathf.Max(0, ownCollider.height-ownCollider.radius*2);
            distanceFromBottom = (ownCollider.bounds.center.y - transform.position.y) - (Mathf.Max(ownCollider.radius, ownCollider.height*.5f));
            radius = ownCollider.radius * 0.99f;
        }
    }
    protected virtual void Start() {}
    
    public void MoveToward(Vector3 direction, Space space = Space.Self) {
        (Vector3 point1, Vector3 point2, float radius) = colliderBounds.RaycastComponent;
        
        if(space == Space.Self)
            direction = transform.localToWorldMatrix * direction;

        RaycastHit[] hits;
        Debug.DrawLine(point1, point2+ direction + Vector3.down*radius, Color.green);
        Debug.DrawLine(point1+ direction, point2 + Vector3.down*radius, Color.green);
        hits = Physics.CapsuleCastAll(point1, point2, radius, direction, direction.magnitude, blockLayer);
        Vector3 normal = direction.normalized;
        if(hits.Length > 0) {
            foreach(RaycastHit hit in hits) {
                float xx = direction.normalized.x + (Mathf.Abs(direction.normalized.x)>Mathf.Abs(hit.normal.x) ? hit.normal.x : -direction.normalized.x);
                float yy = direction.normalized.y + (Mathf.Abs(direction.normalized.y)>Mathf.Abs(hit.normal.y) ? hit.normal.y : -direction.normalized.y);
                float zz = direction.normalized.z + (Mathf.Abs(direction.normalized.z)>Mathf.Abs(hit.normal.z) ? hit.normal.z : -direction.normalized.z);
                normal = new Vector3(xx, yy, zz);
            }
            Debug.DrawLine(transform.position + normal, transform.position + normal*5, Color.red);
            transform.Translate(direction.magnitude * normal, Space.World);
        } else {
            transform.Translate(direction, Space.World);
        }
    }
    public RaycastHit[] CheckDirection(Vector3 dir, float distance, int layerMask=~0) {
        (Vector3 point1, Vector3 point2, float radius) = colliderBounds.RaycastComponent;
        return Physics.CapsuleCastAll(point1, point2, radius, dir, distance, layerMask);
    }
    private void Pulldown() {
        (Vector3 point1, Vector3 point2, float radius) = colliderBounds.RaycastComponent;
        RaycastHit hit;
        bool isGround = Physics.CapsuleCast(point1, point2, radius, Vector3.down, out hit, distanceFromBottom+pullingDistance, blockLayer);
        float gap = distanceFromBottom - hit.distance;
        
        float pullingForce = gap * pullingCoef;
        if(isGround) {
            fallingSpeed = 0;
            transform.Translate(new Vector3(0, pullingForce, 0));
        } else {
            fallingSpeed += Physics.gravity.y * Time.deltaTime;
        }
        if(useGravity)
            transform.Translate(0, fallingSpeed * Time.deltaTime, 0);
    }
    void Update() {
        Pulldown();
    }
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if(ownCollider != null) {
            (Vector3 point1, Vector3 point2, _) = colliderBounds.RaycastComponent;
            // Gizmos.DrawWireCube(colliderBounds.center, new Vector3(radius*2, cylinderHeight, radius*2));
            Gizmos.DrawWireSphere(point1, radius);
            Gizmos.DrawWireSphere(point2, radius);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(colliderBounds.center + Vector3.down*(cylinderHeight*.5f+radius) + Vector3.down*distanceFromBottom, new Vector3(1, 0, 1));
    }
}