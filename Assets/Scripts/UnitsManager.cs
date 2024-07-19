using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : Singleton<UnitsManager>
{
    [SerializeField] private GameObject objetoReferMovPrefab;

    public static Action<UnidadMovilColocada> EventoUnidadControladaPorUsuario;

    [SerializeField] private List<UnidadMovilColocada> unidadesMovilesColocadas;
    public List<UnidadMovilColocada> UnidadesMovilesColocadas => unidadesMovilesColocadas;

    // Start is called before the first frame update
    void Start()
    {
        // Si no hay unidades seleccionadas por el user entonces crea la lista para que no sea nula
        if (unidadesMovilesColocadas == null) unidadesMovilesColocadas = new List<UnidadMovilColocada>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(ray, out var hit))
            {
                // Intentar obtener componente de unidad colocada del objeto donde impacto el rayo
                var unidadObjetivo = hit.collider.GetComponent<UnidadColocada>();

                // Si impacta contra una unidad enemiga
                if (unidadObjetivo != null && (unidadObjetivo.Equipo == Team.Enemigo))
                {
                    // Atacar
                    Atacar(SelectManager.Instance.UnidadesMovilesSeleccionadas ,unidadObjetivo);
                }
                else
                {
                    MoverUnidad(hit);
                }
            } 
            
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
        if(SelectManager.Instance.UnidadesMovilesSeleccionadas.Count == 1)
        {
            SelectManager.Instance.UnidadesMovilesSeleccionadas[0].MoverUnidad(hit.point);
            // Llamar al evento para que la unidad sepa que esta siendo controlada por el usuario y tenga prioridad este movimiento
            EventoUnidadControladaPorUsuario?.Invoke(SelectManager.Instance.UnidadesMovilesSeleccionadas[0]);
        }
        // Si son muchas
        else
        {
            MoverMultiplesUnidades(SelectManager.Instance.UnidadesMovilesSeleccionadas, hit.point);
        }

        // Instanciar objeto de referencia de movimiento
        var objetoReferMov = Instantiate(objetoReferMovPrefab, hit.point, Quaternion.identity);
        objetoReferMov.transform.Rotate(new Vector3(90,0,0)); 
        StartCoroutine(DestruirObjetoReferMov(objetoReferMov));
    }

    IEnumerator DestruirObjetoReferMov(GameObject objetoReferMov)
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(objetoReferMov);
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
            EventoUnidadControladaPorUsuario?.Invoke(SelectManager.Instance.UnidadesMovilesSeleccionadas[i]);
        }
    }

    public void CambiarComportamiento(string comportamientoString)
    {
        //Obtener las unidades moviles seleccionadas
        var unidadesMovilesSeleccionadas = SelectManager.Instance.UnidadesMovilesSeleccionadas;

        // Intenta convertir el string a un valor enum
        if (Estado.TryParse(comportamientoString, out Estado comportamiento))
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

    #region Eventos

    private void OnEnable() 
    {
        AccionDeEdificio.EventoNuevaUnidadMovil += AgregarUnidadMovil;
        UnidadVida.EventoUnidadDerrotada += EliminarUnidadMovil;
    }

    private void OnDisable() 
    {
        AccionDeEdificio.EventoNuevaUnidadMovil -= AgregarUnidadMovil;      
        UnidadVida.EventoUnidadDerrotada -= EliminarUnidadMovil;
    }

    void AgregarUnidadMovil(UnidadMovilColocada unidadNueva)
    {
        UnidadesMovilesColocadas.Add(unidadNueva);
    }

    void EliminarUnidadMovil(UnidadColocada unidadEliminada)
    {
        UnidadMovilColocada unidadMovilEliminada = unidadEliminada.gameObject.GetComponent<UnidadMovilColocada>();
        if(unidadMovilEliminada == null) return;
        // Busca la unidad y si esta en la lista la elimina
        if (UnidadesMovilesColocadas.Contains(unidadMovilEliminada))
        {
            UnidadesMovilesColocadas.Remove(unidadMovilEliminada);
        }
    }

    #endregion
}
