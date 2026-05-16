using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Necesario para cargar escenas
using TMPro;
using System.Collections.Generic;

public class Empaque : MonoBehaviour
{
    [Header("Configuración de Retorno")]
    public string escenaMundo3D = "SampleScene"; // Nombre de tu escena principal

    [Header("Sprites productos")]
    public Sprite spriteBulk;
    public Sprite spriteBouquet;

    [Header("Sprites cajas abiertas")]
    public Sprite spriteFull;
    public Sprite spriteTabaco;
    public Sprite spriteTercio;
    public Sprite spriteCuarto;
    public Sprite spriteOctavo;

    [Header("Sprites tapas")]
    public Sprite tapFull;
    public Sprite tapTabaco;
    public Sprite tapTercio;
    public Sprite tapCuarto;
    public Sprite tapOctavo;

    [Header("Prefab producto")]
    public GameObject prefabProducto;

    [Header("Objetos en escena")]
    public SpriteRenderer imagenCaja;
    public SpriteRenderer imagenTapa;
    public Transform zonaProductos;

    [Header("Textos UI")]
    public TextMeshProUGUI textoPedido;
    public TextMeshProUGUI textoPuntaje;
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoProgreso;
    public TextMeshProUGUI textoResultado;

    [Header("Panel botones CAJA")]
    public GameObject panelBotonesCaja;
    public Button btnFull, btnTabaco, btnTercio, btnCuarto, btnOctavo;

    [Header("Panel botones TAPA")]
    public GameObject panelBotonesTapa;
    public Button btnTapaFull, btnTapaTabaco, btnTapaTercio, btnTapaCuarto, btnTapaOctavo;

    [Header("Contadores")]
    public TextMeshProUGUI Contador_Full;
    public TextMeshProUGUI Contador_Tabaco, Contador_Tercio, Contador_Cuarto, Contador_Octavo;

    // Estado interno
    bool esBulk;
    int cantidadPedido;
    string clientePedido;
    string cajaCorrecta;
    int capacidadCaja;

    int productosEnCaja = 0;
    bool cajaSeleccionada = false;
    bool esperandoTapa = false;
    string cajaActual = "";

    int contFull = 0, contTabaco = 0, contTercio = 0, contCuarto = 0, contOctavo = 0, contErrores = 0;
    int puntaje = 0;
    float tiempo = 20f;
    bool activo = true;
    bool estaPausado = false; // NUEVO: Estado de pausa

    List<GameObject> productosEnEscena = new List<GameObject>();

    void Start()
    {
        Time.timeScale = 1f; // Asegurar que el juego no empiece pausado

        // Asignar listeners
        btnFull.onClick.AddListener(() => ElegirCaja("Full"));
        btnTabaco.onClick.AddListener(() => ElegirCaja("Tabaco"));
        btnTercio.onClick.AddListener(() => ElegirCaja("Tercio"));
        btnCuarto.onClick.AddListener(() => ElegirCaja("Cuarto"));
        btnOctavo.onClick.AddListener(() => ElegirCaja("Octavo"));

        btnTapaFull.onClick.AddListener(() => ElegirTapa("Full"));
        btnTapaTabaco.onClick.AddListener(() => ElegirTapa("Tabaco"));
        btnTapaTercio.onClick.AddListener(() => ElegirTapa("Tercio"));
        btnTapaCuarto.onClick.AddListener(() => ElegirTapa("Cuarto"));
        btnTapaOctavo.onClick.AddListener(() => ElegirTapa("Octavo"));

        if (imagenTapa != null) imagenTapa.gameObject.SetActive(false);

        DesactivarBotonesTapa();
            ActualizarContadores();
        GenerarPedido();
    }

    void Update()
    {
        // 1. SALIDA CON ESCAPE
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            VolverAlMundo3D();
            return;
        }

        // 2. PAUSA CON LA TECLA P
        if (Input.GetKeyDown(KeyCode.P))
        {
            AlternarPausa();
        }

        if (!activo || estaPausado) return;

        // 3. TIEMPO
        tiempo -= Time.deltaTime;
        if (textoTiempo != null)
            textoTiempo.text = "Tiempo: " + Mathf.Max(0, Mathf.CeilToInt(tiempo)) + "s";

        if (tiempo <= 0f) TerminarJuego();
    }

    void AlternarPausa()
    {
        estaPausado = !estaPausado;
        Time.timeScale = estaPausado ? 0f : 1f;

        if (textoResultado != null)
        {
            textoResultado.text = estaPausado ? "PAUSA (Presiona P)" : "";
            textoResultado.color = Color.white;
        }
    }

    public void VolverAlMundo3D()
    {
        // Limpiar procesos antes de salir
        CancelInvoke();
        Time.timeScale = 1f;

        // Informar al controlador global
        ControladorGlobal.acabamosDeRegresar = true;
        ControladorGlobal.escenaDeRetorno = "empaque";

        SceneManager.LoadScene(escenaMundo3D);
    }

    // --- LOGICA DE PRODUCTOS (Se mantiene igual) ---

    public void ProductoColocadoEnCaja(GameObject producto)
    {
        if (!cajaSeleccionada || estaPausado) return;
        if (productosEnCaja >= capacidadCaja) return;

        productosEnCaja++;
        puntaje += 5;

        textoProgreso.text = "Productos: " + productosEnCaja + "/" + cantidadPedido;
        textoPuntaje.text = "Puntaje: " + puntaje;
        textoResultado.text = productosEnCaja + "/" + cantidadPedido + " en caja";
        textoResultado.color = Color.green;

        if (productosEnCaja >= cantidadPedido)
        {
            Invoke("ActivarBotonesTapa", 0.3f);
        }
    }

    // --- TERMINAR Y RESETEAR ---

    void TerminarJuego()
    {
        if (!activo) return;
        activo = false;

        float prec = (puntaje + contErrores * 10) > 0
            ? (float)puntaje / (puntaje + contErrores * 10) * 100f : 0f;

        textoResultado.text = "TURNO FIN: " + puntaje + " PTS";
        textoResultado.color = Color.yellow;

        DesactivarBotonesCaja();
        DesactivarBotonesTapa();

        // Esperar 3 segundos y volver automáticamente
        Invoke("VolverAlMundo3D", 3f);
    }

    // --- RESTO DE MÉTODOS (BOTONES Y GENERACIÓN) ---

    void ElegirCaja(string caja)
    {
        if (cajaSeleccionada || estaPausado) return;

        if (caja == cajaCorrecta)
        {
            cajaSeleccionada = true;
            cajaActual = caja;
            puntaje += 10;
            textoResultado.text = "Caja correcta.";
            textoResultado.color = Color.green;
            MostrarImagenCaja(caja);
            DesactivarBotonesCaja();
            SpawnProductos();
        }
        else
        {
            contErrores++;
            puntaje = Mathf.Max(0, puntaje - 5);
            textoResultado.text = "Caja incorrecta.";
            textoResultado.color = Color.red;
        }
        textoPuntaje.text = "Puntaje: " + puntaje;
    }

    void ElegirTapa(string tapa)
    {
        if (!esperandoTapa || estaPausado) return;

        if (tapa == cajaCorrecta)
        {
            DesactivarBotonesTapa();
            MostrarTapa(tapa);
        }
        else
        {
            contErrores++;
            puntaje = Mathf.Max(0, puntaje - 5);
            textoResultado.text = "Tapa incorrecta.";
            textoResultado.color = Color.red;
            textoPuntaje.text = "Puntaje: " + puntaje;
        }
    }

    void ActivarBotonesTapa()
    {
        esperandoTapa = true;
        btnTapaFull.interactable = btnTapaTabaco.interactable = btnTapaTercio.interactable =
        btnTapaCuarto.interactable = btnTapaOctavo.interactable = true;
        textoResultado.text = "Pon la tapa";
    }

    void DesactivarBotonesTapa() => btnTapaFull.interactable = btnTapaTabaco.interactable = btnTapaTercio.interactable = btnTapaCuarto.interactable = btnTapaOctavo.interactable = false;
    void DesactivarBotonesCaja() => btnFull.interactable = btnTabaco.interactable = btnTercio.interactable = btnCuarto.interactable = btnOctavo.interactable = false;
    void ActivarBotonesCaja() => btnFull.interactable = btnTabaco.interactable = btnTercio.interactable = btnCuarto.interactable = btnOctavo.interactable = true;

    void MostrarTapa(string tipo)
    {
        if (imagenTapa != null)
        {
            imagenTapa.sprite = ObtenerSpriteTapa(tipo);
            imagenTapa.gameObject.SetActive(true);
            imagenTapa.transform.position = imagenCaja.transform.position + new Vector3(0, 0.5f, 0);
        }
        Invoke("CompletarEmpaque", 1f);
    }

    void CompletarEmpaque()
    {
        puntaje += 20;
        textoPuntaje.text = "Puntaje: " + puntaje;
        switch (cajaActual)
        {
            case "Full": contFull++; break;
            case "Tabaco": contTabaco++; break;
            case "Tercio": contTercio++; break;
            case "Cuarto": contCuarto++; break;
            case "Octavo": contOctavo++; break;
        }
        ActualizarContadores();
        Invoke("GenerarPedido", 1.5f);
    }

    void GenerarPedido()
    {
        foreach (var p in productosEnEscena) if (p != null) Destroy(p);
        productosEnEscena.Clear();
        if (imagenTapa != null) imagenTapa.gameObject.SetActive(false);

        productosEnCaja = 0;
        cajaSeleccionada = false;
        esperandoTapa = false;
        cajaActual = "";
        imagenCaja.sprite = null;

        esBulk = Random.value < 0.5f;
        string[] clientes = { "Florexpo", "Queen", "BenchmarkGrowers" };
        clientePedido = clientes[Random.Range(0, clientes.Length)];

        if (esBulk)
        {
            string[] cajasBulk = { "Tabaco", "Tercio", "Cuarto", "Octavo" };
            cajaCorrecta = cajasBulk[Random.Range(0, cajasBulk.Length)];
            switch (cajaCorrecta)
            {
                case "Tabaco": cantidadPedido = 10; capacidadCaja = 10; break;
                case "Tercio": cantidadPedido = 6; capacidadCaja = 6; break;
                case "Cuarto": cantidadPedido = 4; capacidadCaja = 4; break;
                case "Octavo": cantidadPedido = 2; capacidadCaja = 2; break;
            }
        }
        else
        {
            string[] cajasDoc = { "Full", "Tabaco", "Tercio", "Cuarto", "Octavo" };
            cajaCorrecta = cajasDoc[Random.Range(0, cajasDoc.Length)];
            switch (cajaCorrecta)
            {
                case "Full": cantidadPedido = 30; capacidadCaja = 35; break;
                case "Tabaco": cantidadPedido = 16; capacidadCaja = 16; break;
                case "Tercio": cantidadPedido = 10; capacidadCaja = 10; break;
                case "Cuarto": cantidadPedido = 10; capacidadCaja = 10; break;
                case "Octavo": cantidadPedido = 4; capacidadCaja = 4; break;
            }
        }

        textoPedido.text = "PEDIDO: " + (esBulk ? "Bulk " : "Bouquets ") + "x" + cantidadPedido + " | " + clientePedido;
        textoProgreso.text = "Productos: 0/" + cantidadPedido;
        textoResultado.text = "Elige la caja";
        textoResultado.color = Color.white;

        ActivarBotonesCaja();
        DesactivarBotonesTapa();
    }

    void SpawnProductos()
    {
        for (int i = 0; i < cantidadPedido; i++)
        {
            Vector3 pos = zonaProductos.position + new Vector3((i % 4) * 0.7f, -(i / 4) * 0.8f, 0);
            GameObject prod = Instantiate(prefabProducto, pos, Quaternion.identity);
            prod.GetComponent<SpriteRenderer>().sprite = esBulk ? spriteBulk : spriteBouquet;

            // Si tu prefab tiene el script ProductoEmpaque, le pasamos el dato
            ProductoEmpaque pe = prod.GetComponent<ProductoEmpaque>();
            if (pe != null) pe.esBulk = esBulk;

            productosEnEscena.Add(prod);
        }
    }

    void MostrarImagenCaja(string caja)
    {
        if (caja == "Full") imagenCaja.sprite = spriteFull;
        else if (caja == "Tabaco") imagenCaja.sprite = spriteTabaco;
        else if (caja == "Tercio") imagenCaja.sprite = spriteTercio;
        else if (caja == "Cuarto") imagenCaja.sprite = spriteCuarto;
        else if (caja == "Octavo") imagenCaja.sprite = spriteOctavo;
    }

    Sprite ObtenerSpriteTapa(string caja)
    {
        return caja switch
        {
            "Full" => tapFull,
            "Tabaco" => tapTabaco,
            "Tercio" => tapTercio,
            "Cuarto" => tapCuarto,
            "Octavo" => tapOctavo,
            _ => tapTabaco
        };
    }

    void ActualizarContadores()
    {
        if (Contador_Full != null) Contador_Full.text = "Full: " + contFull;
        if (Contador_Tabaco != null) Contador_Tabaco.text = "Tabaco: " + contTabaco;
        if (Contador_Tercio != null) Contador_Tercio.text = "Tercio: " + contTercio;
        if (Contador_Cuarto != null) Contador_Cuarto.text = "Cuarto: " + contCuarto;
        if (Contador_Octavo != null) Contador_Octavo.text = "Octavo: " + contOctavo;
    }
}