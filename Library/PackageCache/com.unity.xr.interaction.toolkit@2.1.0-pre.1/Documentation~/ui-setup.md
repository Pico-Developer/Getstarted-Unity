# UI interaction setup

To interact with Unity's built-in UI elements, you need to perform extra steps, particularly if you're dealing with 3D-tracked devices. The XR Interaction Toolkit package provides a number of new components that you can use to convert an XR controller to work seamlessly with the UI, as well as helper menu options that handle basic configuration settings.

## Using the GameObject menu

The XR Interaction Toolkit package comes with menu items that perform basic setup. Use these helpers to create a new UI Canvas. You can access them from the **GameObject &gt; XR** menu.

|Helper|Function|
|---|---|
|**UI Canvas**|Creates a new world-space Canvas that you can add standard UI elements to. If you haven't configured an Event System yet, it also creates and configures a new Event System for XR.|
|**UI EventSystem**|Creates a new Event System for XR, or modifies and selects the existing one in the loaded scenes.|

## Event System

The [Event System](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/EventSystem.html) component acts as a central dispatch for UI events to process input, and update individual active canvases. Additionally, each Event System needs an Input Module to process input. Use the default configuration, pictured below, as a starting point. Only one Input Module can be active at one time. The Tracked Device Physics Raycaster can also be added to a scene so that objects with physics colliders are able to receive Event System events from tracked devices.

## XR UI Input Module

The **XR UI Input Module** is the component that XRI requires to properly interface with the Event System. It works in concert with the [XR Ray Interactor](xr-ray-interactor.md) to ensure XRI interactions with the UI are processed correctly. It also handles input from other non-XR sources (such as gamepad or mouse) if configured to do so. In the component configuration pictured below, the **Input System UI Actions** and **Input Manager** options are displayed. Depending on the **Active Input Handling** setting in **Edit &gt; Project Settings &gt; Player**, one or both of these configuration options may be visible. If you are using the Input System package for handling input you have the option of manually entering the Input Action References for each of the fields or you can use the preset provided with the [Starter Assets](samples.md#starter-assets) sample.

> [!NOTE]
> If you have **Active Input Handling** set to **Both**, you will need to make sure to set the **Active Input Mode** on the **XR UI Input Module** to your desired source of input. Keep in mind that there may be minor performance impacts by setting this option to Both.

> [!IMPORTANT]
> If you have an existing Canvas or Event System, you will likely have a **Standalone Input Module** or **Input System UI Input Module** component which will prevent proper input processing. Remove it by clicking the **More menu (&#8942;)** and selecting **Remove Component**. Other UI Input Modules are not compatible with the **XR UI Input Module** and may cause undesired or unexpected behavior. Therefore, only use a single Input Module to handle UI interactions.

![ui-event-system-setup](images/ui-event-system-setup.png)

## Canvas
All UI elements exist in the canvas. In the XR Interaction Toolkit, a user can only interact with canvases that have their **Render Mode** set to **World Space**. The XR Interaction Toolkit package contains a new component (pictured below) called the Tracked Device Graphic Raycaster. This component lets you use 3D tracked devices to highlight and select UI elements in that canvas.

![ui-canvas-setup](images/ui-canvas-setup.png)
