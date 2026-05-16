using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("Referencias de UI")]
    public Image barraVerde; // Aquí arrastrarás tu imagen verde del Canvas

    [Header("Configuración")]
    public string nombreEscenaMenu = "MainMenu"; // Asegúrate de que tu escena de menú se llame así

    void Start()
    {
        if (barraVerde != null)
        {
            barraVerde.fillAmount = 0; // Empezamos la barra en cero
        }
        StartCoroutine(CargarEscena());
    }

    IEnumerator CargarEscena()
    {
        // Esto inicia la carga de la siguiente escena en segundo plano
        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscenaMenu);

        // No dejes que la escena se active sola para que podamos ver la barra llenarse
        operacion.allowSceneActivation = false;

        while (!operacion.isDone)
        {
            // El progreso de carga va de 0 a 0.9
            float progreso = Mathf.Clamp01(operacion.progress / 0.9f);

            if (barraVerde != null)
            {
                barraVerde.fillAmount = progreso;
            }

            // Cuando la carga llega al final (0.9), esperamos un poco y entramos al menú
            if (operacion.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1.0f);
                operacion.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}