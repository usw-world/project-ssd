using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class _Player : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Rigidbody rbody;
    private Vector3 moveDir;
    private InputManager inputManager;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        inputManager = GameObject.FindObjectOfType<InputManager>();     // 나중에 직접 참조로 변경
        if (inputManager == null)
        {
            Debug.LogError("InputManager not found in scene.");
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(new Vector3(moveDir.x, 0f, moveDir.y) * speed * Time.deltaTime);
    }

    private void HandleMove(Vector3 vec)
    {
        moveDir = vec;
    }

    private void HandleJump()
    {
        rbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    private void OnEnable()
    {
        inputManager.Jump += HandleJump;
        inputManager.Move += HandleMove;
    }

    private void OnDisable()
    {
        inputManager.Jump -= HandleJump;
        inputManager.Move -= HandleMove;
    }
}
