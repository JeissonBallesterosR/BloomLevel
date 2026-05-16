using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public string playerName;
    public float bestTime; // Este valor se leerá en el MainMenu al final

    void Awake()
    {
        // PATRÓN SINGLETON: Esto evita que el objeto se destruya entre escenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // CLAVE: Mantiene los datos vivos
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Función para actualizar el récord desde cualquier estación
    public void SaveData(int puntajeFinal)
    {
        // Aquí puedes convertir el puntaje a tiempo o guardar el puntaje como récord
        if (puntajeFinal > bestTime)
        {
            bestTime = puntajeFinal;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            PlayerPrefs.Save();
        }
    }

    internal void SaveData(float cronometro)
    {
        throw new NotImplementedException();
    }
}