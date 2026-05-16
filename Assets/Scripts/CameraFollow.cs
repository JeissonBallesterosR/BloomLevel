using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform target; // Arrastra aquí el GameObject "juan"

    [Header("Configuración")]
    public Vector3 offset = new Vector3(0f, 2f, -5f); // altura y distancia
    public float smoothSpeed = 8f;

    void LateUpdate()
    {
        if (target == null) return;

        // La cámara sigue posición suavemente
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Siempre mira al personaje
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}