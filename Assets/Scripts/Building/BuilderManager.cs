using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BuilderManager : Singleton<BuilderManager>
{

    [Header("Config")]
    [SerializeField] private EdificioBlueprint[] edificiosList;
    [SerializeField] LayerMask zonaPosible;

    [Header("Colores")]
    [SerializeField] private Color colorLoock = new Color(1, 0 , 0 , 0.5f);
    [SerializeField] private Color colorUnloock = new Color(0, 1 , 0 , 0.5f);


    private EdificioBlueprint actualEdificioBlueprint = null;
    private EdificioColocado edificioPorColocarInstanciado;
    List<Color> coloresOriginales = new List<Color>();

    private bool construyendo = false;
    private bool puedeSoltar = false;

    private void Start() 
    {
        // Actualizar Los Edificios Disponibles
        UIManager.Instance.InicializarPanelConstruccion(edificiosList);
    }
    
    private void Update() 
    {
        //Compronacion
        if (!construyendo || actualEdificioBlueprint == null) return;

        MoverObjetoAlMouse(edificioPorColocarInstanciado.gameObject);

        if(puedeSoltar && Input.GetButtonDown("Fire1"))
        {
            ColocarEdificio();
        }
    }

    public void PosicionarEdificio(EdificioBlueprint edificio)
    {
        //Comprobacion de seguridad
        if (edificio == null) return;

        // Declaracion de variables
        actualEdificioBlueprint = edificio;
        bool recursosSuficientes = RecursosManager.Instance.ComprobarRecursosSuficientesEdificio(actualEdificioBlueprint);
        List<Color> coloresOriginales = new List<Color>();
        
        // Comprobar si hay recursos suficientes
        if (!recursosSuficientes){return; }

        //Spawnea un nuevo Edificio
        Vector3 coordenadasSpawn = new Vector3(0, 0, 0);
        edificioPorColocarInstanciado = Instantiate(actualEdificioBlueprint.EdificioColocadoPrefab, coordenadasSpawn, Quaternion.identity);

        // Setear el modo construccion
        construyendo = true;


        // Obtener el color original
        coloresOriginales.AddRange( ObtenerColoresOriginales(edificioPorColocarInstanciado.gameObject));

        //Setear color verde
        CambiarColor(edificioPorColocarInstanciado.gameObject, colorUnloock);

        //Seguir al mouse
        MoverObjetoAlMouse(edificioPorColocarInstanciado.gameObject);

    }

    private List<Color> ObtenerColoresOriginales(GameObject edificioOBJ)
    {
        

        // Obtiene el componente de renderizado del objeto
        Renderer renderer = edificioOBJ.GetComponent<Renderer>();

        if (renderer != null)
        {
            // Obtiene el material utilizado para renderizar el objeto
            Material material = renderer.sharedMaterial;

            // Obtiene el color original del material y lo agrega a la lista
            coloresOriginales.Add(material.color);
        }

        // Recorre los hijos del objeto actual
        foreach (Transform child in edificioOBJ.transform)
        {
            // Obtiene los colores originales de los hijos recursivamente
            List<Color> coloresHijo = ObtenerColoresOriginales(child.gameObject);

            // Agrega los colores originales de los hijos a la lista principal
            coloresOriginales.AddRange(coloresHijo);
        }

        return coloresOriginales;
    }

    private void CambiarColor(GameObject edificioOBJ, Color color)
    {
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = color;

        // Obtener el componente de renderizado del objeto
        Renderer renderer = edificioOBJ.GetComponent<Renderer>();

        if (renderer != null)
        {
            // Cambiar el material
            renderer.sharedMaterial = newMaterial;

        }

        // Recorre los hijos del objeto actual
        foreach (Transform child in edificioOBJ.transform)
        {
            // Les cambia el color a verde
            CambiarColor(child.gameObject, color);
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

    private void MoverObjetoAlMouse(GameObject edificioOBJ)
    {
        if (!construyendo || edificioOBJ == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity,zonaPosible))
        {
            edificioOBJ.transform.position = hit.point;

            edificioOBJ.transform.rotation = Quaternion.FromToRotation(Vector3.up,hit.normal);

            if(hit.normal == Vector3.up)
            {
                if(!edificioOBJ.GetComponent<EdificioColocado>().ConstruccionBloqueada)
                {
                    CambiarColor(edificioOBJ, colorUnloock);
                    puedeSoltar = true;
                }
                else
                {
                    CambiarColor(edificioOBJ, colorLoock);
                    puedeSoltar = false;
                }
            }
        }
        else
        {
            CambiarColor(edificioOBJ, colorLoock);
            puedeSoltar = false;
        }
    }

    private void ColocarEdificio()
    {
        // Comprobacion de seguridad
        if(puedeSoltar == false) return;

        //Configurar EdificioColocado
        edificioPorColocarInstanciado.Configurar(actualEdificioBlueprint);

        // Gastar materiales
        RecursosManager.Instance.ColocarEdificio(actualEdificioBlueprint);

        // Devolver colores
        DevolverColores(edificioPorColocarInstanciado.gameObject, coloresOriginales);

        // Informarle al eidifcio que fue colocado
        edificioPorColocarInstanciado.ColocarEdificio();

        //Seleccionar unidad
        UIManager.Instance.SeleccionarUnidad(edificioPorColocarInstanciado);
        // Finalizar el modo construccion
        construyendo = false;   
        
        // Resetear variables
        edificioPorColocarInstanciado = null;
        actualEdificioBlueprint = null;
    }

}
