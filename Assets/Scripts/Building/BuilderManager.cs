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
    //private List<Material> materialesOriginales = new List<Material>();
    private List<Material[]> originalMaterials = new List<Material[]>();
    private Material materialEditado;

    private bool construyendo = false;
    private bool puedeSoltar = false;
    float ajusteAlturaEdificio = 0f;

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

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float sentido = Input.GetAxis("Mouse ScrollWheel") * 10;
            RotarObjeto(edificioPorColocarInstanciado.gameObject, sentido);
        }
    }

    private void CambiarColor(GameObject edificioOBJ, Color color)
    {
        // Obtener el componente de renderizado del objeto
        Renderer renderer = edificioOBJ.GetComponent<Renderer>();

        if (renderer != null)
        {
            // Crear un nuevo arreglo de materiales basado en los materiales actuales
            Material[] newMaterials = new Material[renderer.materials.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                // Crear un nuevo material basado en el material existente para evitar compartir materiales entre objetos
                newMaterials[i] = new Material(renderer.materials[i]);
                newMaterials[i].color = color;
            }

            // Asignar los nuevos materiales al renderer
            renderer.materials = newMaterials;
        }
    }

    // Cuando guardas los materiales:
    private void GuardarMaterialesOriginales(GameObject edificioOBJ)
    {
        Renderer[] renderers = edificioOBJ.GetComponentsInChildren<Renderer>();
        originalMaterials.Clear();

        foreach (Renderer renderer in renderers)
        {
            // Guardamos una copia de todos los materiales en este renderer
            originalMaterials.Add(renderer.sharedMaterials.Clone() as Material[]);
        }
    }

    // Cuando restauras los materiales:
    private void DevolverColores(GameObject edificioOBJ)
    {
        Renderer[] renderers = edificioOBJ.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            if (i < originalMaterials.Count)
            {
                renderers[i].materials = originalMaterials[i];
            }
        }

        originalMaterials.Clear(); // Limpieza final
    }

    private void MoverObjetoAlMouse(GameObject edificioOBJ)
    {
        if (!construyendo || edificioOBJ == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity,zonaPosible))
        {
            //edificioOBJ.transform.position = hit.point;
            edificioOBJ.transform.position = new Vector3(hit.point.x, hit.point.y + ajusteAlturaEdificio,hit.point.z);

            //edificioOBJ.transform.rotation = Quaternion.FromToRotation(Vector3.up,hit.normal);

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

    private void RotarObjeto(GameObject edificioOBJ, float sentido)
    {
        // Asegúrate de que el objeto no es null para evitar errores de ejecución
        if (edificioOBJ != null)
        {
            // Rota el objeto en 15 grados alrededor del eje Y
            edificioOBJ.transform.Rotate(0, 15f * sentido, 0, Space.World);
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
        DevolverColores(edificioPorColocarInstanciado.gameObject);

        // Informarle al eidifcio que fue colocado
        edificioPorColocarInstanciado.ColocarEdificio();

        // Finalizar el modo construccion
        construyendo = false;   
        
        // Resetear variables
        edificioPorColocarInstanciado = null;
        actualEdificioBlueprint = null;
    }

    public void PosicionarEdificio(EdificioBlueprint edificio)
    {
        //Comprobacion de seguridad
        if (edificio == null) return;

        // Declaracion de variables
        actualEdificioBlueprint = edificio;
        bool recursosSuficientes = RecursosManager.Instance.ComprobarRecursosSuficientesEdificio(actualEdificioBlueprint);
        
        // Comprobar si hay recursos suficientes
        if (!recursosSuficientes){return; }

        //Spawnea un nuevo Edificio
        Vector3 coordenadasSpawn = new Vector3(0, ajusteAlturaEdificio, 0);
        edificioPorColocarInstanciado = Instantiate(actualEdificioBlueprint.EdificioColocadoPrefab, coordenadasSpawn, Quaternion.identity);

        // Ajustar la posición basada en la altura del objeto
        ajusteAlturaEdificio = edificio.EdificioColocadoPrefab.AjusteDeAltura;


        // Obtener el renderer del objeto
        GuardarMaterialesOriginales(edificio.EdificioColocadoPrefab.gameObject);
        
        //Setear color verde o rojo
        CambiarColor(edificioPorColocarInstanciado.gameObject, colorUnloock);

        //Seguir al mouse
        MoverObjetoAlMouse(edificioPorColocarInstanciado.gameObject);

        // Setear el modo construccion
        construyendo = true;
    }
}
