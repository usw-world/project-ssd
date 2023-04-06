using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TPlayer))]
public class TPlayerInput : MonoBehaviour
{
    private TPlayer player;
    private Vector3 moveVecter;

    RaycastHit hit;
    float MaxDistance = 15f; //Ray의 거리(길이)

    private void Awake() => player = GetComponent<TPlayer>(); 
    private void Update()
    {
        InputMove();
        if (Input.GetKeyDown(KeyCode.Alpha1))   player.OnDamage();    // 임시
        if (Input.GetKeyDown(KeyCode.Alpha2))   player.OnDown();      // 임시
        if (Input.GetKeyDown(KeyCode.Tab))      player.OnSwap();      // 임시
        if (Input.GetKeyDown(KeyCode.Space))    player.OnSlide();     // 임시
        if (Input.GetMouseButton(0))            player.OnAttack(true);// 임시
        else                                    player.OnAttack(false);// 임시 

    }
    public void InputMove() => player.InputMove( new Vector3( Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical") ) );
}