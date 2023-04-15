using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NavigateableMovement))]
public abstract class MovableEnemy : Enemy {
    private NavigateableMovement enemyMovement;

    private GameObject target;

    void Awake() {
        enemyMovement = GetComponent<NavigateableMovement>();
    }
    
}