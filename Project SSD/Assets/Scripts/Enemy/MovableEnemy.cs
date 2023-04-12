using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NavigateableMovement))]
public class MovableEnemy : Enemy {
    private NavigateableMovement enemyMovement;

    private GameObject target;
    private Transform targetPoint;

    void Awake() {
        enemyMovement = GetComponent<NavigateableMovement>();
    }
    
}