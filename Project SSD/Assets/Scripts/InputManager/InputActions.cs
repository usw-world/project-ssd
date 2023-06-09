//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/InputManager/InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""Player Map"",
            ""id"": ""352a9f3d-6c84-47ea-a75f-b1ba234b5940"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""eda420a9-0e79-4719-ad79-f01f1a0c7226"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""11cd795f-5e93-4fe1-be70-3d3fee3288d2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SAttack"",
                    ""type"": ""Button"",
                    ""id"": ""c8955d6e-9ac9-418e-afd8-cb39d7d6ad80"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dodge"",
                    ""type"": ""Button"",
                    ""id"": ""9d799f83-8041-468e-8a3f-54ab258ce8c4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CameraZoomIn"",
                    ""type"": ""Value"",
                    ""id"": ""1416dfb4-0927-40e5-8efd-b8bc579becfa"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CameraZoomOut"",
                    ""type"": ""Value"",
                    ""id"": ""2737f8ad-9827-4fee-a1ea-92eac8eb193e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""3D Vector"",
                    ""id"": ""cf0522d3-477d-4a16-8335-ee66a5e68338"",
                    ""path"": ""3DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""forward"",
                    ""id"": ""2ca5f4b0-2e95-472f-892b-59524ebc2df3"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""backward"",
                    ""id"": ""715d1ae6-8f9f-40cf-b7e8-a09237371340"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fcc45ff2-66a0-4a61-89a5-98c36827ecb6"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8eca13d2-8403-4177-8423-28da00e918db"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""25d4ccba-9c33-49fb-a2af-0b5d9031a0a3"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f21c64af-b5dc-4d4b-9935-bae755704f1a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Hold(duration=0.6,pressPoint=0.1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ebe5d88e-e16f-49f5-9cd0-f04064426c6c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2851b1ac-51e4-4821-abcf-ea406d377e92"",
                    ""path"": ""<Mouse>/scroll/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomIn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f581395e-dc1d-4916-b1e9-e2c757533080"",
                    ""path"": ""<Mouse>/scroll/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomOut"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player Map
        m_PlayerMap = asset.FindActionMap("Player Map", throwIfNotFound: true);
        m_PlayerMap_Move = m_PlayerMap.FindAction("Move", throwIfNotFound: true);
        m_PlayerMap_Attack = m_PlayerMap.FindAction("Attack", throwIfNotFound: true);
        m_PlayerMap_SAttack = m_PlayerMap.FindAction("SAttack", throwIfNotFound: true);
        m_PlayerMap_Dodge = m_PlayerMap.FindAction("Dodge", throwIfNotFound: true);
        m_PlayerMap_CameraZoomIn = m_PlayerMap.FindAction("CameraZoomIn", throwIfNotFound: true);
        m_PlayerMap_CameraZoomOut = m_PlayerMap.FindAction("CameraZoomOut", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player Map
    private readonly InputActionMap m_PlayerMap;
    private IPlayerMapActions m_PlayerMapActionsCallbackInterface;
    private readonly InputAction m_PlayerMap_Move;
    private readonly InputAction m_PlayerMap_Attack;
    private readonly InputAction m_PlayerMap_SAttack;
    private readonly InputAction m_PlayerMap_Dodge;
    private readonly InputAction m_PlayerMap_CameraZoomIn;
    private readonly InputAction m_PlayerMap_CameraZoomOut;
    public struct PlayerMapActions
    {
        private @InputActions m_Wrapper;
        public PlayerMapActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerMap_Move;
        public InputAction @Attack => m_Wrapper.m_PlayerMap_Attack;
        public InputAction @SAttack => m_Wrapper.m_PlayerMap_SAttack;
        public InputAction @Dodge => m_Wrapper.m_PlayerMap_Dodge;
        public InputAction @CameraZoomIn => m_Wrapper.m_PlayerMap_CameraZoomIn;
        public InputAction @CameraZoomOut => m_Wrapper.m_PlayerMap_CameraZoomOut;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMapActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerMapActions instance)
        {
            if (m_Wrapper.m_PlayerMapActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnMove;
                @Attack.started -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnAttack;
                @SAttack.started -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnSAttack;
                @SAttack.performed -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnSAttack;
                @SAttack.canceled -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnSAttack;
                @Dodge.started -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnDodge;
                @Dodge.performed -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnDodge;
                @Dodge.canceled -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnDodge;
                @CameraZoomIn.started -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnCameraZoomIn;
                @CameraZoomIn.performed -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnCameraZoomIn;
                @CameraZoomIn.canceled -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnCameraZoomIn;
                @CameraZoomOut.started -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnCameraZoomOut;
                @CameraZoomOut.performed -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnCameraZoomOut;
                @CameraZoomOut.canceled -= m_Wrapper.m_PlayerMapActionsCallbackInterface.OnCameraZoomOut;
            }
            m_Wrapper.m_PlayerMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @SAttack.started += instance.OnSAttack;
                @SAttack.performed += instance.OnSAttack;
                @SAttack.canceled += instance.OnSAttack;
                @Dodge.started += instance.OnDodge;
                @Dodge.performed += instance.OnDodge;
                @Dodge.canceled += instance.OnDodge;
                @CameraZoomIn.started += instance.OnCameraZoomIn;
                @CameraZoomIn.performed += instance.OnCameraZoomIn;
                @CameraZoomIn.canceled += instance.OnCameraZoomIn;
                @CameraZoomOut.started += instance.OnCameraZoomOut;
                @CameraZoomOut.performed += instance.OnCameraZoomOut;
                @CameraZoomOut.canceled += instance.OnCameraZoomOut;
            }
        }
    }
    public PlayerMapActions @PlayerMap => new PlayerMapActions(this);
    public interface IPlayerMapActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnSAttack(InputAction.CallbackContext context);
        void OnDodge(InputAction.CallbackContext context);
        void OnCameraZoomIn(InputAction.CallbackContext context);
        void OnCameraZoomOut(InputAction.CallbackContext context);
    }
}
