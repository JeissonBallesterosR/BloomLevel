using UnityEngine;
using TMPro;

public class HUDEstacion : MonoBehaviour
{
    public TextMeshProUGUI textoNombreHUD;
    public TextMeshProUGUI textoTiempoHUD;

    private float cronometro = 0f;

    void Start()
    {
     
        if (DataManager.Instance != null)
        {
            textoNombreHUD.text = "Operador: " + DataManager.Instance.playerName;
        }
    }

    void Update()
    {
      
        cronometro += Time.deltaTime;
        textoTiempoHUD.text = "Tiempo: " + cronometro.ToString("F2") + "s";
    }


    public void GuardarResultadoFinal()
    {
        if (DataManager.Instance != null)
        {
    
            DataManager.Instance.SaveData(cronometro);
        }
    }
}