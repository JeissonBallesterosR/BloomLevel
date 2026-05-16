using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;

using TMPro;



public class Boncheo : MonoBehaviour

{

    [Header("Referencias HUD")]

    public TextMeshProUGUI textoNombreHUD;



    [Header("Referencias UI Juego")]

    public TextMeshProUGUI textoPuntaje;

    public TextMeshProUGUI textoTiempo;

    public TextMeshProUGUI textoResultado;



    [Header("Botones y Escenas")]

    public Button btnBonchar;

    public Button btnSalir;

    public string escenaDestino = "SampleScene";



    [Header("Sprites Rosa")]

    public SpriteRenderer imagenRosa;

    public Sprite spriteRosaCerrada;

    public Sprite spriteRosaAbierta;



    [Header("Configuración")]

    public bool pedidoCerradas = true;





    int puntaje = 0;

    int contErrores = 0;

    float tiempo = 120f;

    bool activo = true;

    bool estaPausado = false;

    bool saliendo = false;



    void Start()

    {

        Time.timeScale = 1f;

        saliendo = false;



        if (DataManager.Instance != null && textoNombreHUD != null)

            textoNombreHUD.text = "Operador: " + DataManager.Instance.playerName;



        if (btnBonchar != null) btnBonchar.onClick.AddListener(SimularBoncheo);

        if (btnSalir != null) btnSalir.onClick.AddListener(VolverAlMundo3D);



        ActualizarUI();

    }



    void Update()

    {



        if (Input.GetKeyDown(KeyCode.Escape) && !saliendo)

        {

            CancelInvoke();

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

        {

            textoTiempo.text = "Tiempo: " + Mathf.Max(0, Mathf.CeilToInt(tiempo)) + "s";

            textoTiempo.color = tiempo < 10f ? Color.red : Color.white;

        }



        if (tiempo <= 0f) TerminarJuego();

    }



    public bool RosaEsCorrecta(bool esCerrada)

    {

        return esCerrada == pedidoCerradas;

    }





    public void RosaColocadaEnRegleta(bool esCorrecta, GameObject rosa)

    {

        if (!activo || saliendo) return;



        if (esCorrecta)

        {

            puntaje += 20;

            MostrarResultado("¡Rosa correcta!", Color.green);

        }

        else

        {

            contErrores++;

            puntaje = Mathf.Max(0, puntaje - 10);

            MostrarResultado("Rosa incorrecta — necesitas " +

                (pedidoCerradas ? "CERRADA" : "ABIERTA"), Color.red);

        }



        ActualizarUI();

    }



    void SimularBoncheo()

    {

        if (!activo || estaPausado || saliendo) return;

        puntaje += 20;

        ActualizarUI();

        MostrarResultado("¡Rosa Boncheada!", Color.green);



        if (imagenRosa != null)

            imagenRosa.sprite = Random.value > 0.5f

                ? spriteRosaCerrada : spriteRosaAbierta;

    }



    void ActualizarUI()

    {

        if (textoPuntaje != null)

            textoPuntaje.text = "Puntaje: " + puntaje;

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

        CancelInvoke();



        if (DataManager.Instance != null)

            DataManager.Instance.SaveData(puntaje);



        MostrarResultado("FIN DEL TURNO — Puntaje: " + puntaje, Color.yellow);

        Invoke("VolverAlMundo3D", 3f);

    }



    public void VolverAlMundo3D()

    {

        if (saliendo) return;

        saliendo = true;



        CancelInvoke();

        Time.timeScale = 1f;





        ControladorGlobal.acabamosDeRegresar = true;

        ControladorGlobal.escenaDeRetorno = "boncheo";

        ControladorGlobal.mesasBloqueadas = true;



        if (!string.IsNullOrEmpty(escenaDestino))

            SceneManager.LoadScene(escenaDestino);

        else

            Debug.LogError("[Boncheo] escenaDestino está vacío.");

    }

}