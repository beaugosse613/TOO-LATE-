using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance { get; private set; }

    public event Action JumpPressed;

    private InputSystem_Actions _actions;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        _actions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _actions.Player.Jump.performed += HandleJump;
        _actions.Player.Enable();
    }

    void OnDisable()
    {
        _actions.Player.Jump.performed -= HandleJump;
        _actions.Player.Disable();
    }

    private void HandleJump(InputAction.CallbackContext ctx) => JumpPressed?.Invoke();
}
