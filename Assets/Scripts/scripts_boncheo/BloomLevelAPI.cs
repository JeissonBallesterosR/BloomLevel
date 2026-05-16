// BloomLevelAPI.cs — Script para Unity
// Pega este archivo en Assets/_Boncheo/Scripts/Core/
// Llama a los métodos desde BulkBuilder, ClasificacionManager, etc.

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BloomLevel.Network
{
    // ── DTOs que Unity envía a la API ─────────────────────────────────────────
    [Serializable] public class CrearJugadorRequest  { public string nombre; public string apellido; public string email; public string codigoOperario; }
    [Serializable] public class IniciarSesionRequest { public string jugadorId; public int nivelJugado = 1; }
    [Serializable] public class FinalizarSesionRequest { public string sesionId; public int puntajeTotal; public float precisionTotal; public int erroresTotal; public int duracionSeg; }
    [Serializable] public class BoncheoRequest { public string sesionId; public string jugadorId; public string tipoProducto; public int grado; public string apertura; public string cliente; public string laminaCorrecta; public string laminaElegida; public int rosasColocadas; public int totalRosas; }
    [Serializable] public class ClasificacionRequest { public string sesionId; public string jugadorId; public float longitudCm; public string apertura; public bool tieneDefecto; public bool estaTorcida; public string gradoCorrecto; public string gradoElegido; }

    // ── Respuestas de la API ──────────────────────────────────────────────────
    [Serializable] public class JugadorResponse  { public string id; public string nombre; public int nivelActual; }
    [Serializable] public class SesionResponse   { public string id; public string jugadorId; }
    [Serializable] public class ResultadoResponse { public bool esCorrecto; public int puntosGanados; public string mensaje; public string codigoBonchador; }

    public class BloomLevelAPI : MonoBehaviour
    {
        // ── Cambia esta URL cuando publiques en somee.com ─────────────────────
        [Header("URL de la API (somee.com)")]
        public string apiUrl = "https://TU_USUARIO.somee.com";

        public static BloomLevelAPI Instance { get; private set; }

        // IDs de sesión activa (se guardan al iniciar sesión)
        public string JugadorId { get; private set; } = "";
        public string SesionId  { get; private set; } = "";

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ── JUGADOR ───────────────────────────────────────────────────────────

        public void CrearJugador(string nombre, string codigo, Action<JugadorResponse> onOk)
        {
            var body = new CrearJugadorRequest { nombre = nombre, codigoOperario = codigo };
            StartCoroutine(Post<JugadorResponse>("api/jugadores", body, r => {
                JugadorId = r.id;
                onOk?.Invoke(r);
            }));
        }

        // ── SESIÓN ────────────────────────────────────────────────────────────

        public void IniciarSesion(int nivel, Action<SesionResponse> onOk)
        {
            var body = new IniciarSesionRequest { jugadorId = JugadorId, nivelJugado = nivel };
            StartCoroutine(Post<SesionResponse>("api/sesiones/iniciar", body, r => {
                SesionId = r.id;
                onOk?.Invoke(r);
            }));
        }

        public void FinalizarSesion(int puntaje, float precision, int errores, int duracion)
        {
            var body = new FinalizarSesionRequest {
                sesionId       = SesionId,
                puntajeTotal   = puntaje,
                precisionTotal = precision,
                erroresTotal   = errores,
                duracionSeg    = duracion
            };
            StartCoroutine(Put("api/sesiones/finalizar", body));
        }

        // ── BONCHEO ───────────────────────────────────────────────────────────

        public void EnviarResultadoBoncheo(
            string tipo, int grado, string apertura,
            string cliente, string lamina, int rosasColocadas,
            int totalRosas, Action<ResultadoResponse> onOk)
        {
            var body = new BoncheoRequest {
                sesionId       = SesionId,
                jugadorId      = JugadorId,
                tipoProducto   = tipo,
                grado          = grado,
                apertura       = apertura,
                cliente        = cliente,
                laminaCorrecta = lamina,
                laminaElegida  = lamina,
                rosasColocadas = rosasColocadas,
                totalRosas     = totalRosas
            };
            StartCoroutine(Post<ResultadoResponse>("api/boncheo", body, onOk));
        }

        // ── CLASIFICACIÓN ─────────────────────────────────────────────────────

        public void EnviarResultadoClasificacion(
            float longitud, string apertura, bool defecto,
            bool torcida, string gradoCorrecto, string gradoElegido,
            Action<ResultadoResponse> onOk)
        {
            var body = new ClasificacionRequest {
                sesionId      = SesionId,
                jugadorId     = JugadorId,
                longitudCm    = longitud,
                apertura      = apertura,
                tieneDefecto  = defecto,
                estaTorcida   = torcida,
                gradoCorrecto = gradoCorrecto,
                gradoElegido  = gradoElegido
            };
            StartCoroutine(Post<ResultadoResponse>("api/clasificacion", body, onOk));
        }

        // ── HTTP Helpers ──────────────────────────────────────────────────────

        IEnumerator Post<T>(string endpoint, object body, Action<T> onOk)
        {
            string json = JsonUtility.ToJson(body);
            var req = new UnityWebRequest($"{apiUrl}/{endpoint}", "POST");
            req.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                onOk?.Invoke(JsonUtility.FromJson<T>(req.downloadHandler.text));
            else
                Debug.LogError($"[BloomLevelAPI] POST {endpoint}: {req.error}");
        }

        IEnumerator Put(string endpoint, object body)
        {
            string json = JsonUtility.ToJson(body);
            var req = new UnityWebRequest($"{apiUrl}/{endpoint}", "PUT");
            req.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                Debug.LogError($"[BloomLevelAPI] PUT {endpoint}: {req.error}");
        }
    }
}
