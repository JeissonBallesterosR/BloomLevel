# BloomLevel API
**API del juego BloomLevel — Flores El Tandil**
Estaciones: Clasificación | Boncheo | Empaque

---

## Endpoints disponibles

| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | /api/jugadores | Crear jugador |
| GET | /api/jugadores | Listar jugadores |
| GET | /api/jugadores/{id} | Obtener jugador por ID |
| GET | /api/jugadores/{id}/reporte | Reporte de estadísticas |
| POST | /api/sesiones/iniciar | Iniciar sesión de juego |
| PUT | /api/sesiones/finalizar | Finalizar sesión de juego |
| POST | /api/clasificacion | Registrar clasificación |
| GET | /api/clasificacion/rangos | Obtener rangos disponibles |
| POST | /api/boncheo | Registrar boncheo |
| GET | /api/boncheo/codigo/{grado} | Obtener código por grado |
| POST | /api/empaque | Registrar empaque |
| GET | /api/empaque/cajas | Obtener tipos de caja |
| GET | /api/ranking | Ranking global |

---

## Configuración para Somee

1. Abrir `appsettings.json`
2. Reemplazar la cadena de conexión:
```json
"DefaultConnection": "Server=sql5.somee.com;Database=TU_BASE;User Id=TU_USUARIO;Password=TU_CONTRASEÑA;TrustServerCertificate=True;"
```
3. En Visual Studio: clic derecho en el proyecto → **Publish** → **Folder**
4. Subir la carpeta `publish` al File Manager de Somee

---

## Configuración para Unity (C#)

```csharp
// Ejemplo de llamada desde Unity
public class BloomLevelAPIManager : MonoBehaviour
{
    private const string BASE_URL = "https://www.bloomlevelJN.somee.com";

    // 1. Crear jugador
    public IEnumerator CrearJugador(string nombre, Action<int> onSuccess)
    {
        var body = JsonUtility.ToJson(new { Nombre = nombre });
        using var req = new UnityWebRequest(BASE_URL + "/api/jugadores", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<JugadorResponse>(req.downloadHandler.text);
            onSuccess?.Invoke(response.id);
        }
    }

    // 2. Iniciar sesión
    public IEnumerator IniciarSesion(int jugadorId, Action<int> onSuccess)
    {
        var body = JsonUtility.ToJson(new { JugadorId = jugadorId });
        using var req = new UnityWebRequest(BASE_URL + "/api/sesiones/iniciar", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<SesionResponse>(req.downloadHandler.text);
            onSuccess?.Invoke(response.id);
        }
    }

    // 3. Registrar clasificacion
    public IEnumerator RegistrarClasificacion(int sesionId, string tipoFlor, string rango, bool esCorrecto, int puntaje)
    {
        var body = JsonUtility.ToJson(new { SesionId = sesionId, TipoFlor = tipoFlor, Rango = rango, EsCorrecto = esCorrecto, Puntaje = puntaje });
        using var req = new UnityWebRequest(BASE_URL + "/api/clasificacion", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
    }

    // 4. Finalizar sesion
    public IEnumerator FinalizarSesion(int sesionId, int puntajeTotal)
    {
        var body = JsonUtility.ToJson(new { SesionId = sesionId, PuntajeTotal = puntajeTotal });
        using var req = new UnityWebRequest(BASE_URL + "/api/sesiones/finalizar", "PUT");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
    }
}

[System.Serializable]
public class JugadorResponse { public int id; public string nombre; }

[System.Serializable]
public class SesionResponse { public int id; public int jugadorId; }
```

---

## Modelo Relacional

```
Jugadores (Id PK, Nombre, FechaRegistro)
    |
    └─── Sesiones (Id PK, JugadorId FK, FechaInicio, FechaFin, PuntajeTotal, Completada)
              |
              ├─── Clasificaciones (Id PK, SesionId FK, TipoFlor, Rango, EsCorrecto, Puntaje)
              ├─── Boncheos (Id PK, SesionId FK, Codigo, Grado, Cantidad, EsCorrecto, Puntaje)
              └─── Empaques (Id PK, SesionId FK, CajasEmpacadas, Puntaje)

RangosClasificacion (Id PK, Nombre, Descripcion, PuntajeMin, PuntajeMax, Color)
```
