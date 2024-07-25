using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PartyManager : Singleton<PartyManager>
{

    [SerializeField] private List<UnidadMovilColocada> unidadesMovilesAliadas;
    [SerializeField] private List<UnidadMovilColocada> unidadesMovilesEnemigas;
    [SerializeField] private PlanillaPuntajes planillaFacil;
    [SerializeField] private PlanillaPuntajes planillaNormal;
    [SerializeField] private PlanillaPuntajes planillaDificil;
    
    [Header("Datos de la partida en curso")]
    [SerializeField] private DatosPartida datosPartida;

    private PlanillaPuntajes planillaPuntajesActual;

    private Dificultad dificultadActual;
    private Puntaje actualPuntaje;

    void Start()
    {
        // Configurar partida
        dificultadActual = datosPartida.dificultad;
        actualPuntaje.UserName = datosPartida.userName;

        // Si no hay unidades creadas por el user entonces crea la lista para que no sea nula
        if (unidadesMovilesAliadas == null) unidadesMovilesAliadas = new List<UnidadMovilColocada>();

        //Seleccionar la planilla de puntos correcta
        SeleccionarPlanillaCorrecta();

        // Configurar OleadasManager con la dificultad correcta
        OleadasManager.Instance.SetDificultad(dificultadActual);
    }

    private void SeleccionarPlanillaCorrecta()
    {
        switch (dificultadActual)
        {
            case Dificultad.Facil:
                planillaPuntajesActual = planillaFacil;
                break;
            case Dificultad.Normal:
                planillaPuntajesActual = planillaNormal;
                break;
            case Dificultad.Dificil:
                planillaPuntajesActual = planillaDificil;
                break;
            default:
                break;
        }
    }

    void Update()
    {

    }

    public void SumarPuntoPorOleadaDerrotada()
    {
        actualPuntaje.Valor ++;
    }

    public void MostrarPanelDerrota()
    {
        // Agregar el puntaje si se pudo
        planillaPuntajesActual.AddPuntaje(actualPuntaje);
        int posicion = planillaPuntajesActual.GetPosicion(actualPuntaje);

        // Llamar a la UIManager para que muestre el cartel de derrota (asumiendo que tienes un UIManager)
        UIManager.Instance.AbrirPanelDerrota();
        UIManager.Instance.ConfigurarPanelDerrota(actualPuntaje.UserName, actualPuntaje.Valor, posicion ); 
    }

    public void FinalizarPartida()
    {
        SceneManager.LoadScene("MenuPrincipal"); // Reemplaza "MenuPrincipal" con el nombre real de tu escena de menú principal
    }
 
    #region Eventos

    private void OnEnable() 
    {
        AccionDeEdificio.EventoNuevaUnidadMovil += ResponderEventoNuevaUnidadMovil;
        UnidadVida.EventoUnidadDerrotada += ResponderEventoUnidadDerrotada;
    }

    private void OnDisable() 
    {
        AccionDeEdificio.EventoNuevaUnidadMovil -= ResponderEventoNuevaUnidadMovil;      
        UnidadVida.EventoUnidadDerrotada -= ResponderEventoUnidadDerrotada;
    }

    void ResponderEventoNuevaUnidadMovil(UnidadMovilColocada unidadNueva)
    {
        if (unidadNueva.Equipo == Team.Aliado)
        {
            unidadesMovilesAliadas.Add(unidadNueva);
        }
        else
        {
            unidadesMovilesEnemigas.Add(unidadNueva);
        }
    }

    void ResponderEventoUnidadDerrotada(UnidadColocada unidadEliminada)
    {
        UnidadMovilColocada unidadMovilEliminada = unidadEliminada.gameObject.GetComponent<UnidadMovilColocada>();
        if(unidadMovilEliminada == null) return;

        // Busca la unidad y si esta en la lista la elimina
        if (unidadesMovilesAliadas.Contains(unidadMovilEliminada))
        {
            unidadesMovilesAliadas.Remove(unidadMovilEliminada);
            if (unidadesMovilesAliadas.Count == 0) MostrarPanelDerrota();
        }
        else if (unidadesMovilesEnemigas.Contains(unidadMovilEliminada))
        {
            unidadesMovilesEnemigas.Remove(unidadMovilEliminada);
        }
    }

    #endregion
}
