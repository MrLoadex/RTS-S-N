using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BuilderManager : Singleton<BuilderManager>
{

    [SerializeField] private EdificioBlueprint[] edificiosList;

    [Header("Colores")]
    [SerializeField] private Color colorLoock = new Color(1, 0 , 0 , 0.5f);
    [SerializeField] private Color colorUnloock = new Color(0, 1 , 0 , 0.5f);

    private EdificioBlueprint actualEdificioBlueprint;
    private bool colocandoObjeto = false;
    

    //TESTEOOOO
    List<Color> coloresOriginales;


    // Actualizar Los Edificios Disponibles
    private void Start() 
    {
        UIManager.Instance.InicializarPanelConstruccion(edificiosList);
    }
    private void Update() 
    {
        if(actualEdificioBlueprint == null) return;

        if(Input.GetKeyDown(KeyCode.A))
        {
            coloresOriginales = ObtenerColoresOriginales(actualEdificioBlueprint.EdificioColocadoPrefab.gameObject);
            CambiarColores(actualEdificioBlueprint.EdificioColocadoPrefab.gameObject, colorUnloock);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            DevolverColores(actualEdificioBlueprint.EdificioColocadoPrefab.gameObject,coloresOriginales);
        }
    }

    public void ColocarObjeto(EdificioBlueprint edificio)
    {
        //Comprobacion de seguridad
        if (edificio == null) return;
        //Setear edificio actual en construccion
        actualEdificioBlueprint = edificio;
        // Declaracion de variables
        bool recursosSuficientes = RecursosManager.Instance.ComprobarConstruccion(edificio);
        List<Color> coloresOriginales = new List<Color>();

        // Setear el modo construccion
        colocandoObjeto = true;

        // Comprobar si hay recursos suficientes
        if (!recursosSuficientes){return; }

        // Obtener el color original
        coloresOriginales.AddRange( ObtenerColoresOriginales(edificio.EdificioColocadoPrefab.gameObject));

        foreach (Color item in coloresOriginales)
        {
            Debug.Log(item);
        }

    }

    private List<Color> ObtenerColoresOriginales(GameObject edificioOBJ)
    {
        List<Color> colores = new List<Color>();

        // Obtiene el componente de renderizado del objeto
        Renderer renderer = edificioOBJ.GetComponent<Renderer>();

        if (renderer != null)
        {
            // Obtiene el material utilizado para renderizar el objeto
            Material material = renderer.sharedMaterial;

            // Obtiene el color original del material y lo agrega a la lista
            colores.Add(material.color);
        }

        // Recorre los hijos del objeto actual
        foreach (Transform child in edificioOBJ.transform)
        {
            // Obtiene los colores originales de los hijos recursivamente
            List<Color> coloresHijo = ObtenerColoresOriginales(child.gameObject);

            // Agrega los colores originales de los hijos a la lista principal
            colores.AddRange(coloresHijo);
        }

        return colores;
    }

    private void CambiarColores(GameObject edificioOBJ, Color color)
    {
        // Obtiene el componente de renderizado del objeto
        Renderer renderer = edificioOBJ.GetComponent<Renderer>();
        renderer.sharedMaterial.color = color;

        // Recorre los hijos del objeto actual
        foreach (Transform child in edificioOBJ.transform)
        {
            // Les cambia el color a verde
            CambiarColores(child.gameObject, color);
        }
    }

    private void DevolverColores(GameObject edificioOBJ, List<Color> coloresOriginales)
    {
        // Obtiene los componentes de renderizado del objeto y sus hijos
        Renderer[] renderers = edificioOBJ.GetComponentsInChildren<Renderer>();

        // Itera sobre todos los componentes de renderizado
        for (int i = 0; i < renderers.Length; i++)
        {
            // Verifica si el índice de color original existe en la lista
            if (i < coloresOriginales.Count)
            {
                // Obtiene el material del componente de renderizado
                Material material = renderers[i].sharedMaterial;

                // Restaura el color original del material
                material.color = coloresOriginales[i];
            }
            else
            {
                Debug.LogWarning("No se pudo restaurar el color original para el componente de renderizado número " + i + " debido a la falta de colores originales.");
            }
        }
    }


}
