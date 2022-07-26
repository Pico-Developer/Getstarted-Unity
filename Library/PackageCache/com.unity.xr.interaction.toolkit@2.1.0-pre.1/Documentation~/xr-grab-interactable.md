# XR Grab Interactable

Interactable component that hooks into the interaction system (via [XRInteractionManager](xr-interaction-manager.md)) to allow basic "grab" functionality. Can attach to an Interactor that selects this Interactable component and follow it around while obeying physics (and inherit velocity when released).

![XRGrabInteractable component](images/xr-grab-interactable.png)

| **Property** | **Description** |
|--|--|
| **Interaction Manager** | The [XRInteractionManager](xr-interaction-manager.md) that this Interactable will communicate with (will find one if **None**). |
| **Interaction Layer Mask** | Allows interaction with Interactors whose [Interaction Layer Mask](interaction-layers.md) overlaps with any Layer in this Interaction Layer Mask. |
| **Colliders** | Colliders to use for interaction with this Interactable (if empty, will use any child Colliders). |
| **Distance Calculation Mode** | Specifies how distance is calculated to Interactors, from fastest to most accurate. If using Mesh Colliders, Collider Volume only works if the mesh is convex. |
| &emsp;Transform Position | Calculates the distance using the Interactable's transform position. This option has low performance cost, but it may have low distance calculation accuracy for some objects. |
| &emsp;Collider Position | Calculates the distance using the Interactable's Colliders list using the shortest distance to each. This option has moderate performance cost and should have moderate distance calculation accuracy for most objects. |
| &emsp;Collider Volume | Calculates the distance using the Interactable's Colliders list using the shortest distance to the closest point of each (either on the surface or inside the Collider). This option has high performance cost but high distance calculation accuracy. |
| **Custom Reticle** | The reticle that appears at the end of the line when valid. |
| **Select Mode** | Indicates the selection policy of an Interactable. This controls how many Interactors can select this Interactable.<br />The value is only read by the Interaction Manager when a selection attempt is made, so changing this value from **Multiple** to **Single** will not cause selections to be exited. |
| &emsp;Single | Set **Select Mode** to **Single** to prevent additional simultaneous selections from more than one Interactor at a time. |
| &emsp;Multiple | Set **Select Mode** to **Multiple** to allow simultaneous selections on the Interactable from multiple Interactors. |
| **Movement Type** | Specifies how this object moves when selected, either through setting the velocity of the `Rigidbody`, moving the kinematic `Rigidbody` during Fixed Update, or by directly updating the `Transform` each frame. |
| &emsp;Velocity Tracking | Set **Movement Type** to Velocity Tracking to move the Interactable object by setting the velocity and angular velocity of the Rigidbody. Use this if you don't want the object to be able to move through other Colliders without a Rigidbody as it follows the Interactor, however with the tradeoff that it can appear to lag behind and not move as smoothly as Instantaneous. |
| &emsp;Kinematic | Set **Movement Type** to Kinematic to move the Interactable object by moving the kinematic Rigidbody towards the target position and orientation. Use this if you want to keep the visual representation synchronized to match its Physics state, and if you want to allow the object to be able to move through other Colliders without a Rigidbody as it follows the Interactor. |
| &emsp;Instantaneous | Set **Movement Type** to Instantaneous to move the Interactable object by setting the position and rotation of the Transform every frame. Use this if you want the visual representation to be updated each frame, minimizing latency, however with the tradeoff that it will be able to move through other Colliders without a Rigidbody as it follows the Interactor. |
| **Retain Transform Parent** | Enable to have Unity set the parent of this object back to its original parent this object was a child of after this object is dropped. |
| **Track Position** | Enable to have this object follow the position of the Interactor when selected. |
| **Smooth Position** | Enable to have Unity apply smoothing while following the position of the Interactor when selected. |
| **Smooth Position Amount** | Scale factor for how much smoothing is applied while following the position of the Interactor when selected. The larger the value, the closer this object will remain to the position of the Interactor. |
| **Tighten Position** | Reduces the maximum follow position difference when using smoothing.<br />Fractional amount of how close the smoothed position should remain to the position of the Interactor when using smoothing. The value ranges from 0 meaning no bias in the smoothed follow distance, to 1 meaning effectively no smoothing at all. |
| **Velocity Damping** | Scale factor of how much to dampen the existing velocity when tracking the position of the Interactor. The smaller the value, the longer it takes for the velocity to decay.<br />Only applies when **Movement Type** is in Velocity Tracking mode. |
| **Velocity Scale** | Scale factor Unity applies to the tracked velocity while updating the `Rigidbody` when tracking the position of the Interactor.<br />Only applies when **Movement Type** is in Velocity Tracking mode. |
| **Track Rotation** | Enable to have this object follow the rotation of the Interactor when selected. |
| **Smooth Rotation** | Apply smoothing while following the rotation of the Interactor when selected. |
| **Smooth Rotation Amount** | Scale factor for how much smoothing is applied while following the rotation of the Interactor when selected. The larger the value, the closer this object will remain to the rotation of the Interactor. |
| **Tighten Rotation** | Reduces the maximum follow rotation difference when using smoothing.<br />Fractional amount of how close the smoothed rotation should remain to the rotation of the Interactor when using smoothing. The value ranges from 0 meaning no bias in the smoothed follow rotation, to 1 meaning effectively no smoothing at all. |
| **Angular Velocity Damping** | Scale factor of how much Unity dampens the existing angular velocity when tracking the rotation of the Interactor. The smaller the value, the longer it takes for the angular velocity to decay.<br />Only applies when **Movement Type** is in _VelocityTracking_ mode. |
| **Angular Velocity Scale** | Scale factor Unity applies to the tracked angular velocity while updating the `Rigidbody` when tracking the rotation of the Interactor.<br />Only applies when **Movement Type** is in Velocity Tracking mode. |
| **Throw On Detach** | Enable to have this object inherit the velocity of the Interactor when released. |
| **Throw Smoothing Duration** | Time period to average thrown velocity over. |
| **Throw Smoothing Curve** | The curve to use to weight thrown velocity smoothing (most recent frames to the right). |
| **Throw Velocity Scale** | Scale factor Unity applies to this object's velocity inherited from the Interactor when released. |
| **Throw Angular Velocity Scale** | Scale factor Unity applies to this object's angular velocity inherited from the Interactor when released. |
| **Force Gravity On Detach** | Forces this object to have gravity when released (will still use pre-grab value if this is `false` / unchecked). |
| **Attach Transform** | The attachment point Unity uses on this Interactable (will use this object's position if none set). |
| **Use Dynamic Attach** | Enable to make the effective attachment point based on the pose of the Interactor when the selection is made. |
| **Match Position** | Match the position of the Interactor's attachment point when initializing the grab. This will override the position of Attach Transform. |
| **Match Rotation** | Match the rotation of the Interactor's attachment point when initializing the grab. This will override the rotation of Attach Transform. |
| **Snap To Collider Volume** | Adjust the dynamic attachment point to keep it on or inside the Colliders that make up this object. |
| **Attach Ease In Time** | Time in seconds Unity eases in the attach when selected (a value of 0 indicates no easing). |
| **Attach Point Compatibility Mode** | Controls the method used when calculating the target position of the object. Use `AttachPointCompatibilityMode.Default` for consistent attach points between all `XRBaseInteractable.MovementType` values. Marked for deprecation, this property will be removed in a future version.<br />This is a backwards compatibility option in order to keep the old, incorrect method of calculating the attach point. Projects that already accounted for the difference can use the Legacy option to maintain the same attach positioning from older versions without needing to modify the **Attach Transform** position. |
| **Interactable Events** | See the [Interactable Events](interactable-events.md) page. |
