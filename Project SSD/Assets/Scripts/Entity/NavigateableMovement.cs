using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]
public class NavigateableMovement : Movement {
    NavMeshAgent navMesh;

    protected override void Awake() {
        navMesh = GetComponent<NavMeshAgent>();
    }
}