using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Matches the UI Model to the state of the Interactor.
    /// </summary>
    public interface IUIInteractor
    {
        /// <summary>
        /// Updates the current UI Model to match the state of the Interactor.
        /// </summary>
        /// <param name="model">The returned model that will match this Interactor.</param>
        void UpdateUIModel(ref TrackedDeviceModel model);

        /// <summary>
        /// Attempts to retrieve the current UI Model.
        /// </summary>
        /// <param name="model">The returned model that reflects the UI state of this Interactor.</param>
        /// <returns>Returns <see langword="true"/> if the model was able to retrieved. Otherwise, returns <see langword="false"/>.</returns>
        bool TryGetUIModel(out TrackedDeviceModel model);
    }

    /// <summary>
    /// Custom class for input modules that send UI input in XR.
    /// </summary>
    [AddComponentMenu("Event/XR UI Input Module", 11)]
    [HelpURL(XRHelpURLConstants.k_XRUIInputModule)]
    public partial class XRUIInputModule : UIInputModule
    {
        struct RegisteredInteractor
        {
            public IUIInteractor interactor;
            public TrackedDeviceModel model;

            public RegisteredInteractor(IUIInteractor interactor, int deviceIndex)
            {
                this.interactor = interactor;
                model = new TrackedDeviceModel(deviceIndex);
            }
        }

        struct RegisteredTouch
        {
            public bool isValid;
            public int touchId;
            public TouchModel model;

            public RegisteredTouch(Touch touch, int deviceIndex)
            {
                touchId = touch.fingerId;
                model = new TouchModel(deviceIndex);
                isValid = true;
            }
        }

        /// <summary>
        /// Represents which Active Input Mode will be used in the situation where the Active Input Handling project setting is set to Both.
        /// </summary>
        /// /// <seealso cref="activeInputMode"/>
        public enum ActiveInputMode
        {
            /// <summary>
            /// Only use input polled through the built-in Unity Input Manager (Old).
            /// </summary>
            InputManagerBindings,

            /// <summary>
            /// Only use input polled from <see cref="InputActionReference"/> through the newer Input System package.
            /// </summary>
            InputSystemActions,

            /// <summary>
            /// Scan through input from both Unity Input Manager and Input System action references.
            /// Note: This may cause undesired effects or may impact performance if input configuration is duplicated.
            /// </summary>
            Both,
        }

#if !ENABLE_INPUT_SYSTEM || !ENABLE_LEGACY_INPUT_MANAGER
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Represents which Active Input Mode will be used in the situation where the Active Input Handling project setting is set to Both.")]
        ActiveInputMode m_ActiveInputMode;

        /// <summary>
        /// Configures which Active Input Mode will be used in the situation where the Active Input Handling project setting is set to Both.
        /// </summary>
        /// <seealso cref="ActiveInputMode"/>
        public ActiveInputMode activeInputMode
        {
            get => m_ActiveInputMode;
            set => m_ActiveInputMode = value;
        }

        [SerializeField, HideInInspector]
        [Tooltip("The maximum distance to ray cast with tracked devices to find hit objects.")]
        float m_MaxTrackedDeviceRaycastDistance = 1000f;

        [Header("Input Devices")]
        [SerializeField]
        [Tooltip("If true, will forward 3D tracked device data to UI elements.")]
        bool m_EnableXRInput = true;

        /// <summary>
        /// If <see langword="true"/>, will forward 3D tracked device data to UI elements.
        /// </summary>
        public bool enableXRInput
        {
            get => m_EnableXRInput;
            set => m_EnableXRInput = value;
        }

        [SerializeField]
        [Tooltip("If true, will forward 2D mouse data to UI elements.")]
        bool m_EnableMouseInput = true;

        /// <summary>
        /// If <see langword="true"/>, will forward 2D mouse data to UI elements.
        /// </summary>
        public bool enableMouseInput
        {
            get => m_EnableMouseInput;
            set => m_EnableMouseInput = value;
        }

        [SerializeField]
        [Tooltip("If true, will forward 2D touch data to UI elements.")]
        bool m_EnableTouchInput = true;

        /// <summary>
        /// If <see langword="true"/>, will forward 2D touch data to UI elements.
        /// </summary>
        public bool enableTouchInput
        {
            get => m_EnableTouchInput;
            set => m_EnableTouchInput = value;
        }

#if ENABLE_INPUT_SYSTEM
        [Header("Input System UI Actions")]
#else
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Pointer input action reference, such as a mouse or single-finger touch device.")]
        InputActionReference m_PointAction;
        /// <summary>
        /// The Input System action to use to move the pointer on the currently active UI. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionReference pointAction
        {
            get => m_PointAction;
            set => SetInputAction(ref m_PointAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Left-click input action reference, typically the left button on a mouse.")]
        InputActionReference m_LeftClickAction;
        /// <summary>
        /// The Input System action to use to determine whether the left button of a pointer is pressed. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionReference leftClickAction
        {
            get => m_LeftClickAction;
            set => SetInputAction(ref m_LeftClickAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Middle-click input action reference, typically the middle button on a mouse.")]
        InputActionReference m_MiddleClickAction;
        /// <summary>
        /// The Input System action to use to determine whether the middle button of a pointer is pressed. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionReference middleClickAction
        {
            get => m_MiddleClickAction;
            set => SetInputAction(ref m_MiddleClickAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Right-click input action reference, typically the right button on a mouse.")]
        InputActionReference m_RightClickAction;
        /// <summary>
        /// The Input System action to use to determine whether the right button of a pointer is pressed. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionReference rightClickAction
        {
            get => m_RightClickAction;
            set => SetInputAction(ref m_RightClickAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Scroll wheel input action reference, typically the scroll wheel on a mouse.")]
        InputActionReference m_ScrollWheelAction;
        /// <summary>
        /// The Input System action to use to move the pointer on the currently active UI. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionReference scrollWheelAction
        {
            get => m_ScrollWheelAction;
            set => SetInputAction(ref m_ScrollWheelAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Navigation input action reference will change which UI element is currently selected to the one up, down, left of or right of the currently selected one.")]
        InputActionReference m_NavigateAction;
        /// <summary>
        /// The Input System action to use to navigate the currently active UI. Must be a <see cref="Vector2Control"/> Control.
        /// </summary>
        public InputActionReference navigateAction
        {
            get => m_NavigateAction;
            set => SetInputAction(ref m_NavigateAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Submit input action reference will trigger a submission of the currently selected UI in the Event System.")]
        InputActionReference m_SubmitAction;
        /// <summary>
        /// The Input System action to use for submitting or activating a UI element. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionReference submitAction
        {
            get => m_SubmitAction;
            set => SetInputAction(ref m_SubmitAction, value);
        }

#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Cancel input action reference will trigger canceling out of the currently selected UI in the Event System.")]
        InputActionReference m_CancelAction;
        /// <summary>
        /// The Input System action to use for cancelling or backing out of a UI element. Must be a <see cref="ButtonControl"/> Control.
        /// </summary>
        public InputActionReference cancelAction
        {
            get => m_CancelAction;
            set => SetInputAction(ref m_CancelAction, value);
        }

#if ENABLE_LEGACY_INPUT_MANAGER
        [Header("Input Manager (Old) Gamepad/Joystick Bindings")]
#else
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("If true, will use the Input Manager (Old) configuration to forward gamepad data to UI elements.")]
        bool m_EnableGamepadInput = true;

        /// <summary>
        /// If <see langword="true"/>, will forward gamepad data to UI elements.
        /// </summary>
        public bool enableGamepadInput
        {
            get => m_EnableGamepadInput;
            set => m_EnableGamepadInput = value;
        }

#if !ENABLE_LEGACY_INPUT_MANAGER
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("If true, will use the Input Manager (Old) configuration to forward joystick data to UI elements.")]
        bool m_EnableJoystickInput = true;

        /// <summary>
        /// If <see langword="true"/>, will forward joystick data to UI elements.
        /// </summary>
        public bool enableJoystickInput
        {
            get => m_EnableJoystickInput;
            set => m_EnableJoystickInput = value;
        }

#if !ENABLE_LEGACY_INPUT_MANAGER
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Name of the horizontal axis for gamepad/joystick UI navigation when using the old Input Manager.")]
        string m_HorizontalAxis = "Horizontal";

        /// <summary>
        /// Name of the horizontal axis for UI navigation when using the old Input Manager.
        /// </summary>
        public string horizontalAxis
        {
            get => m_HorizontalAxis;
            set => m_HorizontalAxis = value;
        }

#if !ENABLE_LEGACY_INPUT_MANAGER
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Name of the vertical axis for gamepad/joystick UI navigation when using the old Input Manager.")]
        string m_VerticalAxis = "Vertical";

        /// <summary>
        /// Name of the vertical axis for UI navigation when using the old Input Manager.
        /// </summary>
        public string verticalAxis
        {
            get => m_VerticalAxis;
            set => m_VerticalAxis = value;
        }

#if !ENABLE_LEGACY_INPUT_MANAGER
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Name of the gamepad/joystick button to use for UI selection or submission when using the old Input Manager.")]
        string m_SubmitButton = "Submit";

        /// <summary>
        /// Name of the gamepad/joystick button to use for UI selection or submission when using the old Input Manager.
        /// </summary>
        public string submitButton
        {
            get => m_SubmitButton;
            set => m_SubmitButton = value;
        }

#if !ENABLE_LEGACY_INPUT_MANAGER
        [HideInInspector]
#endif
        [SerializeField]
        [Tooltip("Name of the gamepad/joystick button to use for UI cancel or back commands when using the old Input Manager.")]
        string m_CancelButton = "Cancel";

        /// <summary>
        /// Name of the gamepad/joystick button to use for UI cancel or back commands when using the old Input Manager.
        /// </summary>
        public string cancelButton
        {
            get => m_CancelButton;
            set => m_CancelButton = value;
        }

        int m_RollingPointerId;

        MouseModel m_MouseState;
        NavigationModel m_NavigationState;

        internal const float kPixelPerLine = 20f;

        readonly List<RegisteredTouch> m_RegisteredTouches = new List<RegisteredTouch>();
        readonly List<RegisteredInteractor> m_RegisteredInteractors = new List<RegisteredInteractor>();

        /// <summary>
        /// See <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnEnable.html">MonoBehavior.OnEnable</a>.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            // Check active input mode is correct
#if !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            m_ActiveInputMode = ActiveInputMode.InputManagerBindings;
#elif ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            m_ActiveInputMode = ActiveInputMode.InputSystemActions;
#endif
            m_MouseState = new MouseModel(m_RollingPointerId++);
            m_NavigationState = new NavigationModel();

            if (m_ActiveInputMode != ActiveInputMode.InputManagerBindings)
                EnableAllActions();
        }

        /// <summary>
        /// See <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDisable.html">MonoBehavior.OnDisable</a>.
        /// </summary>
        protected override void OnDisable()
        {
            RemovePointerEventData(m_MouseState.pointerId);

            if (m_ActiveInputMode != ActiveInputMode.InputManagerBindings)
                DisableAllActions();

            base.OnDisable();
        }

        /// <summary>
        /// Register an <see cref="IUIInteractor"/> with the UI system.
        /// Calling this will enable it to start interacting with UI.
        /// </summary>
        /// <param name="interactor">The <see cref="IUIInteractor"/> to use.</param>
        public void RegisterInteractor(IUIInteractor interactor)
        {
            for (var i = 0; i < m_RegisteredInteractors.Count; i++)
            {
                if (m_RegisteredInteractors[i].interactor == interactor)
                    return;
            }

            m_RegisteredInteractors.Add(new RegisteredInteractor(interactor, m_RollingPointerId++));
        }

        /// <summary>
        /// Unregisters an <see cref="IUIInteractor"/> with the UI system.
        /// This cancels all UI Interaction and makes the <see cref="IUIInteractor"/> no longer able to affect UI.
        /// </summary>
        /// <param name="interactor">The <see cref="IUIInteractor"/> to stop using.</param>
        public void UnregisterInteractor(IUIInteractor interactor)
        {
            for (var i = 0; i < m_RegisteredInteractors.Count; i++)
            {
                if (m_RegisteredInteractors[i].interactor == interactor)
                {
                    var registeredInteractor = m_RegisteredInteractors[i];
                    registeredInteractor.interactor = null;
                    m_RegisteredInteractors[i] = registeredInteractor;
                    return;
                }
            }
        }

        /// <summary>
        /// Gets an <see cref="IUIInteractor"/> from its corresponding Unity UI Pointer Id.
        /// This can be used to identify individual Interactors from the underlying UI Events.
        /// </summary>
        /// <param name="pointerId">A unique integer representing an object that can point at UI.</param>
        /// <returns>Returns the interactor associated with <paramref name="pointerId"/>.
        /// Returns <see langword="null"/> if no Interactor is associated (e.g. if it's a mouse event).</returns>
        public IUIInteractor GetInteractor(int pointerId)
        {
            for (var i = 0; i < m_RegisteredInteractors.Count; i++)
            {
                if (m_RegisteredInteractors[i].model.pointerId == pointerId)
                {
                    return m_RegisteredInteractors[i].interactor;
                }
            }

            return null;
        }

        /// <summary>Retrieves the UI Model for a selected <see cref="IUIInteractor"/>.</summary>
        /// <param name="interactor">The <see cref="IUIInteractor"/> you want the model for.</param>
        /// <param name="model">The returned model that reflects the UI state of the <paramref name="interactor"/>.</param>
        /// <returns>Returns <see langword="true"/> if the model was able to retrieved. Otherwise, returns <see langword="false"/>.</returns>
        public bool GetTrackedDeviceModel(IUIInteractor interactor, out TrackedDeviceModel model)
        {
            for (var i = 0; i < m_RegisteredInteractors.Count; i++)
            {
                if (m_RegisteredInteractors[i].interactor == interactor)
                {
                    model = m_RegisteredInteractors[i].model;
                    return true;
                }
            }

            model = new TrackedDeviceModel(-1);
            return false;
        }

        /// <inheritdoc />
        protected override void DoProcess()
        {
            if (m_EnableXRInput)
            {
                for (var i = 0; i < m_RegisteredInteractors.Count; i++)
                {
                    var registeredInteractor = m_RegisteredInteractors[i];

                    // If device is removed, we send a default state to unclick any UI
                    if (registeredInteractor.interactor == null)
                    {
                        registeredInteractor.model.Reset(false);
                        ProcessTrackedDevice(ref registeredInteractor.model, true);
                        RemovePointerEventData(registeredInteractor.model.pointerId);
                        m_RegisteredInteractors.RemoveAt(i--);
                    }
                    else
                    {
                        registeredInteractor.interactor.UpdateUIModel(ref registeredInteractor.model);
                        ProcessTrackedDevice(ref registeredInteractor.model);
                        m_RegisteredInteractors[i] = registeredInteractor;
                    }
                }
            }

            // Touch needs to take precedence because of the mouse emulation layer
            var hasTouches = false;
            if (m_EnableTouchInput)
                hasTouches = ProcessTouches();

            if (m_EnableMouseInput && !hasTouches)
                ProcessMouse();

            ProcessNavigation();
        }

        void ProcessMouse()
        {
            if (m_ActiveInputMode != ActiveInputMode.InputManagerBindings)
            {
                if (IsActionEnabled(m_PointAction))
                    m_MouseState.position = m_PointAction.action.ReadValue<Vector2>();
                if (IsActionEnabled(m_ScrollWheelAction))
                    m_MouseState.scrollDelta = m_ScrollWheelAction.action.ReadValue<Vector2>() * (1 / kPixelPerLine); ;
                if (IsActionEnabled(m_LeftClickAction))
                    m_MouseState.leftButtonPressed = m_LeftClickAction.action.IsPressed();
                if (IsActionEnabled(m_RightClickAction))
                    m_MouseState.rightButtonPressed = m_RightClickAction.action.IsPressed();
                if (IsActionEnabled(m_MiddleClickAction))
                    m_MouseState.middleButtonPressed = m_MiddleClickAction.action.IsPressed();
            }

            if (m_ActiveInputMode != ActiveInputMode.InputSystemActions && Input.mousePresent)
            {
                m_MouseState.position = Input.mousePosition;
                m_MouseState.scrollDelta = Input.mouseScrollDelta;
                m_MouseState.leftButtonPressed = Input.GetMouseButton(0);
                m_MouseState.rightButtonPressed = Input.GetMouseButton(1);
                m_MouseState.middleButtonPressed = Input.GetMouseButton(2);
            }

            ProcessMouseState(ref m_MouseState);
        }

        bool ProcessTouches()
        {
            var hasTouches = Input.touchCount > 0;
            if (!hasTouches)
                return false;

            var touchCount = Input.touchCount;
            for (var touchIndex = 0; touchIndex < touchCount; ++touchIndex)
            {
                var touch = Input.GetTouch(touchIndex);
                var registeredTouchIndex = -1;

                // Find if touch already exists
                for (var j = 0; j < m_RegisteredTouches.Count; j++)
                {
                    if (touch.fingerId == m_RegisteredTouches[j].touchId)
                    {
                        registeredTouchIndex = j;
                        break;
                    }
                }

                if (registeredTouchIndex < 0)
                {
                    // Not found, search empty pool
                    for (var j = 0; j < m_RegisteredTouches.Count; j++)
                    {
                        if (!m_RegisteredTouches[j].isValid)
                        {
                            // Re-use the Id
                            var pointerId = m_RegisteredTouches[j].model.pointerId;
                            m_RegisteredTouches[j] = new RegisteredTouch(touch, pointerId);
                            registeredTouchIndex = j;
                            break;
                        }
                    }

                    if (registeredTouchIndex < 0)
                    {
                        // No Empty slots, add one
                        registeredTouchIndex = m_RegisteredTouches.Count;
                        m_RegisteredTouches.Add(new RegisteredTouch(touch, m_RollingPointerId++));
                    }
                }

                var registeredTouch = m_RegisteredTouches[registeredTouchIndex];
                registeredTouch.model.selectPhase = touch.phase;
                registeredTouch.model.position = touch.position;
                m_RegisteredTouches[registeredTouchIndex] = registeredTouch;
            }

            for (var i = 0; i < m_RegisteredTouches.Count; i++)
            {
                var registeredTouch = m_RegisteredTouches[i];
                ProcessTouch(ref registeredTouch.model);
                if (registeredTouch.model.selectPhase == TouchPhase.Ended || registeredTouch.model.selectPhase == TouchPhase.Canceled)
                    registeredTouch.isValid = false;
                m_RegisteredTouches[i] = registeredTouch;
            }

            return true;
        }

        void ProcessNavigation()
        {
            if (m_ActiveInputMode != ActiveInputMode.InputManagerBindings)
            {
                if (IsActionEnabled(m_NavigateAction))
                    m_NavigationState.move = m_NavigateAction.action.ReadValue<Vector2>();
                if (IsActionEnabled(m_SubmitAction))
                    m_NavigationState.submitButtonDown = m_SubmitAction.action.WasPressedThisFrame();
                if (IsActionEnabled(m_CancelAction))
                    m_NavigationState.cancelButtonDown = m_CancelAction.action.WasPressedThisFrame();
            }

            if (m_ActiveInputMode != ActiveInputMode.InputSystemActions && (m_EnableGamepadInput || m_EnableJoystickInput) && Input.GetJoystickNames().Length > 0)
            {
                m_NavigationState.move = new Vector2(Input.GetAxis(m_HorizontalAxis), Input.GetAxis(m_VerticalAxis));
                m_NavigationState.submitButtonDown = Input.GetButton(m_SubmitButton);
                m_NavigationState.cancelButtonDown = Input.GetButton(m_CancelButton);
            }

            base.ProcessNavigationState(ref m_NavigationState);
        }

        void EnableAllActions()
        {
            EnableInputAction(m_PointAction);
            EnableInputAction(m_LeftClickAction);
            EnableInputAction(m_RightClickAction);
            EnableInputAction(m_MiddleClickAction);
            EnableInputAction(m_NavigateAction);
            EnableInputAction(m_SubmitAction);
            EnableInputAction(m_CancelAction);
            EnableInputAction(m_ScrollWheelAction);
        }

        void DisableAllActions()
        {
            DisableInputAction(m_PointAction);
            DisableInputAction(m_LeftClickAction);
            DisableInputAction(m_RightClickAction);
            DisableInputAction(m_MiddleClickAction);
            DisableInputAction(m_NavigateAction);
            DisableInputAction(m_SubmitAction);
            DisableInputAction(m_CancelAction);
            DisableInputAction(m_ScrollWheelAction);
        }

        static bool IsActionEnabled(InputActionReference inputAction)
        {
            return inputAction != null && inputAction.action != null && inputAction.action.enabled;
        }

        static void EnableInputAction(InputActionReference inputAction)
        {
            if (inputAction == null || inputAction.action == null)
                return;
            inputAction.action.Enable();
        }

        static void DisableInputAction(InputActionReference inputAction)
        {
            if (inputAction == null || inputAction.action == null)
                return;
            inputAction.action.Disable();
        }

        void SetInputAction(ref InputActionReference inputAction, InputActionReference value)
        {
            if (Application.isPlaying && inputAction != null)
                inputAction.action?.Disable();

            inputAction = value;

            if (Application.isPlaying && isActiveAndEnabled && inputAction != null)
                inputAction.action?.Enable();
        }
    }
}
