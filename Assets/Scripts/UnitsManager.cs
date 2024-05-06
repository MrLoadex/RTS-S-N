using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : Singleton<UnitsManager>
{

    public List<UnidadMovilColocada> unidadMovilColocadas { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        unidadMovilColocadas = new List<UnidadMovilColocada>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            // Si es una sola unidad
            Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(movePosition, out var hitInfo))
            {
                // Mover unidades
                if(SelectManager.Instance.UnidadesMovilesSeleccionadas.Count == 1)
                {
                    SelectManager.Instance.UnidadesMovilesSeleccionadas[0].MoverUnidad(hitInfo.point);
                }
                // Si son muchas
                else
                {
                    MoverMultiplesUnidades(SelectManager.Instance.UnidadesMovilesSeleccionadas, hitInfo.point);
                }
            } 
            
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
        unidadMovilColocadas.Add(unidadNueva);
    }

    #endregion
}
