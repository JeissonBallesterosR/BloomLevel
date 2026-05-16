using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TMP_InputField nameInputField;
    public TMP_Text bestTimeText;
    public string gameSceneName = "SampleScene"; // Cambiado para coincidir con tu Build Settings

    void Start()
    {
        Time.timeScale = 1f; // Asegurar que el tiempo no esté pausado al volver
        ActualizarRecordVisual();
    }

    public void ActualizarRecordVisual()
    {
        if (DataManager.Instance != null && DataManager.Instance.bestTime > 0)
        {
            bestTimeText.text = "Mejor Tiempo: " + DataManager.Instance.bestTime.ToString("F2") + "s";
        }
        else
        {
            bestTimeText.text = "Sin récords";
        }
    }

    public void PlayGame()
    {
        if (DataManager.Instance != null)
        {
            // Si el campo está vacío, ponemos "Operador" por defecto
            string nombre = string.IsNullOrEmpty(nameInputField.text) ? "Operador" : nameInputField.text;
            DataManager.Instance.playerName = nombre;
        }
        SceneManager.LoadScene(gameSceneName);
    }
}