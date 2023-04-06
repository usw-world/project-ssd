using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TPlayer))]
public class TPlayerInput : MonoBehaviour
{
    private TPlayer player;
    private Vector3 moveVecter;

    RaycastHit hit;
    float MaxDistance = 15f; //Ray�� �Ÿ�(����)

    private void Awake() => player = GetComponent<TPlayer>(); 
    private void Update()
    {
        InputMove();
        if (Input.GetKeyDown(KeyCode.Alpha1))   player.OnDamage();    // �ӽ�
        if (Input.GetKeyDown(KeyCode.Alpha2))   player.OnDown();      // �ӽ�
        if (Input.GetKeyDown(KeyCode.Tab))      player.OnSwap();      // �ӽ�
        if (Input.GetKeyDown(KeyCode.Space))    player.OnSlide();     // �ӽ�
        if (Input.GetMouseButton(0))            player.OnAttack(true);// �ӽ�
        else                                    player.OnAttack(false);// �ӽ� 

    }
    public void InputMove() => player.InputMove( new Vector3( Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical") ) );
}