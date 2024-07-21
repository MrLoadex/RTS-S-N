using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUnitsManager : Singleton<SelectUnitsManager>
{

    [SerializeField] private GameObject objetoReferMovPrefab;

    private Camera cam;
    private RaycastHit hit;
    private List<UnidadMovilColocada> unidadesMovilesSeleccionadas; // Se almacenan las unidades moviles seleccionadas para poder moverlas
    private EdificioColocado edificioColocadoSeleccionado;

    public static Action<UnidadMovilColocada> EventoUnidadControladaPorUsuario;

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
        else if(Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(ray, out var hit))
            {
                // Intentar obtener componente de unidad colocada del objeto donde impacto el rayo
                var unidadObjetivo = hit.collider.GetComponent<UnidadColocada>();

                // Si impacta contra una unidad enemiga
                if (unidadObjetivo != null)
                {
                    if (unidadObjetivo.Equipo == Team.Enemigo)
                    {
                        // Atacar
                        Atacar(unidadesMovilesSeleccionadas ,unidadObjetivo);

                    }
                    else if (unidadObjetivo.GetComponent<EdificioColocado>() != null || unidadObjetivo.GetComponent<RecursoColocado>() != null)
                    {
                        // Seleccionar a la unidad como objetivo
                        unidadObjetivo.SeleccionarComoObjetivo();
                        MoverUnidad(hit);
                    }
                }
                else
                {
                    // Instanciar objeto de referencia de movimiento
                    var objetoReferMov = Instantiate(objetoReferMovPrefab, hit.point, Quaternion.identity);
                    objetoReferMov.transform.Rotate(new Vector3(90,0,0)); 
                    StartCoroutine(DestruirObjetoReferMov(objetoReferMov));
                    MoverUnidad(hit);
                }
            } 
            
        }
    }

    public void CambiarComportamiento(string comportamientoString)
    {
        // Intenta convertir el string a un valor enum
        if (EstadoComportamiento.TryParse(comportamientoString, out EstadoComportamiento comportamiento))
        {
            // Recorrer todas las unidades con el comportamiento adecuado
            foreach (var unidad in unidadesMovilesSeleccionadas)
            {
                unidad.CombatSystem.CambiarComportamiento(comportamiento);
            }
        }
        else
        {
            // Opcional: manejar el caso donde el string no es válido
            Debug.LogError("El valor proporcionado no es un comportamiento válido: " + comportamientoString);
        }
    }

    void Atacar(List<UnidadMovilColocada> unidadesMovilesSelect, UnidadColocada unidadObjetivo)
    {
        if (unidadObjetivo.Equipo == Team.Neutral) return;
        foreach (var unidad in unidadesMovilesSelect)
        {
            unidad.CombatSystem.ComenzarAtaque(unidadObjetivo.VidaSystem);
        }
    }

    void MoverUnidad(RaycastHit hit)
    {
        // Mover unidades
        if(unidadesMovilesSeleccionadas.Count == 1)
        {
           unidadesMovilesSeleccionadas[0].MoverUnidad(hit.point);
            // Llamar al evento para que la unidad sepa que esta siendo controlada por el usuario y tenga prioridad este movimiento
            EventoUnidadControladaPorUsuario?.Invoke(unidadesMovilesSeleccionadas[0]);
        }
        // Si son muchas
        else
        {
            MoverMultiplesUnidades(unidadesMovilesSeleccionadas, hit.point);
        }
    }

    private void MoverMultiplesUnidades(List<UnidadMovilColocada> unidadesMovilesSelect, Vector3 posicion)
    {
        int totalUnidades = unidadesMovilesSelect.Count;
        float radio = 1.0f; // Puedes ajustar este valor para aumentar o disminuir el espacio entre unidades
        float anguloIncremento = 360f / totalUnidades;

        for (int i = 0; i < totalUnidades; i++)
        {
            float anguloEnRad = (anguloIncremento * i) * Mathf.Deg2Rad; // Convertir grados a radianes
            Vector3 posicionModificada = new Vector3(
                posicion.x + Mathf.Cos(anguloEnRad) * radio, // Coordenada X
                posicion.y, // Coordenada Y, asumiendo movimiento en un plano horizontal
                posicion.z + Mathf.Sin(anguloEnRad) * radio  // Coordenada Z
            );

            unidadesMovilesSelect[i].MoverUnidad(posicionModificada);
            // Llamar al evento para que la unidad sepa que esta siendo controlada por el usuario y tenga prioridad este movimiento
            EventoUnidadControladaPorUsuario?.Invoke(unidadesMovilesSeleccionadas[i]);
        }
    }

    IEnumerator DestruirObjetoReferMov(GameObject objetoReferMov)
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(objetoReferMov);
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
                edificioColocadoSeleccionado = edificioSelect;
                edificioColocadoSeleccionado.SeleccionarUnidad();
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
                UIManager.Instance.ActualizarUIUnidadesSeleccionadas(unidadesMovilesSeleccionadas);
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
        // Deseleccionar el edificio
        edificioColocadoSeleccionado?.DeseleccionarUnidad();

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
        if (unidadesMovilesSeleccionadas.Contains(unidadMovil)) unidadesMovilesSeleccionadas.Remove(unidadMovil);
        UIManager.Instance.ActualizarUIUnidadesSeleccionadas(unidadesMovilesSeleccionadas);
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