using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movimiento en primera persona con CharacterController (WASD / flechas).
/// No hay salto, así que no hace falta un ground check: se usa isGrounded del
/// propio CharacterController para mantener al jugador pegado al suelo.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Pequeña velocidad hacia abajo mientras se está en el suelo.
    // Evita que isGrounded parpadee al bajar rampas o pequeños desniveles.
    private const float GroundedStickVelocity = -2f;

    [Header("Movimiento")]
    [SerializeField, Min(0f)] private float speed = 5f;
    [Tooltip("Aceleración de la gravedad (valor negativo).")]
    [SerializeField] private float gravity = -19.62f;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 input = ReadMoveInput();
        Vector3 horizontalMotion = (transform.right * input.x + transform.forward * input.y) * speed;

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = GroundedStickVelocity;
        }
        verticalVelocity += gravity * Time.deltaTime;

        // Una sola llamada a Move por frame: isGrounded se actualiza una vez y de forma coherente.
        Vector3 motion = horizontalMotion + Vector3.up * verticalVelocity;
        controller.Move(motion * Time.deltaTime);
    }

    /// <summary>Lee WASD / flechas. Las diagonales se normalizan a magnitud 1.</summary>
    private static Vector2 ReadMoveInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return Vector2.zero;

        float x = 0f;
        float y = 0f;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1f;

        return Vector2.ClampMagnitude(new Vector2(x, y), 1f);
    }
}