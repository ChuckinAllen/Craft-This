using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, Controls.IPlayerActions
{
    [SerializeField] private float rotationSensitivity = 1.0f;
    private Vector2 lastPointerPosition;

    public bool IsHolding { get; private set; }
    public Vector2 MovementValue { get; private set; }

    public event Action Down;
    public event Action Hold;
    public event Action<float> Rotate;

    private Controls controls;

    private void Awake()
    {
        controls = new Controls();
        controls.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDestroy()
    {
        controls.Player.Disable();
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        Down?.Invoke();
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        //if (!context.performed) { return; }

        MovementValue = context.ReadValue<Vector2>();
    }


    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; };

        Vector2 pointerPosition = context.ReadValue<Vector2>();
        if (lastPointerPosition != Vector2.zero)
        {
            float rotationDelta = (pointerPosition.x - lastPointerPosition.x) * rotationSensitivity;
            Rotate?.Invoke(rotationDelta);
        }
        lastPointerPosition = pointerPosition;
    }
}
