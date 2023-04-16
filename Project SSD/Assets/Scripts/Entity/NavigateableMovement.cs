using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]
public class NavigateableMovement : Movement {
    private NavMeshAgent navMesh;
    private NavMeshPath currentPath;
    private bool isArrive = true;
    private int pathIndex = 0;
    private Vector3 nextDestination;
    /// <summary>This value will be multiplied to Time.deltaTime.</summary>
    [SerializeField] private float pointMoveSpeed = 1f;

    protected override void Awake() {
        base.Awake();
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.updatePosition = false;
        navMesh.updateRotation = false;
        navMesh.speed = 0;
        navMesh.angularSpeed = 0;
    }
    public void MoveToPoint(Vector3 point, float speed) {
        navMesh.nextPosition = transform.position;
        navMesh.SetDestination(point);
        currentPath = navMesh.path;
        pathIndex = 0;
        isArrive = false;
        pointMoveSpeed = speed;
    }
    public void Stop() {
        isArrive = true;
    }
    public void SetSpeed(float nextSpeed) {
        pointMoveSpeed = nextSpeed;
    }
    private void Update() {
        if(!isArrive) {
            nextDestination = new Vector3(currentPath.corners[pathIndex].x, transform.position.y, currentPath.corners[pathIndex].z);
            Vector3 dir = (nextDestination - transform.position).normalized;
            MoveToward(dir * pointMoveSpeed * Time.deltaTime, Space.World);
            CheckArrived();
        }
    }
    private void CheckArrived() {
        if(Vector3.Distance(nextDestination, transform.position) < .2f) {
            if(pathIndex+1 >= currentPath.corners.Length) isArrive = true;
            else pathIndex ++;
        }
    }
}