using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]
public class NavigateableMovement : Movement {
    NavMeshAgent navMesh;
    NavMeshPath currentPath;
    bool isArrive = true;
    int pathIndex = 0;
    Vector3 nextDestination;
    /// <summary>This value will be multiplied to Time.deltaTime.</summary>
    [SerializeField] private float pointMoveSpeed = 1f;

    protected override void Awake() {
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.updatePosition = false;
    }
    public void MoveToPoint(Vector3 point, float speed) {
        navMesh.nextPosition = transform.position;
        navMesh.SetDestination(point);
        currentPath = navMesh.path;
        pathIndex = 0;
        isArrive = false;
        pointMoveSpeed = speed;
    }
    public void SetSpeed(float nextSpeed) {
        pointMoveSpeed = nextSpeed;
    }
    void Update() {
        if(!isArrive) {
            nextDestination = new Vector3(currentPath.corners[pathIndex].x, transform.position.y, currentPath.corners[pathIndex].z);
            Vector3 dir = (nextDestination - transform.position).normalized;
            MoveToward(dir * pointMoveSpeed * Time.deltaTime);
            CheckArrived();
        }
    }
    void CheckArrived() {
        if(Vector3.Distance(nextDestination, transform.position) < .2f) {
            if(pathIndex+1 >= currentPath.corners.Length) isArrive = true;
            else pathIndex ++;
        }
    }
}