
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteraccionMesa : MonoBehaviour
{
    [Header("Configuración")]
    public string escena2D = "";
    public float tiempoEnfriamiento = 50f; 
    public GameObject iconoBloqueado;

    private bool estoyEnEnfriamiento = true;
    private float contador = 0f;

    void Start()
    {
        if (ControladorGlobal.acabamosDeRegresar &&
            ControladorGlobal.escenaDeRetorno == escena2D)
        {
            ForzarBloqueo();
        }
    }

    void ForzarBloqueo()
    {
        estoyEnEnfriamiento = true;
        contador = tiempoEnfriamiento;
        ControladorGlobal.mesasBloqueadas = true;
        if (iconoBloqueado != null) iconoBloqueado.SetActive(true);
        Debug.Log("<color=cyan>Mesa bloqueada 50s:</color> " + escena2D);
    }

    void Update()
    {
        if (!estoyEnEnfriamiento) return;

        contador -= Time.deltaTime;
        if (contador <= 0f)
        {
            estoyEnEnfriamiento = false;
            ControladorGlobal.mesasBloqueadas = false;
            ControladorGlobal.acabamosDeRegresar = false;
            ControladorGlobal.escenaDeRetorno = "";
            if (iconoBloqueado != null) iconoBloqueado.SetActive(false);
            Debug.Log("<color=green>Mesa lista:</color> " + escena2D);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (estoyEnEnfriamiento) return;
        if (ControladorGlobal.mesasBloqueadas) return;
        if (!other.CompareTag("PlayerJuan")) return;

        ControladorGlobal.posicionGuardada = other.transform.position;
        ControladorGlobal.escenaDeRetorno = escena2D;
        ControladorGlobal.acabamosDeRegresar = false;
        ControladorGlobal.mesasBloqueadas = true;

        Debug.Log("<color=green>Entrando a:</color> " + escena2D);
        SceneManager.LoadScene(escena2D);
    }
}