using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ManejadorRetorno : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreEscenaMenu = "MainMenu";

    [Header("Ajustes de Teletransporte")]
    [Tooltip("Distancia extra para alejar a Juan de la mesa")]
    public float offsetRetroceso = 0.1f;

    private CharacterController _cc;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (ControladorGlobal.acabamosDeRegresar)
        {
      
            ControladorGlobal.mesasBloqueadas = true;
            StartCoroutine(ReposicionarJuan());
        }
    }

    IEnumerator ReposicionarJuan()
    {
        if (_cc != null) _cc.enabled = false;

        yield return new WaitForEndOfFrame();

 
        if (ControladorGlobal.posicionGuardada != Vector3.zero)
        {
            
            Vector3 posicionSegura = ControladorGlobal.posicionGuardada
                                   + transform.forward * -offsetRetroceso;
            transform.position = posicionSegura;
            Debug.Log("<color=green>Juan reposicionado frente a la mesa</color>");
        }

        yield return new WaitForEndOfFrame();

        if (_cc != null) _cc.enabled = true;

        if (ControladorGlobal.escenaDeRetorno == "empaque")
        {
            Debug.Log("<color=yellow>Turno finalizado</color>");
            Invoke("IrAlMenuFinal", 3f);
        }
    }

    void IrAlMenuFinal()
    {
        ControladorGlobal.acabamosDeRegresar = false;
        ControladorGlobal.escenaDeRetorno = "";
        ControladorGlobal.mesasBloqueadas = false;
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}