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
                unidadesMovilesSeleccionadas.Clear();
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
                    unidadesMovilesSeleccionadas.Clear();
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
            unidadesMovilesSeleccionadas.Remove(unidadMovil);
        }
        else
        {
            unidadesMovilesSeleccionadas.Add(unidadMovil);
        }
    }

}