using UnityEngine;



public class RosaBoncheo : MonoBehaviour

{

    [HideInInspector] public bool esCerrada;



    private Boncheo cerebro;

    private Vector3 offset;

    private Camera cam;

    private bool enRegleta = false;

    private bool yaColocada = false;

    private Vector3 posicionInicial;



    void Start()

    {

        posicionInicial = transform.position;



        cerebro = Object.FindFirstObjectByType<Boncheo>();

        if (cerebro == null)

            Debug.LogError("[RosaBoncheo] No se encontró Boncheo en la escena.");



        cam = Camera.main;

        if (cam == null)

            Debug.LogError("[RosaBoncheo] No se encontró Camera.main.");



        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)

        {

            rb.gravityScale = 0;

            rb.bodyType = RigidbodyType2D.Kinematic;

        }

    }



    void OnMouseDown()

    {

        if (yaColocada || cam == null) return;

        offset = transform.position - PosicionMouse();

    }



    void OnMouseDrag()

    {

        if (yaColocada || cam == null) return;

        transform.position = PosicionMouse() + offset;

    }



    void OnMouseUp()

    {

        if (yaColocada || cam == null || cerebro == null) return;



        if (enRegleta)

        {

            bool esCorrecta = cerebro.RosaEsCorrecta(esCerrada);

            cerebro.RosaColocadaEnRegleta(esCorrecta, gameObject);



            if (esCorrecta)

                yaColocada = true;

            else

                transform.position = posicionInicial;

        }

    }



    void OnTriggerEnter2D(Collider2D otro)

    {

        if (otro.CompareTag("MesaBoncheo")) enRegleta = true;

    }



    void OnTriggerExit2D(Collider2D otro)

    {

        if (otro.CompareTag("MesaBoncheo")) enRegleta = false;

    }





    public void Resetear()

    {

        yaColocada = false;

        enRegleta = false;

        transform.position = posicionInicial;

    }



    Vector3 PosicionMouse()

    {

        Vector3 mp = Input.mousePosition;

        mp.z = cam.orthographic

        ? Mathf.Abs(cam.transform.position.z)

        : 10f;

        return cam.ScreenToWorldPoint(mp);

    }

}