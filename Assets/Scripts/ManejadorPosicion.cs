using UnityEngine;

public class ManejadorPosicion : MonoBehaviour
{
    void Start()
    {
        if (ControladorGlobal.acabamosDeRegresar)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("PlayerJuan");
            if (jugador != null)
            {
                jugador.transform.position = ControladorGlobal.posicionGuardada;
                Debug.Log("Juan posicionado en la mesa. Iniciando enfriamiento de 50s.");
            }
        }
    }
}