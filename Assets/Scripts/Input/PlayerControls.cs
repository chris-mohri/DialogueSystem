//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Scripts/Input/PlayerControls.inputactions
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

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""207fb73d-78ef-45d7-9299-bf2033456379"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""63def262-d5a5-4c29-bd84-0c483dec05d4"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0049d304-89e4-4b1f-9846-be498e0c6248"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""PassThrough"",
                    ""id"": ""704d905f-d3bb-46e1-87bb-7d2cd9c9c5d6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a2e1e81e-9222-4b17-a294-c802ccceddf0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Skill"",
                    ""type"": ""PassThrough"",
                    ""id"": ""174339cd-a911-42c5-bd9d-e473ce6b8182"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""PassThrough"",
                    ""id"": ""588aab7b-9b22-4bbc-94bd-02c1ff604340"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""e09b98ae-3b67-48cd-ae59-4f9394a098a9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e4aa612b-a1f8-49e0-ad29-2035c9259d85"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""73a9514f-dc6a-4d37-a08e-b3f38e11c8fe"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5fc27eca-8533-4c8a-8160-9f72b4e1bcf5"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""fde19f1f-e887-4c2f-bc9f-eff050015f81"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""fd935c7e-24ef-4392-844f-440b3841290d"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85e0cbc8-32ed-42d2-b206-3ba3e28d901f"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9e00021-0b13-454c-88bb-07f688bcc595"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b64b67c1-2189-446a-85a8-f4c7b68c02ce"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""02aa7393-8671-4cbf-a519-1c1542a4eef7"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Camera"",
            ""id"": ""b9aa2793-b964-43be-8b01-3c72082647d9"",
            ""actions"": [
                {
                    ""name"": ""Mouse Zoom"",
                    ""type"": ""PassThrough"",
                    ""id"": ""2b2745ee-7bd5-4f91-a221-3b9c0dd35aca"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Mouse XY"",
                    ""type"": ""PassThrough"",
                    ""id"": ""6b09e942-5834-41b5-8df7-d3566df57529"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Scroll Press"",
                    ""type"": ""PassThrough"",
                    ""id"": ""61b70b15-2032-4084-a28b-518b7a7145ab"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""51f046e3-3933-40a0-8f03-cfd71303de7a"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8e5644ac-19b9-4cdc-8164-2c99181f0b34"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse XY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be4d0cef-fe3f-46a8-97cb-9b5d6c9d7186"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll Press"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Mouse"",
            ""id"": ""d8389196-63f9-4a70-9633-fe022d3e121a"",
            ""actions"": [
                {
                    ""name"": ""Position"",
                    ""type"": ""PassThrough"",
                    ""id"": ""7e02e1a7-3095-4b3e-a192-6ff5eaaccccf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftHold"",
                    ""type"": ""PassThrough"",
                    ""id"": ""4d10b979-bf32-4e4d-904e-3e6df6835952"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d655b4c6-c290-4072-ba1e-d494f20704b4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""10cc2dc8-8a7a-4266-a911-99c1b4e56597"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e8a52024-cff2-4158-b5fa-5d32bd409a8f"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftHold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""73166c50-3c08-4871-9276-b122c03ed366"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""id"": ""720d34d9-17cc-4081-8278-39b84ded1275"",
            ""actions"": [
                {
                    ""name"": ""Q"",
                    ""type"": ""PassThrough"",
                    ""id"": ""ed711e67-94d8-4123-90f4-ec7c3aba5763"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Continue"",
                    ""type"": ""PassThrough"",
                    ""id"": ""8aa50ad3-edc9-4d33-8c33-be563d786dd0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9b03303e-2952-4b80-b570-da1fb381d100"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Q"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6fa5c0f-8f16-4e93-897a-3aad588645f3"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Continue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4a8deb37-f9f0-4303-863e-ccad417dc7fb"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Continue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8e269db6-2d92-4965-9f53-3e1b74e118e3"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Continue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Sprint = m_Player.FindAction("Sprint", throwIfNotFound: true);
        m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
        m_Player_Skill = m_Player.FindAction("Skill", throwIfNotFound: true);
        m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
        // Camera
        m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
        m_Camera_MouseZoom = m_Camera.FindAction("Mouse Zoom", throwIfNotFound: true);
        m_Camera_MouseXY = m_Camera.FindAction("Mouse XY", throwIfNotFound: true);
        m_Camera_ScrollPress = m_Camera.FindAction("Scroll Press", throwIfNotFound: true);
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_Position = m_Mouse.FindAction("Position", throwIfNotFound: true);
        m_Mouse_LeftHold = m_Mouse.FindAction("LeftHold", throwIfNotFound: true);
        m_Mouse_LeftClick = m_Mouse.FindAction("LeftClick", throwIfNotFound: true);
        // Keyboard
        m_Keyboard = asset.FindActionMap("Keyboard", throwIfNotFound: true);
        m_Keyboard_Q = m_Keyboard.FindAction("Q", throwIfNotFound: true);
        m_Keyboard_Continue = m_Keyboard.FindAction("Continue", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Sprint;
    private readonly InputAction m_Player_Dash;
    private readonly InputAction m_Player_Skill;
    private readonly InputAction m_Player_Interact;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Sprint => m_Wrapper.m_Player_Sprint;
        public InputAction @Dash => m_Wrapper.m_Player_Dash;
        public InputAction @Skill => m_Wrapper.m_Player_Skill;
        public InputAction @Interact => m_Wrapper.m_Player_Interact;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Sprint.started += instance.OnSprint;
            @Sprint.performed += instance.OnSprint;
            @Sprint.canceled += instance.OnSprint;
            @Dash.started += instance.OnDash;
            @Dash.performed += instance.OnDash;
            @Dash.canceled += instance.OnDash;
            @Skill.started += instance.OnSkill;
            @Skill.performed += instance.OnSkill;
            @Skill.canceled += instance.OnSkill;
            @Interact.started += instance.OnInteract;
            @Interact.performed += instance.OnInteract;
            @Interact.canceled += instance.OnInteract;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Sprint.started -= instance.OnSprint;
            @Sprint.performed -= instance.OnSprint;
            @Sprint.canceled -= instance.OnSprint;
            @Dash.started -= instance.OnDash;
            @Dash.performed -= instance.OnDash;
            @Dash.canceled -= instance.OnDash;
            @Skill.started -= instance.OnSkill;
            @Skill.performed -= instance.OnSkill;
            @Skill.canceled -= instance.OnSkill;
            @Interact.started -= instance.OnInteract;
            @Interact.performed -= instance.OnInteract;
            @Interact.canceled -= instance.OnInteract;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Camera
    private readonly InputActionMap m_Camera;
    private List<ICameraActions> m_CameraActionsCallbackInterfaces = new List<ICameraActions>();
    private readonly InputAction m_Camera_MouseZoom;
    private readonly InputAction m_Camera_MouseXY;
    private readonly InputAction m_Camera_ScrollPress;
    public struct CameraActions
    {
        private @PlayerControls m_Wrapper;
        public CameraActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseZoom => m_Wrapper.m_Camera_MouseZoom;
        public InputAction @MouseXY => m_Wrapper.m_Camera_MouseXY;
        public InputAction @ScrollPress => m_Wrapper.m_Camera_ScrollPress;
        public InputActionMap Get() { return m_Wrapper.m_Camera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
        public void AddCallbacks(ICameraActions instance)
        {
            if (instance == null || m_Wrapper.m_CameraActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CameraActionsCallbackInterfaces.Add(instance);
            @MouseZoom.started += instance.OnMouseZoom;
            @MouseZoom.performed += instance.OnMouseZoom;
            @MouseZoom.canceled += instance.OnMouseZoom;
            @MouseXY.started += instance.OnMouseXY;
            @MouseXY.performed += instance.OnMouseXY;
            @MouseXY.canceled += instance.OnMouseXY;
            @ScrollPress.started += instance.OnScrollPress;
            @ScrollPress.performed += instance.OnScrollPress;
            @ScrollPress.canceled += instance.OnScrollPress;
        }

        private void UnregisterCallbacks(ICameraActions instance)
        {
            @MouseZoom.started -= instance.OnMouseZoom;
            @MouseZoom.performed -= instance.OnMouseZoom;
            @MouseZoom.canceled -= instance.OnMouseZoom;
            @MouseXY.started -= instance.OnMouseXY;
            @MouseXY.performed -= instance.OnMouseXY;
            @MouseXY.canceled -= instance.OnMouseXY;
            @ScrollPress.started -= instance.OnScrollPress;
            @ScrollPress.performed -= instance.OnScrollPress;
            @ScrollPress.canceled -= instance.OnScrollPress;
        }

        public void RemoveCallbacks(ICameraActions instance)
        {
            if (m_Wrapper.m_CameraActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICameraActions instance)
        {
            foreach (var item in m_Wrapper.m_CameraActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CameraActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CameraActions @Camera => new CameraActions(this);

    // Mouse
    private readonly InputActionMap m_Mouse;
    private List<IMouseActions> m_MouseActionsCallbackInterfaces = new List<IMouseActions>();
    private readonly InputAction m_Mouse_Position;
    private readonly InputAction m_Mouse_LeftHold;
    private readonly InputAction m_Mouse_LeftClick;
    public struct MouseActions
    {
        private @PlayerControls m_Wrapper;
        public MouseActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Position => m_Wrapper.m_Mouse_Position;
        public InputAction @LeftHold => m_Wrapper.m_Mouse_LeftHold;
        public InputAction @LeftClick => m_Wrapper.m_Mouse_LeftClick;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void AddCallbacks(IMouseActions instance)
        {
            if (instance == null || m_Wrapper.m_MouseActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MouseActionsCallbackInterfaces.Add(instance);
            @Position.started += instance.OnPosition;
            @Position.performed += instance.OnPosition;
            @Position.canceled += instance.OnPosition;
            @LeftHold.started += instance.OnLeftHold;
            @LeftHold.performed += instance.OnLeftHold;
            @LeftHold.canceled += instance.OnLeftHold;
            @LeftClick.started += instance.OnLeftClick;
            @LeftClick.performed += instance.OnLeftClick;
            @LeftClick.canceled += instance.OnLeftClick;
        }

        private void UnregisterCallbacks(IMouseActions instance)
        {
            @Position.started -= instance.OnPosition;
            @Position.performed -= instance.OnPosition;
            @Position.canceled -= instance.OnPosition;
            @LeftHold.started -= instance.OnLeftHold;
            @LeftHold.performed -= instance.OnLeftHold;
            @LeftHold.canceled -= instance.OnLeftHold;
            @LeftClick.started -= instance.OnLeftClick;
            @LeftClick.performed -= instance.OnLeftClick;
            @LeftClick.canceled -= instance.OnLeftClick;
        }

        public void RemoveCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMouseActions instance)
        {
            foreach (var item in m_Wrapper.m_MouseActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MouseActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MouseActions @Mouse => new MouseActions(this);

    // Keyboard
    private readonly InputActionMap m_Keyboard;
    private List<IKeyboardActions> m_KeyboardActionsCallbackInterfaces = new List<IKeyboardActions>();
    private readonly InputAction m_Keyboard_Q;
    private readonly InputAction m_Keyboard_Continue;
    public struct KeyboardActions
    {
        private @PlayerControls m_Wrapper;
        public KeyboardActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Q => m_Wrapper.m_Keyboard_Q;
        public InputAction @Continue => m_Wrapper.m_Keyboard_Continue;
        public InputActionMap Get() { return m_Wrapper.m_Keyboard; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KeyboardActions set) { return set.Get(); }
        public void AddCallbacks(IKeyboardActions instance)
        {
            if (instance == null || m_Wrapper.m_KeyboardActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_KeyboardActionsCallbackInterfaces.Add(instance);
            @Q.started += instance.OnQ;
            @Q.performed += instance.OnQ;
            @Q.canceled += instance.OnQ;
            @Continue.started += instance.OnContinue;
            @Continue.performed += instance.OnContinue;
            @Continue.canceled += instance.OnContinue;
        }

        private void UnregisterCallbacks(IKeyboardActions instance)
        {
            @Q.started -= instance.OnQ;
            @Q.performed -= instance.OnQ;
            @Q.canceled -= instance.OnQ;
            @Continue.started -= instance.OnContinue;
            @Continue.performed -= instance.OnContinue;
            @Continue.canceled -= instance.OnContinue;
        }

        public void RemoveCallbacks(IKeyboardActions instance)
        {
            if (m_Wrapper.m_KeyboardActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IKeyboardActions instance)
        {
            foreach (var item in m_Wrapper.m_KeyboardActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_KeyboardActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public KeyboardActions @Keyboard => new KeyboardActions(this);
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnSkill(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
    }
    public interface ICameraActions
    {
        void OnMouseZoom(InputAction.CallbackContext context);
        void OnMouseXY(InputAction.CallbackContext context);
        void OnScrollPress(InputAction.CallbackContext context);
    }
    public interface IMouseActions
    {
        void OnPosition(InputAction.CallbackContext context);
        void OnLeftHold(InputAction.CallbackContext context);
        void OnLeftClick(InputAction.CallbackContext context);
    }
    public interface IKeyboardActions
    {
        void OnQ(InputAction.CallbackContext context);
        void OnContinue(InputAction.CallbackContext context);
    }
}
