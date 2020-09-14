// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Actions/inputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""inputMaster"",
    ""maps"": [
        {
            ""name"": ""actionMap"",
            ""id"": ""16e83b56-9a1c-424e-b5c3-aec198f81e7e"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""a53bf271-f336-4c63-8216-92c6c3715f6b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""c715acba-2c7f-4046-b0fd-b4c756279d4c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""0d39739f-97b3-46f0-9b64-56bdb0a77fad"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""4cbafefa-0833-4338-8ca7-643c7056ea8e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""be6ada4e-48d7-4512-9a96-a105a6fdfcad"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""b9dd0480-a549-422f-abe7-c1038f1386f6"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse&Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""c3d7b300-41c9-480c-ad64-3242a22ebe85"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse&Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""ad5fa82a-f3c7-4820-9e94-92c6923b7259"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse&Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8821772f-26c4-424a-94c2-cabd6c6d866a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse&Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2758971c-d189-4c39-ba59-b726d40f9016"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse&Keyboard"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cbd40e07-480e-4ee5-a72b-dc2a043a4ef9"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse&Keyboard"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mouse&Keyboard"",
            ""bindingGroup"": ""Mouse&Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // actionMap
        m_actionMap = asset.FindActionMap("actionMap", throwIfNotFound: true);
        m_actionMap_Movement = m_actionMap.FindAction("Movement", throwIfNotFound: true);
        m_actionMap_Jump = m_actionMap.FindAction("Jump", throwIfNotFound: true);
        m_actionMap_Attack = m_actionMap.FindAction("Attack", throwIfNotFound: true);
        m_actionMap_Dash = m_actionMap.FindAction("Dash", throwIfNotFound: true);
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

    // actionMap
    private readonly InputActionMap m_actionMap;
    private IActionMapActions m_ActionMapActionsCallbackInterface;
    private readonly InputAction m_actionMap_Movement;
    private readonly InputAction m_actionMap_Jump;
    private readonly InputAction m_actionMap_Attack;
    private readonly InputAction m_actionMap_Dash;
    public struct ActionMapActions
    {
        private @InputMaster m_Wrapper;
        public ActionMapActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_actionMap_Movement;
        public InputAction @Jump => m_Wrapper.m_actionMap_Jump;
        public InputAction @Attack => m_Wrapper.m_actionMap_Attack;
        public InputAction @Dash => m_Wrapper.m_actionMap_Dash;
        public InputActionMap Get() { return m_Wrapper.m_actionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ActionMapActions set) { return set.Get(); }
        public void SetCallbacks(IActionMapActions instance)
        {
            if (m_Wrapper.m_ActionMapActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnAttack;
                @Dash.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnDash;
            }
            m_Wrapper.m_ActionMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
            }
        }
    }
    public ActionMapActions @actionMap => new ActionMapActions(this);
    private int m_MouseKeyboardSchemeIndex = -1;
    public InputControlScheme MouseKeyboardScheme
    {
        get
        {
            if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse&Keyboard");
            return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
        }
    }
    public interface IActionMapActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
    }
}
