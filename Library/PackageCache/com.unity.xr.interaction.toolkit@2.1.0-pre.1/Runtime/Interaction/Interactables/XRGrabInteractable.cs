using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interactable component that allows basic "grab" functionality.
    /// Can attach to an Interactor that selects this Interactable component and follow it around while obeying physics
    /// (and inherit velocity when released).
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [CanSelectMultiple(false)]
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("XR/XR Grab Interactable", 11)]
    [HelpURL(XRHelpURLConstants.k_XRGrabInteractable)]
    public partial class XRGrabInteractable : XRBaseInteractable
    {
        const float k_DefaultTighteningAmount = 0.5f;
        const float k_DefaultSmoothingAmount = 5f;
        const float k_VelocityDamping = 1f;
        const float k_VelocityScale = 1f;
        const float k_AngularVelocityDamping = 1f;
        const float k_AngularVelocityScale = 1f;
        const int k_ThrowSmoothingFrameCount = 20;
        const float k_DefaultAttachEaseInTime = 0.15f;
        const float k_DefaultThrowSmoothingDuration = 0.25f;
        const float k_DefaultThrowVelocityScale = 1.5f;
        const float k_DefaultThrowAngularVelocityScale = 1f;

        /// <summary>
        /// Controls the method used when calculating the target position of the object.
        /// </summary>
        /// <seealso cref="attachPointCompatibilityMode"/>
        public enum AttachPointCompatibilityMode
        {
            /// <summary>
            /// Use the default, correct method for calculating the target position of the object.
            /// </summary>
            Default,

            /// <summary>
            /// Use an additional offset from the center of mass when calculating the target position of the object.
            /// Also incorporate the scale of the Interactor's Attach Transform.
            /// Marked for deprecation.
            /// This is the backwards compatible support mode for projects that accounted for the
            /// unintended difference when using XR Interaction Toolkit prior to version <c>1.0.0-pre.4</c>.
            /// To have the effective attach position be the same between all <see cref="XRBaseInteractable.MovementType"/> values, use <see cref="Default"/>.
            /// </summary>
            Legacy,
        }

        [SerializeField]
        Transform m_AttachTransform;

        /// <summary>
        /// The attachment point Unity uses on this Interactable (will use this object's position if none set).
        /// </summary>
        public Transform attachTransform
        {
            get => m_AttachTransform;
            set => m_AttachTransform = value;
        }

        [SerializeField]
        bool m_UseDynamicAttach;

        /// <summary>
        /// The grab pose will be based on the pose of the Interactor when the selection is made.
        /// Unity will create a dynamic attachment point for each Interactor that selects this component.
        /// </summary>
        /// <remarks>
        /// A child GameObject will be created for each Interactor that selects this component to serve as the attachment point.
        /// These are cached and part of a shared pool used by all instances of <see cref="XRGrabInteractable"/>.
        /// Therefore, while a reference can be obtained by calling <see cref="GetAttachTransform"/> while selected,
        /// you should typically not add any components to that GameObject unless you remove them after being released
        /// since it won't always be used by the same Interactable.
        /// </remarks>
        /// <seealso cref="attachTransform"/>
        /// <seealso cref="InitializeDynamicAttachPose"/>
        public bool useDynamicAttach
        {
            get => m_UseDynamicAttach;
            set => m_UseDynamicAttach = value;
        }

        [SerializeField]
        bool m_MatchAttachPosition = true;

        /// <summary>
        /// Match the position of the Interactor's attachment point when initializing the grab.
        /// This will override the position of <see cref="attachTransform"/>.
        /// </summary>
        /// <remarks>
        /// This will initialize the dynamic attachment point of this object using the position of the Interactor's attachment point.
        /// </remarks>
        /// <seealso cref="useDynamicAttach"/>
        /// <seealso cref="matchAttachRotation"/>
        public bool matchAttachPosition
        {
            get => m_MatchAttachPosition;
            set => m_MatchAttachPosition = value;
        }

        [SerializeField]
        bool m_MatchAttachRotation = true;

        /// <summary>
        /// Match the rotation of the Interactor's attachment point when initializing the grab.
        /// This will override the rotation of <see cref="attachTransform"/>.
        /// </summary>
        /// <remarks>
        /// This will initialize the dynamic attachment point of this object using the rotation of the Interactor's attachment point.
        /// </remarks>
        /// <seealso cref="useDynamicAttach"/>
        /// <seealso cref="matchAttachPosition"/>
        public bool matchAttachRotation
        {
            get => m_MatchAttachRotation;
            set => m_MatchAttachRotation = value;
        }

        [SerializeField]
        bool m_SnapToColliderVolume = true;

        /// <summary>
        /// Adjust the dynamic attachment point to keep it on or inside the Colliders that make up this object.
        /// </summary>
        /// <seealso cref="useDynamicAttach"/>
        /// <seealso cref="Collider.ClosestPoint"/>
        public bool snapToColliderVolume
        {
            get => m_SnapToColliderVolume;
            set => m_SnapToColliderVolume = value;
        }

        [SerializeField]
        float m_AttachEaseInTime = k_DefaultAttachEaseInTime;

        /// <summary>
        /// Time in seconds Unity eases in the attach when selected (a value of 0 indicates no easing).
        /// </summary>
        public float attachEaseInTime
        {
            get => m_AttachEaseInTime;
            set => m_AttachEaseInTime = value;
        }

        [SerializeField]
        MovementType m_MovementType = MovementType.Instantaneous;

        /// <summary>
        /// Specifies how this object moves when selected, either through setting the velocity of the <see cref="Rigidbody"/>,
        /// moving the kinematic <see cref="Rigidbody"/> during Fixed Update, or by directly updating the <see cref="Transform"/> each frame.
        /// </summary>
        /// <seealso cref="XRBaseInteractable.MovementType"/>
        public MovementType movementType
        {
            get => m_MovementType;
            set
            {
                m_MovementType = value;

                if (isSelected)
                {
                    SetupRigidbodyDrop(m_Rigidbody);
                    UpdateCurrentMovementType();
                    SetupRigidbodyGrab(m_Rigidbody);
                }
            }
        }

        [SerializeField, Range(0f, 1f)]
        float m_VelocityDamping = k_VelocityDamping;

        /// <summary>
        /// Scale factor of how much to dampen the existing velocity when tracking the position of the Interactor.
        /// The smaller the value, the longer it takes for the velocity to decay.
        /// </summary>
        /// <remarks>
        /// Only applies when in <see cref="XRBaseInteractable.MovementType.VelocityTracking"/> mode.
        /// </remarks>
        /// <seealso cref="XRBaseInteractable.MovementType.VelocityTracking"/>
        /// <seealso cref="trackPosition"/>
        public float velocityDamping
        {
            get => m_VelocityDamping;
            set => m_VelocityDamping = value;
        }

        [SerializeField]
        float m_VelocityScale = k_VelocityScale;

        /// <summary>
        /// Scale factor Unity applies to the tracked velocity while updating the <see cref="Rigidbody"/>
        /// when tracking the position of the Interactor.
        /// </summary>
        /// <remarks>
        /// Only applies when in <see cref="XRBaseInteractable.MovementType.VelocityTracking"/> mode.
        /// </remarks>
        /// <seealso cref="XRBaseInteractable.MovementType.VelocityTracking"/>
        /// <seealso cref="trackPosition"/>
        public float velocityScale
        {
            get => m_VelocityScale;
            set => m_VelocityScale = value;
        }

        [SerializeField, Range(0f, 1f)]
        float m_AngularVelocityDamping = k_AngularVelocityDamping;

        /// <summary>
        /// Scale factor of how much Unity dampens the existing angular velocity when tracking the rotation of the Interactor.
        /// The smaller the value, the longer it takes for the angular velocity to decay.
        /// </summary>
        /// <remarks>
        /// Only applies when in <see cref="XRBaseInteractable.MovementType.VelocityTracking"/> mode.
        /// </remarks>
        /// <seealso cref="XRBaseInteractable.MovementType.VelocityTracking"/>
        /// <seealso cref="trackRotation"/>
        public float angularVelocityDamping
        {
            get => m_AngularVelocityDamping;
            set => m_AngularVelocityDamping = value;
        }

        [SerializeField]
        float m_AngularVelocityScale = k_AngularVelocityScale;

        /// <summary>
        /// Scale factor Unity applies to the tracked angular velocity while updating the <see cref="Rigidbody"/>
        /// when tracking the rotation of the Interactor.
        /// </summary>
        /// <remarks>
        /// Only applies when in <see cref="XRBaseInteractable.MovementType.VelocityTracking"/> mode.
        /// </remarks>
        /// <seealso cref="XRBaseInteractable.MovementType.VelocityTracking"/>
        /// <seealso cref="trackRotation"/>
        public float angularVelocityScale
        {
            get => m_AngularVelocityScale;
            set => m_AngularVelocityScale = value;
        }

        [SerializeField]
        bool m_TrackPosition = true;

        /// <summary>
        /// Whether this object should follow the position of the Interactor when selected.
        /// </summary>
        /// <seealso cref="trackRotation"/>
        public bool trackPosition
        {
            get => m_TrackPosition;
            set => m_TrackPosition = value;
        }

        [SerializeField]
        bool m_SmoothPosition;

        /// <summary>
        /// Whether Unity applies smoothing while following the position of the Interactor when selected.
        /// </summary>
        /// <seealso cref="smoothPositionAmount"/>
        /// <seealso cref="tightenPosition"/>
        public bool smoothPosition
        {
            get => m_SmoothPosition;
            set => m_SmoothPosition = value;
        }

        [SerializeField, Range(0f, 20f)]
        float m_SmoothPositionAmount = k_DefaultSmoothingAmount;

        /// <summary>
        /// Scale factor for how much smoothing is applied while following the position of the Interactor when selected.
        /// The larger the value, the closer this object will remain to the position of the Interactor.
        /// </summary>
        /// <seealso cref="smoothPosition"/>
        /// <seealso cref="tightenPosition"/>
        public float smoothPositionAmount
        {
            get => m_SmoothPositionAmount;
            set => m_SmoothPositionAmount = value;
        }

        [SerializeField, Range(0f, 1f)]
        float m_TightenPosition = k_DefaultTighteningAmount;

        /// <summary>
        /// Reduces the maximum follow position difference when using smoothing.
        /// </summary>
        /// <remarks>
        /// Fractional amount of how close the smoothed position should remain to the position of the Interactor when using smoothing.
        /// The value ranges from 0 meaning no bias in the smoothed follow distance,
        /// to 1 meaning effectively no smoothing at all.
        /// </remarks>
        /// <seealso cref="smoothPosition"/>
        /// <seealso cref="smoothPositionAmount"/>
        public float tightenPosition
        {
            get => m_TightenPosition;
            set => m_TightenPosition = value;
        }

        [SerializeField]
        bool m_TrackRotation = true;

        /// <summary>
        /// Whether this object should follow the rotation of the Interactor when selected.
        /// </summary>
        /// <seealso cref="trackPosition"/>
        public bool trackRotation
        {
            get => m_TrackRotation;
            set => m_TrackRotation = value;
        }

        [SerializeField]
        bool m_SmoothRotation;

        /// <summary>
        /// Apply smoothing while following the rotation of the Interactor when selected.
        /// </summary>
        /// <seealso cref="smoothRotationAmount"/>
        /// <seealso cref="tightenRotation"/>
        public bool smoothRotation
        {
            get => m_SmoothRotation;
            set => m_SmoothRotation = value;
        }

        [SerializeField, Range(0f, 20f)]
        float m_SmoothRotationAmount = k_DefaultSmoothingAmount;

        /// <summary>
        /// Scale factor for how much smoothing is applied while following the rotation of the Interactor when selected.
        /// The larger the value, the closer this object will remain to the rotation of the Interactor.
        /// </summary>
        /// <seealso cref="smoothRotation"/>
        /// <seealso cref="tightenRotation"/>
        public float smoothRotationAmount
        {
            get => m_SmoothRotationAmount;
            set => m_SmoothRotationAmount = value;
        }

        [SerializeField, Range(0f, 1f)]
        float m_TightenRotation = k_DefaultTighteningAmount;

        /// <summary>
        /// Reduces the maximum follow rotation difference when using smoothing.
        /// </summary>
        /// <remarks>
        /// Fractional amount of how close the smoothed rotation should remain to the rotation of the Interactor when using smoothing.
        /// The value ranges from 0 meaning no bias in the smoothed follow rotation,
        /// to 1 meaning effectively no smoothing at all.
        /// </remarks>
        /// <seealso cref="smoothRotation"/>
        /// <seealso cref="smoothRotationAmount"/>
        public float tightenRotation
        {
            get => m_TightenRotation;
            set => m_TightenRotation = value;
        }

        [SerializeField]
        bool m_ThrowOnDetach = true;

        /// <summary>
        /// Whether this object inherits the velocity of the Interactor when released.
        /// </summary>
        public bool throwOnDetach
        {
            get => m_ThrowOnDetach;
            set => m_ThrowOnDetach = value;
        }

        [SerializeField]
        float m_ThrowSmoothingDuration = k_DefaultThrowSmoothingDuration;

        /// <summary>
        /// Time period to average thrown velocity over.
        /// </summary>
        /// <seealso cref="throwOnDetach"/>
        public float throwSmoothingDuration
        {
            get => m_ThrowSmoothingDuration;
            set => m_ThrowSmoothingDuration = value;
        }

        [SerializeField]
        AnimationCurve m_ThrowSmoothingCurve = AnimationCurve.Linear(1f, 1f, 1f, 0f);

        /// <summary>
        /// The curve to use to weight thrown velocity smoothing (most recent frames to the right).
        /// </summary>
        /// <seealso cref="throwOnDetach"/>
        public AnimationCurve throwSmoothingCurve
        {
            get => m_ThrowSmoothingCurve;
            set => m_ThrowSmoothingCurve = value;
        }

        [SerializeField]
        float m_ThrowVelocityScale = k_DefaultThrowVelocityScale;

        /// <summary>
        /// Scale factor Unity applies to this object's velocity inherited from the Interactor when released.
        /// </summary>
        /// <seealso cref="throwOnDetach"/>
        public float throwVelocityScale
        {
            get => m_ThrowVelocityScale;
            set => m_ThrowVelocityScale = value;
        }

        [SerializeField]
        float m_ThrowAngularVelocityScale = k_DefaultThrowAngularVelocityScale;

        /// <summary>
        /// Scale factor Unity applies to this object's angular velocity inherited from the Interactor when released.
        /// </summary>
        /// <seealso cref="throwOnDetach"/>
        public float throwAngularVelocityScale
        {
            get => m_ThrowAngularVelocityScale;
            set => m_ThrowAngularVelocityScale = value;
        }

        [SerializeField, FormerlySerializedAs("m_GravityOnDetach")]
        bool m_ForceGravityOnDetach;

        /// <summary>
        /// Forces this object to have gravity when released
        /// (will still use pre-grab value if this is <see langword="false"/>).
        /// </summary>
        public bool forceGravityOnDetach
        {
            get => m_ForceGravityOnDetach;
            set => m_ForceGravityOnDetach = value;
        }

        [SerializeField]
        bool m_RetainTransformParent = true;

        /// <summary>
        /// Whether Unity sets the parent of this object back to its original parent this object was a child of after this object is dropped.
        /// </summary>
        public bool retainTransformParent
        {
            get => m_RetainTransformParent;
            set => m_RetainTransformParent = value;
        }

        [SerializeField]
        AttachPointCompatibilityMode m_AttachPointCompatibilityMode = AttachPointCompatibilityMode.Default;

        /// <summary>
        /// Controls the method used when calculating the target position of the object.
        /// Use <see cref="AttachPointCompatibilityMode.Default"/> for consistent attach points
        /// between all <see cref="XRBaseInteractable.MovementType"/> values.
        /// Marked for deprecation, this property will be removed in a future version.
        /// </summary>
        /// <remarks>
        /// This is a backwards compatibility option in order to keep the old, incorrect method
        /// of calculating the attach point. Projects that already accounted for the difference
        /// can use the Legacy option to maintain the same attach positioning from older versions
        /// without needing to modify the Attach Transform position.
        /// </remarks>
        /// <seealso cref="AttachPointCompatibilityMode"/>
        public AttachPointCompatibilityMode attachPointCompatibilityMode
        {
            get => m_AttachPointCompatibilityMode;
            set => m_AttachPointCompatibilityMode = value;
        }

        // Point we are attaching to on this Interactable (in Interactor's attach coordinate space)
        Vector3 m_InteractorLocalPosition;
        Quaternion m_InteractorLocalRotation;

        // Point we are moving towards each frame (eventually will be at Interactor's attach point)
        Vector3 m_TargetWorldPosition;
        Quaternion m_TargetWorldRotation;

        float m_CurrentAttachEaseTime;
        MovementType m_CurrentMovementType;

        bool m_DetachInLateUpdate;
        Vector3 m_DetachVelocity;
        Vector3 m_DetachAngularVelocity;

        int m_ThrowSmoothingCurrentFrame;
        readonly float[] m_ThrowSmoothingFrameTimes = new float[k_ThrowSmoothingFrameCount];
        readonly Vector3[] m_ThrowSmoothingVelocityFrames = new Vector3[k_ThrowSmoothingFrameCount];
        readonly Vector3[] m_ThrowSmoothingAngularVelocityFrames = new Vector3[k_ThrowSmoothingFrameCount];

        Rigidbody m_Rigidbody;
        Vector3 m_LastPosition;
        Quaternion m_LastRotation;

        // Rigidbody's settings upon select, kept to restore these values when dropped
        bool m_WasKinematic;
        bool m_UsedGravity;
        float m_OldDrag;
        float m_OldAngularDrag;

        Transform m_OriginalSceneParent;

        // Account for teleportation to avoid throws with unintentionally high energy
        TeleportationProvider m_TeleportationProvider;
        Pose m_PoseBeforeTeleport;

        readonly Dictionary<IXRSelectInteractor, Transform> m_DynamicAttachTransforms = new Dictionary<IXRSelectInteractor, Transform>();

        static readonly LinkedPool<Transform> s_DynamicAttachTransformPool = new LinkedPool<Transform>(OnCreatePooledItem, OnGetPooledItem, OnReleasePooledItem, OnDestroyPooledItem);

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            m_CurrentMovementType = m_MovementType;
            m_Rigidbody = GetComponent<Rigidbody>();
            if (m_Rigidbody == null)
                Debug.LogError("Grab Interactable does not have a required Rigidbody.", this);
        }

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            switch (updatePhase)
            {
                // During Fixed update we want to perform any physics-based updates (e.g., Kinematic or VelocityTracking).
                case XRInteractionUpdateOrder.UpdatePhase.Fixed:
                    if (isSelected)
                    {
                        if (m_CurrentMovementType == MovementType.Kinematic)
                            PerformKinematicUpdate(updatePhase);
                        else if (m_CurrentMovementType == MovementType.VelocityTracking)
                            PerformVelocityTrackingUpdate(Time.deltaTime, updatePhase);
                    }

                    break;

                // During Dynamic update we want to perform any Transform-based manipulation (e.g., Instantaneous).
                case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                    if (isSelected)
                    {
                        var interactor = interactorsSelecting[0];
                        // Legacy does not support the Attach Transform position changing after being grabbed
                        // due to depending on the Physics update to compute the world center of mass.
                        if (m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default)
                            UpdateInteractorLocalPose(interactor);
                        UpdateTarget(interactor, Time.deltaTime);
                        SmoothVelocityUpdate(interactor);

                        if (m_CurrentMovementType == MovementType.Instantaneous)
                            PerformInstantaneousUpdate(updatePhase);
                    }

                    break;

                // During OnBeforeRender we want to perform any last minute Transform position changes before rendering (e.g., Instantaneous).
                case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:
                    if (isSelected)
                    {
                        var interactor = interactorsSelecting[0];
                        // Legacy does not support the Attach Transform position changing after being grabbed
                        // due to depending on the Physics update to compute the world center of mass.
                        if (m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default)
                            UpdateInteractorLocalPose(interactor);
                        UpdateTarget(interactor, Time.deltaTime);

                        if (m_CurrentMovementType == MovementType.Instantaneous)
                            PerformInstantaneousUpdate(updatePhase);
                    }

                    break;

                // Late update is only used to handle detach as late as possible.
                case XRInteractionUpdateOrder.UpdatePhase.Late:
                    if (m_DetachInLateUpdate)
                    {
                        if (!isSelected)
                            Detach();
                        m_DetachInLateUpdate = false;
                    }

                    break;
            }
        }

        /// <inheritdoc />
        public override Transform GetAttachTransform(IXRInteractor interactor)
        {
            if (m_UseDynamicAttach && interactor is IXRSelectInteractor selectInteractor &&
                m_DynamicAttachTransforms.TryGetValue(selectInteractor, out var dynamicAttachTransform))
            {
                if (dynamicAttachTransform != null)
                    return dynamicAttachTransform;

                m_DynamicAttachTransforms.Remove(selectInteractor);
                Debug.LogWarning($"Dynamic Attach Transform created by {this} for {interactor} was destroyed after being created." +
                    " Continuing as if Use Dynamic Attach was disabled for this pair.", this);
            }

            return m_AttachTransform != null ? m_AttachTransform : base.GetAttachTransform(interactor);
        }

        void UpdateTarget(IXRInteractor interactor, float timeDelta)
        {
            // Compute the unsmoothed target world pose
            Vector3 rawTargetWorldPosition;
            Quaternion rawTargetWorldRotation;

            var interactorAttachTransform = interactor.GetAttachTransform(this);
            var interactorAttachPose = new Pose(interactorAttachTransform.position, interactorAttachTransform.rotation);
            if (m_TrackRotation)
            {
                rawTargetWorldPosition = interactorAttachPose.rotation * m_InteractorLocalPosition + interactorAttachPose.position;
                rawTargetWorldRotation = interactorAttachPose.rotation * m_InteractorLocalRotation;
            }
            else
            {
                var thisAttachTransform = GetAttachTransform(interactor);
                rawTargetWorldPosition = thisAttachTransform.TransformDirection(m_InteractorLocalPosition) + interactorAttachPose.position;
                rawTargetWorldRotation = m_TargetWorldRotation;
            }

            // Apply smoothing (if configured)
            if (m_AttachEaseInTime > 0f && m_CurrentAttachEaseTime <= m_AttachEaseInTime)
            {
                var easePercent = m_CurrentAttachEaseTime / m_AttachEaseInTime;
                m_TargetWorldPosition = Vector3.Lerp(m_TargetWorldPosition, rawTargetWorldPosition, easePercent);
                m_TargetWorldRotation = Quaternion.Slerp(m_TargetWorldRotation, rawTargetWorldRotation, easePercent);
                m_CurrentAttachEaseTime += timeDelta;
            }
            else
            {
                if (m_SmoothPosition)
                {
                    m_TargetWorldPosition = Vector3.Lerp(m_TargetWorldPosition, rawTargetWorldPosition, m_SmoothPositionAmount * timeDelta);
                    m_TargetWorldPosition = Vector3.Lerp(m_TargetWorldPosition, rawTargetWorldPosition, m_TightenPosition);
                }
                else
                {
                    m_TargetWorldPosition = rawTargetWorldPosition;
                }

                if (m_SmoothRotation)
                {
                    m_TargetWorldRotation = Quaternion.Slerp(m_TargetWorldRotation, rawTargetWorldRotation, m_SmoothRotationAmount * timeDelta);
                    m_TargetWorldRotation = Quaternion.Slerp(m_TargetWorldRotation, rawTargetWorldRotation, m_TightenRotation);
                }
                else
                {
                    m_TargetWorldRotation = rawTargetWorldRotation;
                }
            }
        }

        void PerformInstantaneousUpdate(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic ||
                updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
            {
                if (m_TrackPosition)
                {
                    transform.position = m_TargetWorldPosition;
                }

                if (m_TrackRotation)
                {
                    transform.rotation = m_TargetWorldRotation;
                }
            }
        }

        void PerformKinematicUpdate(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                if (m_TrackPosition)
                {
                    var position = m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default
                        ? m_TargetWorldPosition
                        : m_TargetWorldPosition - m_Rigidbody.worldCenterOfMass + m_Rigidbody.position;
                    m_Rigidbody.velocity = Vector3.zero;
                    m_Rigidbody.MovePosition(position);
                }

                if (m_TrackRotation)
                {
                    m_Rigidbody.angularVelocity = Vector3.zero;
                    m_Rigidbody.MoveRotation(m_TargetWorldRotation);
                }
            }
        }

        void PerformVelocityTrackingUpdate(float timeDelta, XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                // Do velocity tracking
                if (m_TrackPosition)
                {
                    // Scale initialized velocity by prediction factor
                    m_Rigidbody.velocity *= (1f - m_VelocityDamping);
                    var positionDelta = m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default
                        ? m_TargetWorldPosition - transform.position
                        : m_TargetWorldPosition - m_Rigidbody.worldCenterOfMass;
                    var velocity = positionDelta / timeDelta;

                    if (!float.IsNaN(velocity.x))
                        m_Rigidbody.velocity += (velocity * m_VelocityScale);
                }

                // Do angular velocity tracking
                if (m_TrackRotation)
                {
                    // Scale initialized velocity by prediction factor
                    m_Rigidbody.angularVelocity *= (1f - m_AngularVelocityDamping);
                    var rotationDelta = m_TargetWorldRotation * Quaternion.Inverse(transform.rotation);
                    rotationDelta.ToAngleAxis(out var angleInDegrees, out var rotationAxis);
                    if (angleInDegrees > 180f)
                        angleInDegrees -= 360f;

                    if (Mathf.Abs(angleInDegrees) > Mathf.Epsilon)
                    {
                        var angularVelocity = (rotationAxis * (angleInDegrees * Mathf.Deg2Rad)) / timeDelta;
                        if (!float.IsNaN(angularVelocity.x))
                            m_Rigidbody.angularVelocity += (angularVelocity * m_AngularVelocityScale);
                    }
                }
            }
        }

        void UpdateInteractorLocalPose(IXRInteractor interactor)
        {
            if (m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Legacy)
            {
                UpdateInteractorLocalPoseLegacy(interactor);
                return;
            }

            // In order to move the Interactable to the Interactor we need to
            // calculate the Interactable attach point in the coordinate system of the
            // Interactor's attach point.
            var thisAttachTransform = GetAttachTransform(interactor);
            var attachOffset = transform.position - thisAttachTransform.position;
            var localAttachOffset = thisAttachTransform.InverseTransformDirection(attachOffset);

            m_InteractorLocalPosition = localAttachOffset;
            m_InteractorLocalRotation = Quaternion.Inverse(Quaternion.Inverse(transform.rotation) * thisAttachTransform.rotation);
        }

        void UpdateInteractorLocalPoseLegacy(IXRInteractor interactor)
        {
            // In order to move the Interactable to the Interactor we need to
            // calculate the Interactable attach point in the coordinate system of the
            // Interactor's attach point.
            var thisAttachTransform = GetAttachTransform(interactor);
            var attachOffset = m_Rigidbody.worldCenterOfMass - thisAttachTransform.position;
            var localAttachOffset = thisAttachTransform.InverseTransformDirection(attachOffset);

            var inverseLocalScale = interactor.GetAttachTransform(this).lossyScale;
            inverseLocalScale = new Vector3(1f / inverseLocalScale.x, 1f / inverseLocalScale.y, 1f / inverseLocalScale.z);
            localAttachOffset.Scale(inverseLocalScale);

            m_InteractorLocalPosition = localAttachOffset;
            m_InteractorLocalRotation = Quaternion.Inverse(Quaternion.Inverse(transform.rotation) * thisAttachTransform.rotation);
        }

        void UpdateCurrentMovementType()
        {
            // Special case where the interactor will override this objects movement type (used for Sockets and other absolute interactors)
            var interactor = interactorsSelecting[0];
            var baseInteractor = interactor as XRBaseInteractor;
            m_CurrentMovementType = (baseInteractor != null ? baseInteractor.selectedInteractableMovementTypeOverride : null) ?? m_MovementType;
        }

        /// <inheritdoc />
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            // Setup the dynamic attach transform.
            // Done before calling the base method so the attach pose captured is the dynamic one.
            if (m_UseDynamicAttach)
            {
                var dynamicAttachTransform = CreateDynamicAttach(args.interactorObject);
                InitializeDynamicAttachPose(args.interactorObject, dynamicAttachTransform);
            }

            base.OnSelectEntering(args);
            Grab();
        }

        /// <inheritdoc />
        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);
            Drop();
        }

        /// <inheritdoc />
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            // Skip checking m_UseDynamicAttach since it may have changed after being grabbed,
            // and we should ensure it is released. We instead check Count first as a faster way to avoid hashing
            // and the Dictionary lookup, which should handle when it was never enabled in the first place.
            if (m_DynamicAttachTransforms.Count > 0 && m_DynamicAttachTransforms.TryGetValue(args.interactorObject, out var dynamicAttachTransform))
            {
                if (dynamicAttachTransform != null)
                    s_DynamicAttachTransformPool.Release(dynamicAttachTransform);

                m_DynamicAttachTransforms.Remove(args.interactorObject);
            }
        }

        Transform CreateDynamicAttach(IXRSelectInteractor interactor)
        {
            Transform dynamicAttachTransform;

            do
            {
                dynamicAttachTransform = s_DynamicAttachTransformPool.Get();
            } while (dynamicAttachTransform == null);

            m_DynamicAttachTransforms.Remove(interactor);
            var staticAttachTransform = GetAttachTransform(interactor);
            m_DynamicAttachTransforms[interactor] = dynamicAttachTransform;

#if UNITY_EDITOR
            dynamicAttachTransform.name = $"[{interactor.transform.name}] Dynamic Attach";
#endif
            dynamicAttachTransform.SetParent(transform, false);

            // Base the initial pose on the Attach Transform.
            // Technically we could just do the final else statement, but setting the local position and rotation this way
            // keeps the position and rotation seen in the Inspector tidier by exactly matching instead of potentially having small
            // floating point offsets.
            if (staticAttachTransform == transform)
            {
                dynamicAttachTransform.localPosition = Vector3.zero;
                dynamicAttachTransform.localRotation = Quaternion.identity;
            }
            else if (staticAttachTransform.parent == transform)
            {
                dynamicAttachTransform.localPosition = staticAttachTransform.localPosition;
                dynamicAttachTransform.localRotation = staticAttachTransform.localRotation;
            }
            else
            {
                dynamicAttachTransform.SetPositionAndRotation(staticAttachTransform.position, staticAttachTransform.rotation);
            }

            return dynamicAttachTransform;
        }

        /// <summary>
        /// Unity calls this method automatically when the Interactor first initiates selection of this Interactable.
        /// Override this method to set the pose of the dynamic attachment point. Before this method is called, the transform
        /// is already set as a child GameObject with inherited Transform values.
        /// </summary>
        /// <param name="interactor">The Interactor that is initiating the selection.</param>
        /// <param name="dynamicAttachTransform">The dynamic attachment Transform that serves as the attachment point for the given Interactor.</param>
        /// <remarks>
        /// This method is only called when <see cref="useDynamicAttach"/> is enabled.
        /// </remarks>
        /// <seealso cref="useDynamicAttach"/>
        protected virtual void InitializeDynamicAttachPose(IXRSelectInteractor interactor, Transform dynamicAttachTransform)
        {
            // We assume the static pose should be used for sockets
            // and for Ray Interactors that bring the object to hand.
            // In both these cases, this object should move to the interactor.
            if (interactor is XRSocketInteractor ||
                interactor is XRRayInteractor rayInteractor && rayInteractor.useForceGrab)
                return;

            // Copy the pose of the interactor's attach transform
            var interactorAttachTransform = interactor.GetAttachTransform(this);
            var position = interactorAttachTransform.position;
            var rotation = interactorAttachTransform.rotation;

            // Optionally constrain the position to within the Collider(s) of this Interactable
            if (m_SnapToColliderVolume && m_MatchAttachPosition &&
                XRInteractableUtility.TryGetClosestPointOnCollider(this, position, out var distanceInfo))
            {
                position = distanceInfo.point;
            }

            if (m_MatchAttachPosition && m_MatchAttachRotation)
                dynamicAttachTransform.SetPositionAndRotation(position, rotation);
            else if (m_MatchAttachPosition)
                dynamicAttachTransform.position = position;
            else if (m_MatchAttachRotation)
                dynamicAttachTransform.rotation = rotation;
        }

        /// <summary>
        /// Updates the state of the object due to being grabbed.
        /// Automatically called when entering the Select state.
        /// </summary>
        /// <seealso cref="Drop"/>
        protected virtual void Grab()
        {
            var thisTransform = transform;
            m_OriginalSceneParent = thisTransform.parent;
            thisTransform.SetParent(null);
            
            UpdateCurrentMovementType();
            SetupRigidbodyGrab(m_Rigidbody);

            // Reset detach velocities
            m_DetachVelocity = Vector3.zero;
            m_DetachAngularVelocity = Vector3.zero;

            // Initialize target pose for easing and smoothing
            m_TargetWorldPosition = m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default ? thisTransform.position : m_Rigidbody.worldCenterOfMass;
            m_TargetWorldRotation = thisTransform.rotation;
            m_CurrentAttachEaseTime = 0f;

            var interactor = interactorsSelecting[0];
            UpdateInteractorLocalPose(interactor);

            SmoothVelocityStart(interactor);
        }

        /// <summary>
        /// Updates the state of the object due to being dropped and schedule to finish the detach during the end of the frame.
        /// Automatically called when exiting the Select state.
        /// </summary>
        /// <seealso cref="Detach"/>
        /// <seealso cref="Grab"/>
        protected virtual void Drop()
        {
            if (m_RetainTransformParent && m_OriginalSceneParent != null && !m_OriginalSceneParent.gameObject.activeInHierarchy)
            {
#if UNITY_EDITOR
                // Suppress the warning when exiting Play mode to avoid confusing the user
                var exitingPlayMode = UnityEditor.EditorApplication.isPlaying && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
                var exitingPlayMode = false;
#endif
                if (!exitingPlayMode)
                    Debug.LogWarning("Retain Transform Parent is set to true, and has a non-null Original Scene Parent. " +
                        "However, the old parent is deactivated so we are choosing not to re-parent upon dropping.", this);
            }
            else if (m_RetainTransformParent && gameObject.activeInHierarchy)
                transform.SetParent(m_OriginalSceneParent);

            SetupRigidbodyDrop(m_Rigidbody);

            m_CurrentMovementType = m_MovementType;
            m_DetachInLateUpdate = true;
            SmoothVelocityEnd();
        }

        /// <summary>
        /// Updates the state of the object to finish the detach after being dropped.
        /// Automatically called during the end of the frame after being dropped.
        /// </summary>
        /// <remarks>
        /// This method updates the velocity of the Rigidbody if configured to do so.
        /// </remarks>
        /// <seealso cref="Drop"/>
        protected virtual void Detach()
        {
            if (m_ThrowOnDetach)
            {
                m_Rigidbody.velocity = m_DetachVelocity;
                m_Rigidbody.angularVelocity = m_DetachAngularVelocity;
            }
        }

        /// <summary>
        /// Setup the <see cref="Rigidbody"/> on this object due to being grabbed.
        /// Automatically called when entering the Select state.
        /// </summary>
        /// <param name="rigidbody">The <see cref="Rigidbody"/> on this object.</param>
        /// <seealso cref="SetupRigidbodyDrop"/>
        // ReSharper disable once ParameterHidesMember
        protected virtual void SetupRigidbodyGrab(Rigidbody rigidbody)
        {
            // Remember Rigidbody settings and setup to move
            m_WasKinematic = rigidbody.isKinematic;
            m_UsedGravity = rigidbody.useGravity;
            m_OldDrag = rigidbody.drag;
            m_OldAngularDrag = rigidbody.angularDrag;
            rigidbody.isKinematic = m_CurrentMovementType == MovementType.Kinematic || m_CurrentMovementType == MovementType.Instantaneous;
            rigidbody.useGravity = false;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
        }

        /// <summary>
        /// Setup the <see cref="Rigidbody"/> on this object due to being dropped.
        /// Automatically called when exiting the Select state.
        /// </summary>
        /// <param name="rigidbody">The <see cref="Rigidbody"/> on this object.</param>
        /// <seealso cref="SetupRigidbodyGrab"/>
        // ReSharper disable once ParameterHidesMember
        protected virtual void SetupRigidbodyDrop(Rigidbody rigidbody)
        {
            // Restore Rigidbody settings
            rigidbody.isKinematic = m_WasKinematic;
            rigidbody.useGravity = m_UsedGravity;
            rigidbody.drag = m_OldDrag;
            rigidbody.angularDrag = m_OldAngularDrag;
            
            if (!isSelected)
                m_Rigidbody.useGravity |= m_ForceGravityOnDetach;
        }

        void SmoothVelocityStart(IXRInteractor interactor)
        {
            SetTeleportationProvider(interactor);

            var interactorAttachTransform = interactor.GetAttachTransform(this);
            m_LastPosition = interactorAttachTransform.position;
            m_LastRotation = interactorAttachTransform.rotation;
            Array.Clear(m_ThrowSmoothingFrameTimes, 0, m_ThrowSmoothingFrameTimes.Length);
            Array.Clear(m_ThrowSmoothingVelocityFrames, 0, m_ThrowSmoothingVelocityFrames.Length);
            Array.Clear(m_ThrowSmoothingAngularVelocityFrames, 0, m_ThrowSmoothingAngularVelocityFrames.Length);
            m_ThrowSmoothingCurrentFrame = 0;
        }

        void SmoothVelocityEnd()
        {
            if (m_ThrowOnDetach)
            {
                var smoothedVelocity = GetSmoothedVelocityValue(m_ThrowSmoothingVelocityFrames);
                var smoothedAngularVelocity = GetSmoothedVelocityValue(m_ThrowSmoothingAngularVelocityFrames);
                m_DetachVelocity = smoothedVelocity * m_ThrowVelocityScale;
                m_DetachAngularVelocity = smoothedAngularVelocity * m_ThrowAngularVelocityScale;
            }

            ClearTeleportationProvider();
        }

        void SmoothVelocityUpdate(IXRInteractor interactor)
        {
            var interactorAttachTransform = interactor.GetAttachTransform(this);
            var interactorPosition = interactorAttachTransform.position;
            var interactorRotation = interactorAttachTransform.rotation;
            m_ThrowSmoothingFrameTimes[m_ThrowSmoothingCurrentFrame] = Time.time;
            m_ThrowSmoothingVelocityFrames[m_ThrowSmoothingCurrentFrame] = (interactorPosition - m_LastPosition) / Time.deltaTime;

            var velocityDiff = (interactorRotation * Quaternion.Inverse(m_LastRotation));
            m_ThrowSmoothingAngularVelocityFrames[m_ThrowSmoothingCurrentFrame] =
                (new Vector3(Mathf.DeltaAngle(0f, velocityDiff.eulerAngles.x),
                        Mathf.DeltaAngle(0f, velocityDiff.eulerAngles.y),
                        Mathf.DeltaAngle(0f, velocityDiff.eulerAngles.z))
                    / Time.deltaTime) * Mathf.Deg2Rad;

            m_ThrowSmoothingCurrentFrame = (m_ThrowSmoothingCurrentFrame + 1) % k_ThrowSmoothingFrameCount;
            m_LastPosition = interactorPosition;
            m_LastRotation = interactorRotation;
        }

        Vector3 GetSmoothedVelocityValue(Vector3[] velocityFrames)
        {
            var calcVelocity = new Vector3();
            var totalWeights = 0f;
            for (var frameCounter = 0; frameCounter < k_ThrowSmoothingFrameCount; ++frameCounter)
            {
                var frameIdx = (((m_ThrowSmoothingCurrentFrame - frameCounter - 1) % k_ThrowSmoothingFrameCount) + k_ThrowSmoothingFrameCount) % k_ThrowSmoothingFrameCount;
                if (m_ThrowSmoothingFrameTimes[frameIdx] == 0f)
                    break;

                var timeAlpha = (Time.time - m_ThrowSmoothingFrameTimes[frameIdx]) / m_ThrowSmoothingDuration;
                var velocityWeight = m_ThrowSmoothingCurve.Evaluate(Mathf.Clamp(1f - timeAlpha, 0f, 1f));
                calcVelocity += velocityFrames[frameIdx] * velocityWeight;
                totalWeights += velocityWeight;
                if (Time.time - m_ThrowSmoothingFrameTimes[frameIdx] > m_ThrowSmoothingDuration)
                    break;
            }

            if (totalWeights > 0f)
                return calcVelocity / totalWeights;

            return Vector3.zero;
        }

        void OnBeginTeleportation(LocomotionSystem locomotionSystem)
        {
            var originTransform = locomotionSystem.xrOrigin.Origin.transform;
            m_PoseBeforeTeleport = new Pose(originTransform.position, originTransform.rotation);
        }

        void OnEndTeleportation(LocomotionSystem locomotionSystem)
        {
            var originTransform = locomotionSystem.xrOrigin.Origin.transform;
            var translated = originTransform.position - m_PoseBeforeTeleport.position;
            var rotated = originTransform.rotation * Quaternion.Inverse(m_PoseBeforeTeleport.rotation);

            for (var frameIdx = 0; frameIdx < k_ThrowSmoothingFrameCount; ++frameIdx)
            {
                if (m_ThrowSmoothingFrameTimes[frameIdx] == 0f)
                    break;

                m_ThrowSmoothingVelocityFrames[frameIdx] = rotated * m_ThrowSmoothingVelocityFrames[frameIdx];
            }

            m_LastPosition += translated;
            m_LastRotation = rotated * m_LastRotation;
        }

        void SetTeleportationProvider(IXRInteractor interactor)
        {
            ClearTeleportationProvider();
            var interactorTransform = interactor?.transform;
            if (interactorTransform == null)
                return;

            m_TeleportationProvider = interactorTransform.GetComponentInParent<TeleportationProvider>();
            if (m_TeleportationProvider == null)
                return;

            m_TeleportationProvider.beginLocomotion += OnBeginTeleportation;
            m_TeleportationProvider.endLocomotion += OnEndTeleportation;
        }

        void ClearTeleportationProvider()
        {
            if (m_TeleportationProvider == null)
                return;

            m_TeleportationProvider.beginLocomotion -= OnBeginTeleportation;
            m_TeleportationProvider.endLocomotion -= OnEndTeleportation;
            m_TeleportationProvider = null;
        }

        static Transform OnCreatePooledItem()
        {
            var item = new GameObject().transform;
            item.localPosition = Vector3.zero;
            item.localRotation = Quaternion.identity;
            item.localScale = Vector3.one;

            return item;
        }

        static void OnGetPooledItem(Transform item)
        {
            if (item == null)
                return;

            item.hideFlags &= ~HideFlags.HideInHierarchy;
        }

        static void OnReleasePooledItem(Transform item)
        {
            if (item == null)
                return;

            // Don't clear the parent of the GameObject on release since there could be issues
            // with changing it while a parent GameObject is deactivating, which logs an error.
            // By keeping it under this interactable, it could mean that GameObjects in the pool
            // have a chance of being destroyed, but we check that the GameObject we obtain from the pool
            // has not been destroyed. This means potentially more creations of new GameObjects, but avoids
            // the issue with reparenting.

            // Hide the GameObject in the Hierarchy so it doesn't pollute this Interactable's hierarchy
            // when it is no longer used.
            item.hideFlags |= HideFlags.HideInHierarchy;
        }

        static void OnDestroyPooledItem(Transform item)
        {
            if (item == null)
                return;

            Destroy(item.gameObject);
        }
    }
}
