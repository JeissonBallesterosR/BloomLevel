using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Clasificacion : MonoBehaviour
{
    [Header("Configuración HUD")]
    public TextMeshProUGUI textoNombreHUD;
    public string escenaMundo3D = "SampleScene";

    [Header("Rosa en escena")]
    public SpriteRenderer imagenRosa;
    public Sprite spriteRosaCerrada, spriteRosaAbierta, spriteRosaTorcida, spriteRosaDefecto;

    [Header("UI Textos")]
    public TextMeshProUGUI textoLongitud;
    public TextMeshProUGUI textoApertura, textoTorcedura, textoResultado, textoPuntaje, textoTiempo;

    [Header("Botones")]
    public Button btnNacional;
    public Button btnGrado40, btnGrado50, btnGrado60;

    float longitud;
    bool estaCerrada, estaTorcida, tieneDefecto;
    bool rosaEnRegleta = false;
    bool activo = true;
    bool estaPausado = false;
    bool saliendo = false; // ? FLAG NUEVO — evita doble carga

    int puntaje = 0;
    int contErrores = 0;
    float tiempo = 120f; // CORREGIDO: 15s ? 120s

    void Start()
    {
        Time.timeScale = 1f;
        saliendo = false;

        if (DataManager.Instance != null && textoNombreHUD != null)
            textoNombreHUD.text = "Op: " + DataManager.Instance.playerName;

        if (btnNacional != null) btnNacional.onClick.AddListener(() => Clasificar("Nacional"));
        if (btnGrado40 != null) btnGrado40.onClick.AddListener(() => Clasificar("Grado40"));
        if (btnGrado50 != null) btnGrado50.onClick.AddListener(() => Clasificar("Grado50"));
        if (btnGrado60 != null) btnGrado60.onClick.AddListener(() => Clasificar("Grado60"));

        GenerarRosa();
    }

    void Update()
    {
        // CORRECCIÓN: saliendo evita que ESC se procese múltiples veces
        if (Input.GetKeyDown(KeyCode.Escape) && !saliendo)
        {
            CancelInvoke(); // cancela todos los Invokes pendientes
            VolverAlMundo3D();
            return;
        }

        if (Input.GetKeyDown(KeyCode.P) && !saliendo)
        {
            estaPausado = !estaPausado;
            Time.timeScale = estaPausado ? 0f : 1f;
        }

        if (!activo || estaPausado) return;

        tiempo -= Time.deltaTime;
        if (textoTiempo != null)
            textoTiempo.text = "Tiempo: " + Mathf.Max(0, Mathf.CeilToInt(tiempo)) + "s";

        // Color rojo al final
        if (textoTiempo != null)
            textoTiempo.color = tiempo < 10f ? Color.red : Color.white;

        if (tiempo <= 0f) TerminarJuego();
    }

    public void RosaEnRegleta()
    {
        rosaEnRegleta = true;
        if (textoLongitud != null) textoLongitud.text = "Longitud: " + longitud + " cm";
        if (textoResultado != null)
        {
            textoResultado.text = "¿A qué grado pertenece?";
            textoResultado.color = Color.white;
        }
        ActivarBotones(true);
    }

    public void RosaSaleRegleta()
    {
        rosaEnRegleta = false;
        ActivarBotones(false);
        if (textoResultado != null)
        {
            textoResultado.text = "Lleva la rosa a la regleta";
            textoResultado.color = Color.white;
        }
    }

    void Clasificar(string elegido)
    {
        if (!rosaEnRegleta || !activo || saliendo) return;

        string correcto = NivelCorrecto();

        if (elegido == correcto)
        {
            puntaje += 10;
            MostrarResultado("CORRECTO — " + longitud + " cm ? " + correcto, Color.green);
        }
        else
        {
            contErrores++;
            puntaje = Mathf.Max(0, puntaje - 5);
            MostrarResultado("ERROR — era " + correcto + " (elegiste " + elegido + ")", Color.red);
        }

        if (textoPuntaje != null) textoPuntaje.text = "Puntaje: " + puntaje;
        ActivarBotones(false);
        Invoke("GenerarRosa", 1.2f);
    }

    string NivelCorrecto()
    {
        if (tieneDefecto || estaTorcida || longitud < 45f) return "Nacional";
        if (longitud >= 63f) return "Grado60";
        if (longitud >= 56f) return "Grado50";
        return "Grado40";
    }

    void GenerarRosa()
    {
        // CORRECCIÓN: no genera si está saliendo o inactivo
        if (!activo || saliendo) return;

        longitud = Random.Range(40, 70);
        estaCerrada = Random.value > 0.5f;
        estaTorcida = Random.value < 0.15f;
        tieneDefecto = Random.value < 0.1f;

        if (imagenRosa != null)
        {
            if (tieneDefecto) imagenRosa.sprite = spriteRosaDefecto ?? spriteRosaAbierta;
            else if (estaTorcida) imagenRosa.sprite = spriteRosaTorcida ?? spriteRosaAbierta;
            else imagenRosa.sprite = estaCerrada ? spriteRosaCerrada : spriteRosaAbierta;

            imagenRosa.transform.position = new Vector3(-1.5f, 0, 0);
        }

        rosaEnRegleta = false;
        ActivarBotones(false);

        if (textoResultado != null)
        {
            textoResultado.text = "Lleva la rosa a la regleta";
            textoResultado.color = Color.white;
        }
    }

    void ActivarBotones(bool estado)
    {
        if (btnNacional != null) btnNacional.interactable = estado;
        if (btnGrado40 != null) btnGrado40.interactable = estado;
        if (btnGrado50 != null) btnGrado50.interactable = estado;
        if (btnGrado60 != null) btnGrado60.interactable = estado;
    }

    void MostrarResultado(string msg, Color color)
    {
        if (textoResultado == null) return;
        textoResultado.text = msg;
        textoResultado.color = color;
    }

    void TerminarJuego()
    {
        if (!activo || saliendo) return;
        activo = false;
        CancelInvoke(); // CORRECCIÓN: cancela GenerarRosa pendiente

        MostrarResultado("¡TURNO TERMINADO!", Color.yellow);
        ActivarBotones(false);

        if (DataManager.Instance != null)
            DataManager.Instance.SaveData(puntaje);

        Invoke("VolverAlMundo3D", 3f);
    }

    public void VolverAlMundo3D()
    {
        if (saliendo) return; // CORRECCIÓN: bloquea llamadas duplicadas
        saliendo = true;

        CancelInvoke();
        Time.timeScale = 1f;
        ControladorGlobal.acabamosDeRegresar = true;
        ControladorGlobal.escenaDeRetorno = SceneManager.GetActiveScene().name;

        if (string.IsNullOrEmpty(escenaMundo3D))
        {
            Debug.LogError("[Clasificacion] escenaMundo3D vacío.");
            return;
        }
        SceneManager.LoadScene(escenaMundo3D);
    }
}