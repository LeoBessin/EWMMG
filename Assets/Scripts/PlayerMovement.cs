using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;

    private InputAction moveAction;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            GetComponent<Renderer>().material.color = Color.red;

            moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");
            moveAction.Enable();
        }
    }

    public override void OnNetworkDespawn()
    {
        moveAction?.Disable();
        moveAction?.Dispose();
    }

    void Update()
    {
        if (!IsOwner) return;

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(input.x, 0f, input.y);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}
