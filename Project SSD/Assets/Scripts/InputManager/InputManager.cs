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
    public event Action SAttack;
    public event Action Dodge;
    public event Action CameraZoom;

    public static InputManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this; 
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnMove(InputValue value) => Move?.Invoke(value.Get<Vector3>());
    private void OnDodge() => Dodge?.Invoke(); 
    private void OnJump() => Jump?.Invoke(); 
    private void OnAttack() => Attack?.Invoke(); 
    private void OnSAttack() => SAttack?.Invoke(); 
    private void OnCameraZoom() => CameraZoom?.Invoke(); 
}