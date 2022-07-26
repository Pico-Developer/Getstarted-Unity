# AR interaction

> [!NOTE]
> AR interaction components are only available in a Project that also includes the [AR Foundation](https://docs.unity3d.com/Manual/com.unity.xr.arfoundation.html) package. You can install the AR Foundation package via the [Package Manager](https://docs.unity3d.com/Manual/upm-ui-install.html).

## AR gestures

AR interaction is mostly driven by an AR Gesture Interactor component that translates touch events into gestures such as tap, drag, and pinch. These gestures get fed down to gesture Interactables that turn these into interactions.

The XR Interaction Toolkit package comes with a number of pre-defined gestures and gesture interactables, but you can always extend this package by defining your own gestures.

| Gesture | Triggered by input | Maps to interactable |
|-|-|-|
| **Tap** | User touches the screen | [AR Placement Interactable](ar-placement-interactable.md), [AR Selection Interactable](ar-selection-interactable.md) |
| **Drag** | User drags finger across screen | [AR Translation Interactable](ar-translation-interactable.md) |
| **Pinch** | User moves two fingers together or apart in a straight line | [AR Scale Interactable](ar-scale-interactable.md) |
| **Twist** | User rotates two fingers around a center point | [AR Rotation Interactable](ar-rotation-interactable.md) |
| **Two Finger Drag** | User drags with two fingers | Nothing currently |

The AR Gesture Interactor component translates screen touches to gestures. Unity feeds gestures down to Interactables, which then respond to the gesture event. The AR Gesture Interactor and its gesture recognizers require an XR Origin in the scene.

## Placement of objects with the AR Placement Interactable

The [AR Placement Interactable](ar-placement-interactable.md) component facilitates placing objects in the scene. Users specify a placement prefab that Unity later places on an AR plane when a tap occurs. Unity also generates a ray cast against the plane at the same time. The Prefab can contain additional AR interactables to facilitate further gesture interaction.

## AR annotations

Use the [AR Annotation Interactable](ar-annotation-interactable.md) to place annotations alongside virtual objects in an AR scene. These annotations are visualization GameObjects that the application can show or hide when they satisfy a set of constraints. Each annotation has a minimum and maximum range from the Camera at which it displays, as well as a maximum Field of View (FOV) center offset from the Camera to hide or minimize annotations that are not centered in the user's view.
