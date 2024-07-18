using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : Singleton<SelectManager>
{
    private Camera cam;
    private RaycastHit hit;
    private List<UnidadMovilColocada> unidadesMovilesSeleccionadas; // Se almacenan las unidades moviles seleccionadas para poder moverlas
    
    public List<UnidadMovilColocada> UnidadesMovilesSeleccionadas => unidadesMovilesSeleccionadas;
    
    private void Start()
    {
        // Obtener la cámara principal
        cam = Camera.main;
        unidadesMovilesSeleccionadas = new List<UnidadMovilColocada>();
        
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            //Se selecciona la uniad que este justo en ese lugar
            SeleccionarUnidad();
        }


    }

    private void SeleccionarUnidad()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            // Intentar obtener componente de unidad colocada del objeto donde impacto el rayo
            EdificioColocado edificioSelect = hit.collider.GetComponent<EdificioColocado>();
            UnidadMovilColocada unidadMovilSelect = hit.collider.GetComponent<UnidadMovilColocada>();

            if (edificioSelect != null)
            {
                UnselectAll();
                edificioSelect.SeleccionarUnidad();
            }
            else if (unidadMovilSelect != null && unidadMovilSelect.Equipo == Team.Aliado)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    // Lógica para selección de múltiples unidades móviles
                    ToggleUnidadMovil(unidadMovilSelect);
                }
                else
                {
                    // Seleccionar solo esta unidad
                    UnselectAll();
                    unidadesMovilesSeleccionadas.Add(unidadMovilSelect);
                    unidadMovilSelect.SeleccionarUnidad();
                }
                UIManager.Instance.ActualizarUIUnidadesSeleccionadas(UnidadesMovilesSeleccionadas);
            }
        }
    }

    private void ToggleUnidadMovil(UnidadMovilColocada unidadMovil)
    {
        if (unidadesMovilesSeleccionadas.Contains(unidadMovil))
        {
            // Avisarle a la unidad que fue deseleccionada
            unidadMovil?.DeseleccionarUnidad();
            // Quitar de la lista
            unidadesMovilesSeleccionadas.Remove(unidadMovil);
        }
        else
        {
            // Avisarle a la unidad que fue seleccionada
            unidadMovil.SeleccionarUnidad();
            // Quitar de la lista
            unidadesMovilesSeleccionadas.Add(unidadMovil);
        }
    }

    private void UnselectAll()
    {
        // Avisarle a las unidades que ya no estan seleccionadas
        foreach (var unidad in unidadesMovilesSeleccionadas)
        {
            unidad.DeseleccionarUnidad();
        }
        unidadesMovilesSeleccionadas.Clear();
    }
    public void UnselectUnit(UnidadMovilColocada unidad)
    {
        // Comprobar si la unidad esta seleccionada
        if (unidadesMovilesSeleccionadas.Contains(unidad))
        {
            if (!unidadesMovilesSeleccionadas.Contains(unidad) || unidad == null) return;

            // Deseleccionarla, actualizar la interface y finalizar el bucle
            // Avisarle a la unidad que fue deseleccionada
            unidad.DeseleccionarUnidad();
            unidadesMovilesSeleccionadas.Remove(unidad);
            UIManager.Instance.ActualizarUIUnidadesSeleccionadas(unidadesMovilesSeleccionadas);
        }
    }

    #region Eventos

    void ResponderEventoUnidadDerrotada(UnidadColocada  unidadColocada)
    {
        var unidadMovil = unidadColocada.gameObject.GetComponent<UnidadMovilColocada>();
        if (unidadMovil == null) return;
        // Si la unidad es movil y estaba seleccionada la elimina de la seleccion
        if (UnidadesMovilesSeleccionadas.Contains(unidadMovil)) unidadesMovilesSeleccionadas.Remove(unidadMovil);
        UIManager.Instance.ActualizarUIUnidadesSeleccionadas(UnidadesMovilesSeleccionadas);
    }
    private void OnEnable() 
    {
        UnidadVida.EventoUnidadDerrotada += ResponderEventoUnidadDerrotada;
    }

    private void OnDisable() 
    {
        UnidadVida.EventoUnidadDerrotada -= ResponderEventoUnidadDerrotada;   
    }
    #endregion
}