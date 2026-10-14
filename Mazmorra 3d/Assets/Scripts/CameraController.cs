using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Cámara en primera persona: inclina la cámara en vertical y gira el cuerpo del jugador en horizontal.
/// </summary>
[DisallowMultipleComponent]
public class CameraController : MonoBehaviour
{
    [Header("Sensibilidad")]
    [Tooltip("Grados de giro por píxel de movimiento del ratón.")]
    [SerializeField, Min(0.01f)] private float lookSensitivity = 0.15f;
    [Tooltip("Ángulo máximo de inclinación hacia arriba y hacia abajo.")]
    [SerializeField, Range(10f, 90f)] private float maxPitch = 85f;

    [Header("Referencias")]
    [Tooltip("Cuerpo que gira en horizontal. Si se deja vacío se usa el padre de la cámara.")]
    [SerializeField] private Transform playerBody;

    private float pitch;

    private void Start()
    {
        if (playerBody == null)
        {
            playerBody = transform.parent;
        }

        if (playerBody == null)
        {
            Debug.LogWarning($"[{nameof(CameraController)}] No hay Player Body asignado: la cámara solo girará en vertical.", this);
        }

        SetCursorLocked(true);
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        // Si el cursor está libre (por ejemplo tras pulsar Esc en el editor), un clic lo vuelve a bloquear.
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (mouse.leftButton.wasPressedThisFrame) SetCursorLocked(true);
            return;
        }

        // El delta del Input System ya es "por frame": NO se multiplica por Time.deltaTime,
        // de lo contrario la sensibilidad cambiaría con los FPS.
        Vector2 delta = mouse.delta.ReadValue() * lookSensitivity;

        pitch = Mathf.Clamp(pitch - delta.y, -maxPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up, delta.x);
        }
    }

    private static void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}