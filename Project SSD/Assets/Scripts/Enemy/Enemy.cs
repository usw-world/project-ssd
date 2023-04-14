using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* __temporary : Must move to implement class this code after test movement. >> */
[RequireComponent(typeof (Movement))]
/* << __temporary : Must move to implement class this code after test movement. */

/* This gonna be 'Abscract Class'. */
public class Enemy : MonoBehaviour {
    [Header("Develop & Debug")]
    private NavigateableMovement enemyMovement;
    [SerializeField] float t_moveSpeed = 5f;
    [SerializeField] float q_moveSpeed = 5f;
    enum MovingType { Third_Person, Quarter_View }
    [SerializeField] MovingType movingType = MovingType.Third_Person;
    
    private void Awake() {
        enemyMovement = GetComponent<NavigateableMovement>();
    }
    private void Update() {
        if(movingType == MovingType.Third_Person) {
            Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            enemyMovement.MoveToward(dir * t_moveSpeed * Time.deltaTime);
        } else if(movingType == MovingType.Quarter_View) {
            if(Input.GetMouseButton(0)) {
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, 1 << 6);
                enemyMovement.MoveToPoint(hit.point, q_moveSpeed);
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
            }
        }
    }
}