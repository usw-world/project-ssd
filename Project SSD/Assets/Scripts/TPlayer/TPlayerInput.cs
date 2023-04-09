using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TPlayer))]
public class TPlayerInput : MonoBehaviour
{
    private TPlayer player;
    private Vector3 moveVecter;
    private float mouseClikTime = 0;
    private float mouseClikTimeMax = 0.8f;
    private bool sAttackFirst = true;

    RaycastHit hit;
    float MaxDistance = 15f; //Ray�� �Ÿ�(����)

    private void Awake() => player = GetComponent<TPlayer>(); 
    private void Update()
    {
        InputMove();
        SAttackChack();
        if (Input.GetKeyDown(KeyCode.Alpha1))   player.OnDamage();    // �ӽ�
        if (Input.GetKeyDown(KeyCode.Alpha2))   player.OnDown();      // �ӽ�
        if (Input.GetKeyDown(KeyCode.Space))    player.OnSlide();     // �ӽ�
        if (Input.GetMouseButtonDown(0))        player.OnAttack();    // �ӽ�
    }
    void SAttackChack()
    {
        if (Input.GetMouseButtonUp(0))
        {
            sAttackFirst = true;
            mouseClikTime = 0;
        }

        if (sAttackFirst == false) return; 

        if (Input.GetMouseButton(0))
        {
            mouseClikTime += Time.deltaTime;
            if (mouseClikTime >= mouseClikTimeMax)
            {
                sAttackFirst = false;
                mouseClikTime = 0;
                player.OnSAttack();
            }
        }
        
    }
    void InputMove() => player.InputMove( new Vector3( Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical") ) );
}