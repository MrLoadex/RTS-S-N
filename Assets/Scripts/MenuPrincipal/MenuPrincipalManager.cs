using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipalManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject panelDificultad;
    [SerializeField] private GameObject panelPuntajesSelectDificultad;
    [SerializeField] private GameObject panelPuntajes;
    [SerializeField] private GameObject panelJugar;

    [Header("Punajes Config")]
    [SerializeField] private TextMeshProUGUI tituloDificultadTMP;
    [SerializeField] private Transform contenedorPuntajes;
    [SerializeField] private PuntajeTarjeta puntajeTarjetaPrefab;

    [Header("Planillas Puntajes y Dificultad")]
    [SerializeField] private PlanillaPuntajes planillaFacil;
    [SerializeField] private PlanillaPuntajes planillaNormal;
    [SerializeField] private PlanillaPuntajes planillaDificil;

    [Header("Configuracion de partida")]
    [SerializeField] private TextMeshProUGUI userNameTMP;
    [SerializeField] DatosPartida datosPartidaSO;

    [Header("Tutorial")]
    [SerializeField] private GameObject panelTutorial;

    private string userName;
    private Dificultad dificultad = Dificultad.Normal;

    public void AbrirCerrarPanelTutorial(bool estado)
    {
        panelTutorial.SetActive(estado);
    }

    public void AbrirPanelPrincipal ()
    {
        CerrarTodosPaneles();
        panelPrincipal.SetActive(true);
    }   
    
    public void AbrirPanelDificultad()
    {
        CerrarTodosPaneles();
        panelDificultad.SetActive(true);
    }
    
    public void AbrirPanelPuntajesSelectDificultad()
    {
        CerrarTodosPaneles();
        panelPuntajesSelectDificultad.SetActive(true);
    }

    public void AbrirPanelJugar()
    {
        CerrarTodosPaneles();
        panelJugar.SetActive(true);
    }

    public void AbrirPanelPauntajes(string dificultadString)
    {
        CerrarTodosPaneles();
        panelPuntajes.SetActive(true);
        // Intenta convertir el string a un valor enum
        if (Dificultad.TryParse(dificultadString, out Dificultad dificultad))
        {
            switch (dificultad)
            {
                case Dificultad.Facil:
                    tituloDificultadTMP.text = "PUNTAJES FACIL";
                    LlenarTablaPuntajes(planillaFacil);
                    break;
                case Dificultad.Normal:
                    tituloDificultadTMP.text = "PUNTAJES NORMAL";
                    LlenarTablaPuntajes(planillaNormal);
                    break;
                case Dificultad.Dificil:
                    tituloDificultadTMP.text = "PUNTAJES DIFICIL";
                    LlenarTablaPuntajes(planillaDificil);
                    break;
                default:
                    break;
            }
        }
        else
        {
            // Opcional: manejar el caso donde el string no es válido
            Debug.LogError("El valor proporcionado no es una dificultad válida: " + dificultadString);
        }
    }

    private void LlenarTablaPuntajes(PlanillaPuntajes planillaPuntajes)
    {
        // Recorre todos los hijos del padre y los destruye
        for (int i = contenedorPuntajes.childCount - 1; i >= 0; i--)
        {
            // Destruye el hijo actual
            Destroy(contenedorPuntajes.GetChild(i).gameObject);
        }

        // Llenar nuevamente el contenedor
        foreach (var puntaje in planillaPuntajes.Puntajes)
        {
            //crear la tarjeta
            PuntajeTarjeta puntajeTarjetaActual = Instantiate(puntajeTarjetaPrefab,contenedorPuntajes);
            //configurar la tarjeta
            puntajeTarjetaActual.configurarTarjeta(puntaje.UserName, puntaje.Valor);
        }
    }

    void CerrarTodosPaneles()
    {
       panelPrincipal.SetActive(false);
       panelDificultad.SetActive(false);
       panelPuntajesSelectDificultad.SetActive(false);
       panelPuntajes.SetActive(false);
       panelJugar.SetActive(false);
    }
    
    public void SetDificultad(string dificultadString)
    {
        if (Dificultad.TryParse(dificultadString, out Dificultad _dificultad))
        {
            dificultad = _dificultad;
        }
    }

    public void Jugar()
    {
        userName = userNameTMP.text;
        if ( userName == "...UserName...​" || userName.Length > 10 || userName.Length <= 3) return;
        datosPartidaSO.dificultad = dificultad;
        datosPartidaSO.userName = userName;
        SceneManager.LoadScene("EscenarioPrincipal");
    }

    public void SalirDelJuego()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Detener el juego en el editor
        #else
            Application.Quit(); // Cerrar la aplicación en builds
        #endif
    }
}
