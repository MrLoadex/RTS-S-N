using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : Singleton<UnitsManager>
{

    public List<UnidadMovilColocada> UnidadMovilColocadas { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        UnidadMovilColocadas = new List<UnidadMovilColocada>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            // Si es una sola unidad
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(ray, out var hit))
            {
                // Intentar obtener componente de unidad colocada del objeto donde impacto el rayo
                var unidadObjetivo = hit.collider.GetComponent<UnidadColocada>();

                // Si impacta contra una unidad enemiga
                if (unidadObjetivo != null && unidadObjetivo.Equipo != Team.Aliado)
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
        }
        // Si son muchas
        else
        {
            MoverMultiplesUnidades(SelectManager.Instance.UnidadesMovilesSeleccionadas, hit.point);
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
        }
    }

    public void CambiarComportamiento(string comportamientoString)
    {
        //Obtener las unidades moviles seleccionadas
        var unidadesMovilesSeleccionadas = SelectManager.Instance.UnidadesMovilesSeleccionadas;

        // Intenta convertir el string a un valor enum
        if (Comportamiento.TryParse(comportamientoString, out Comportamiento comportamiento))
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
        AccionDeEdificio.EventoNuevaUnidad += AgregarUnidadMovil;
    }

    private void OnDisable() 
    {
        AccionDeEdificio.EventoNuevaUnidad -= AgregarUnidadMovil;      
    }

    void AgregarUnidadMovil(UnidadMovilColocada unidadNueva)
    {
        UnidadMovilColocadas.Add(unidadNueva);
    }

    #endregion
}
