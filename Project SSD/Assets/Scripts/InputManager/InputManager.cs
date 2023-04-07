using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Managers/InputManager")]
public class InputManager : MonoBehaviour
{
    public event Action<Vector3> Move;
    public event Action Jump;
    public event Action Attack;

    public static InputManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnMove(InputValue value)
    {
        Vector3 moveValue = value.Get<Vector3>();
        Move?.Invoke(moveValue);
    }

    private void OnJump()
    {
        Jump?.Invoke();
    }

    private void OnAttack()
    {
        Attack?.Invoke();
    }
}