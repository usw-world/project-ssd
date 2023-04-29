using UnityEngine;

[RequireComponent(typeof(NavigateableMovement))]
public abstract class MovableEnemy : Enemy {
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float arriveDistance = 0.5f;
    protected bool IsArrive {
        get {
            Vector3 point1 = Vector3.Scale(new Vector3(1, 0, 1), transform.position);
            Vector3 point2 = Vector3.Scale(new Vector3(1, 0, 1), targetPoint);
            if(Vector3.Distance(point1, point2) <= arriveDistance) {
                return true;
            } else {
                return false;
            }
        }
    }
    protected float DistanceToTarget {
        get {
            Vector3 point1 = Vector3.Scale(new Vector3(1, 0, 1), transform.position);
            Vector3 point2 = Vector3.Scale(new Vector3(1, 0, 1), targetPoint);
            return Vector3.Distance(point1, point2);
        }
    }
    protected Vector3 DirectionOfTarget {
        get {
            Vector3 point1 = Vector3.Scale(new Vector3(1, 0, 1), transform.position);
            Vector3 point2 = Vector3.Scale(new Vector3(1, 0, 1), targetPoint);
            return (point2 - point2).normalized;
        }
    }

    protected NavigateableMovement enemyMovement;
    private Vector3 targetPoint;

    protected override void Awake() {
        base.Awake();
        enemyMovement = GetComponent<NavigateableMovement>();
        updateTargetEvent += UpdateTargetEvent;
        lostTargetEvent += LostTargetEvent;
    }
    private void UpdateTargetEvent() {
        targetPoint = target.transform.position;
        ChaseTarget(targetPoint);
    }
    private void LostTargetEvent() {
        OnLostTarget();
    }
    /// <summary>
    /// This method is called when target point is updated.<br/>
    /// <paramref name="point"/> : The point where target is located.
    /// </summary>
    protected abstract void ChaseTarget(Vector3 point);
    protected abstract void OnLostTarget();
}