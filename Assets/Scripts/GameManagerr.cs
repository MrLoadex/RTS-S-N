using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerr : Singleton<GameManagerr>
{
    private Camera cam;
    private RaycastHit hit;

    private void Start()
    {
        // Obtener la cámara principal
        cam = Camera.main;
    }

    private void Update()
    {
        SeleccionarUnidad();
    }
    
    private void SeleccionarUnidad()
    {
        // Si se presiona el botón izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Lanzar un rayo desde la posición del mouse en la pantalla
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Si el rayo choca con algo
            if (Physics.Raycast(ray, out hit))
            {
                // Verificar si el objeto con el que chocó tiene un componente de tipo EdificioColocado
                EdificioColocado edificioColocado = hit.collider.GetComponent<EdificioColocado>();
                if (edificioColocado != null)
                {
                    // Llamar a la función SeleccionarUnidad del objeto EdificioColocado
                    edificioColocado.SeleccionarUnidad();
                }
            }
        }
    }
}