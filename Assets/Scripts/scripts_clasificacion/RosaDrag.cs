using UnityEngine;

public class RosaDrag : MonoBehaviour
{
    private Clasificacion cerebro;
    private bool enRegleta = false;
    private Vector3 posicionInicial;

    [Header("Velocidad de movimiento")]
    public float velocidad = 5f;

    [Header("Límites de movimiento (para que no salga de pantalla)")]
    public float limiteX = 7f;
    public float limiteY = 4f;

    void Start()
    {
        posicionInicial = transform.position;

        cerebro = Object.FindFirstObjectByType<Clasificacion>();
        if (cerebro == null)
            Debug.LogWarning("[RosaDrag] No se encontró Clasificacion en la escena.");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    void Update()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) y = 1f;
        if (Input.GetKey(KeyCode.S)) y = -1f;

        Vector3 nuevaPos = transform.position +
                           new Vector3(x, y, 0f) * velocidad * Time.deltaTime;

        // CORRECCIÓN: limitar para que no salga de pantalla
        nuevaPos.x = Mathf.Clamp(nuevaPos.x, -limiteX, limiteX);
        nuevaPos.y = Mathf.Clamp(nuevaPos.y, -limiteY, limiteY);

        transform.position = nuevaPos;
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (!otro.CompareTag("Regleta") || enRegleta) return;
        enRegleta = true;
        if (cerebro != null) cerebro.RosaEnRegleta();
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (!otro.CompareTag("Regleta")) return;
        enRegleta = false;
        if (cerebro != null) cerebro.RosaSaleRegleta();
    }

    // Llamado desde Clasificacion al GenerarRosa
    public void ResetearPosicion()
    {
        transform.position = posicionInicial;
        enRegleta = false;
    }
}