using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* __temporary : Must move to implement class this code after test movement. >> */
[RequireComponent(typeof (Movement))]
/* << __temporary : Must move to implement class this code after test movement. */

/* This gonna be 'Abscract Class'. */
public class Enemy : MonoBehaviour {
    private Movement enemyMovement;
    [SerializeField] float t_moveSpeed = 15f;
    
    private void Awake() {
        enemyMovement = GetComponent<Movement>();
    }
    private void Update() {
        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        enemyMovement.MoveToward(dir * t_moveSpeed * Time.deltaTime);
    }
}