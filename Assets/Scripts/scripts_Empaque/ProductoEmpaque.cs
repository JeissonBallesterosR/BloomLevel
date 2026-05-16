using UnityEngine;

public class ProductoEmpaque : MonoBehaviour
{
    [HideInInspector] public bool esBulk;
    [HideInInspector] public bool yaEmpacado = false;

    private Empaque  cerebro;
    private Vector3  offset;
    private Camera   cam;
    private bool     enCaja      = false;
    private bool     siendoArrastrado = false;

    void Start()
    {
        cerebro = Object.FindFirstObjectByType<Empaque>();
        cam     = Camera.main;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.bodyType     = RigidbodyType2D.Kinematic;
        }
    }

    void Update()
    {
        if (yaEmpacado) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

        // Iniciar arrastre — clic sobre el producto
        if (Input.GetMouseButtonDown(0))
        {
            float distancia = Vector2.Distance(mouseWorld, transform.position);
            if (distancia < 0.5f)
            {
                siendoArrastrado = true;
                offset = transform.position - mouseWorld;
                Debug.Log("Producto agarrado");
            }
        }

        // Arrastrar
        if (siendoArrastrado && Input.GetMouseButton(0))
        {
            transform.position = mouseWorld + offset;
        }

        // Soltar
        if (siendoArrastrado && Input.GetMouseButtonUp(0))
        {
            siendoArrastrado = false;
            Debug.Log("Producto soltado. enCaja = " + enCaja);

            if (enCaja)
            {
                yaEmpacado = true;
                Debug.Log("LLAMANDO ProductoColocadoEnCaja");
                cerebro.ProductoColocadoEnCaja(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("ZonaCaja"))
        {
            enCaja = true;
            Debug.Log("ENTRÓ en zona caja");
        }
    }

    void OnTriggerStay2D(Collider2D otro)
    {
        if (otro.CompareTag("ZonaCaja"))
            enCaja = true;
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.CompareTag("ZonaCaja"))
        {
            enCaja = false;
            Debug.Log("SALIÓ de zona caja");
        }
    }
}