using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;

    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        // Evita que el Rigidbody rote solo
        if (rb != null)
            rb.freezeRotation = true;
    }

    void Update()
    {
        HandleRotation();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleRotation()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float rotDir = 0f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) rotDir = 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) rotDir = -1f;

        if (rotDir != 0f)
            transform.Rotate(0f, rotDir * rotationSpeed * Time.deltaTime, 0f);
    }

    void HandleMovement()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        Vector3 dir = Vector3.zero;

        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)
            dir = transform.forward;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)
            dir = -transform.forward;

        if (rb != null)
        {
            Vector3 vel = dir * moveSpeed;
            vel.y = rb.linearVelocity.y; // respeta gravedad
            rb.linearVelocity = vel;
        }
        else
        {
            // Si no hay Rigidbody, mueve directo
            transform.Translate(dir * moveSpeed * Time.fixedDeltaTime, Space.World);
        }
    }

    void HandleAnimation()
    {
        var kb = Keyboard.current;
        if (kb == null || animator == null) return;

        float x = 0f, y = 0f;

        // Y controla adelante/atrás
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) y = 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) y = -1f;

        // X controla giro izquierda/derecha en el Blend Tree
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x = 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x = -1f;

        // Suaviza la transición para evitar cortes bruscos
        animator.SetFloat("X", x, 0.1f, Time.deltaTime);
        animator.SetFloat("Y", y, 0.1f, Time.deltaTime);
    }
}