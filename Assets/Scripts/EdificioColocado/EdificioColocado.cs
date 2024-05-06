using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EstadoEdificio
{
    Posicionando,
    EnConstruccion,
    Construido
}

public class EdificioColocado : UnidadColocada
{

    //Este evento se utilizara para notificar a los aldeanos
    public Action<EstadoEdificio> EventoConstruccionEdificio;

    public int TiempoDeConstruccion{private set; get;}


    [Header("Acciones Disponibles")]
    private List<AccionDeEdificioConfig> unidadesDisponiblesConfigs;
    private List<AccionDeEdificioConfig> investigacionesDisponiblesConfigs;

    [Header("Config")]
    [SerializeField] private Transform spawnUnitPosition; // Donde se spawnearan las unidades
    [SerializeField] private int colaMaximaAcciones = 5; // Max de unidades e investigaciones en la cola al mismo tiempo
    [SerializeField] private float ajusteDeAltura;

    //Propiedades de acciones
    public List<AccionDeEdificio> UnidadesDisponibles {private set;get;} = new List<AccionDeEdificio>();
    public List<AccionDeEdificio> InvestigacionesDisponibles {private set;get;} = new List<AccionDeEdificio>();
    public Transform SpawnUnitPosition => spawnUnitPosition;
    

    // Propiedades de construccion
    public EstadoEdificio EstadoConstruccion {private set; get;} = EstadoEdificio.Posicionando;
    public bool ConstruccionBloqueada {private set; get;} // Saber si esta bloqueado para su construcicon
    public int TiempoActualConstruccion {get; private set;} = 0;
    public float AjusteDeAltura => ajusteDeAltura;

    private bool aldeanoDisponible = false;

    // Unidades e investigaciones en cola de produccion | La primer accion se estara creando
    public AccionDeEdificio[] colaActualUnidades {get; private set;}
    public AccionDeEdificio[] colaActualInvestigaciones {get; private set;}
    private bool spawneandoUnidad = false;
    private bool investigando = false;
    public int TiempoActualUnidadDesarrollo {private set; get;} = 0;
    public int TiempoActualInvestigacionDesarrollo {private set; get;} = 0;


    private void Update() 
    {
        if(colaActualUnidades == null) return;
        // Si no se esta spawneando ninguna unidad y hay algun elemento en la cola de unidades entonces se comienza a spawnear la unidad
        if(!spawneandoUnidad && colaActualUnidades[0] != null)
        {
            spawneandoUnidad = true;
            StartCoroutine(SpawnearUnidad());
        }

        if(colaActualInvestigaciones == null) return;
        if(!investigando && colaActualInvestigaciones[0] != null)
        {
            investigando = true;
            StartCoroutine(Investigar());
        }
    }

    public void Configurar(EdificioBlueprint blueprint)
    {
        Icono = blueprint.Icono;
        Name = blueprint.Name;
        Descripcion = blueprint.Descripcion;
        TiempoDeConstruccion = blueprint.TiempoDeConstruccion;
        unidadesDisponiblesConfigs = blueprint.unidadesDisponiblesConfigs;
        investigacionesDisponiblesConfigs = blueprint.investigacionesDisponiblesConfigs;
        colaMaximaAcciones = blueprint.colaMaximaAcciones;
        GetComponent<NavMeshObstacle>().enabled = true;

    }

    public void ColocarEdificio()
    {
        // Modificar el estado del edificio
        EstadoConstruccion = EstadoEdificio.EnConstruccion;

        //Activar la vida
        GetComponent<UnidadVida>().enabled = true;

        // Setear el maximo de acciones
        colaActualUnidades = new AccionDeEdificio[colaMaximaAcciones];
        colaActualInvestigaciones = new AccionDeEdificio[colaMaximaAcciones];

        // Configurar acciones
        foreach (AccionDeEdificioConfig unidadDispoConfig in unidadesDisponiblesConfigs)
        {
            // Crear un nuevo GameObject o usar un GameObject existente
            GameObject go = new GameObject("UnidadDisponible");
            AccionDeEdificio unidadDispoNueva = go.AddComponent<AccionDeEdificio>();

            // Configurar el componente
            unidadDispoNueva.Configurar(unidadDispoConfig);
            unidadDispoNueva.edificioDueño = this;

            // Añadir el GameObject a una lista si es necesario
            UnidadesDisponibles.Add(unidadDispoNueva);
        }

        EventoConstruccionEdificio?.Invoke(EstadoConstruccion); // NO APLICADO AUN. SERA PARA EL CITYMANAGER O ALGO ASI
    }

    public void ConstruirEdificio()
    {
        EstadoConstruccion = EstadoEdificio.Construido;
        EventoConstruccionEdificio?.Invoke(EstadoConstruccion);
    }

    private void OnCollisionEnter(Collision other) 
    {
        //Si el objeto se esta posicionando interesa saber si es otro edificio para bloquear la construccion
        if (EstadoConstruccion == EstadoEdificio.Posicionando)
        {
            // Si es una otra unidad el objeto debera bloquearse
            UnidadColocada edificio = other.gameObject.GetComponent<EdificioColocado>();
            if(edificio != null)
            {
                ConstruccionBloqueada = true;
            }
        }
        //Si el objeto ya esta colocado interesa saber si es un aldeano para construir
        else if(EstadoConstruccion == EstadoEdificio.EnConstruccion)
        {
            UnidadMovilColocada unidadMovil = other.gameObject.GetComponent<UnidadMovilColocada>();
            // Si la unidad es una unidad movil y es un aldeano
            if(unidadMovil != null && unidadMovil.Tipo == TipoUnidadMovil.Aldeano)
            {
                //comenzar Construccion
                aldeanoDisponible = true;
                StartCoroutine(ContinuarConstruccion());
            }
        }

    }

    private void OnCollisionExit(Collision other) 
    {
        if (EstadoConstruccion == EstadoEdificio.Posicionando)
        {
            // Si es una otra unidad el objeto debera bloquearse
            UnidadColocada otraUnidad = other.gameObject.GetComponent<EdificioColocado>();
            if(otraUnidad == null) return;
            ConstruccionBloqueada = false;
        }
        else if(EstadoConstruccion == EstadoEdificio.EnConstruccion)
        {
            UnidadMovilColocada unidadMovil = other.gameObject.GetComponent<UnidadMovilColocada>();
            if(unidadMovil != null && unidadMovil.Tipo == TipoUnidadMovil.Aldeano)
            {
                // Informar que no hay aldeanos disponibles
                aldeanoDisponible = false;
                //detener Construccion
                StopCoroutine(ContinuarConstruccion());
            }
        }
    }

    public override void SeleccionarUnidad()
    {
        UIManager.Instance.SeleccionarUnidad(this);   
    }

    private IEnumerator ContinuarConstruccion()
    {
        TiempoActualConstruccion ++;
        yield return new WaitForSeconds(1f);

        if(aldeanoDisponible)
        {
            if (TiempoActualConstruccion < TiempoDeConstruccion)
            {
                StartCoroutine(ContinuarConstruccion());
            }
            else
            {
                ConstruirEdificio();
            }

        }
    }

    #region Acciones
    // Comprobar si puede agregar una unidad a la cola y si es asi lo hace y devuelve true, caso contrario devuelve false
    public bool ComprobarYAgregarUnidadCola(AccionDeEdificio unidadAAgregar)
    {
        if (unidadAAgregar == null) return false;

        for (int i = 0; i < colaActualUnidades.Length; i++)
        {
            if ( colaActualUnidades[i] == null)
            {
                colaActualUnidades[i] = unidadAAgregar;
                ActualizarAccionesEnProgresoUI() ;
                
                return true;
            }
        }

        return false;
    } 
    
    public bool ComprobarYAgregarInvestigacionCola(AccionDeEdificio InvestigacionAAgregar)
    {
        if (InvestigacionAAgregar == null) return false;

        for (int i = 0; i < colaActualInvestigaciones.Length; i++)
        {
            if ( colaActualInvestigaciones[i] == null)
            {
                colaActualInvestigaciones[i] = InvestigacionAAgregar;
                ActualizarAccionesEnProgresoUI() ;
                return true;
            }
        }

        return false;
    } 
    
    // Si hay unidades para eliminar de la cola devuelve true y resta una
    public bool EliminarUltimaUnidadDeCola()
    {
        // Se almacenara el index de la unidad que se borrara
        int unidadABorrarIndex = -1;

        // Se recorre la lista y se va sobreescribiendo la ultima unidad que no es null asi se borra de atras para adelante
        for (int i = 0; i < colaActualUnidades.Length; i++)
        {
            if ( colaActualUnidades[i] != null)
            {
                unidadABorrarIndex = i;
            }
        }

        // Si el indice es diferente a -1 significa que hay al menos una unidad en entonces se borra
        if (unidadABorrarIndex != -1)
        {
            // Elimina la ultima unidad 
            colaActualUnidades[unidadABorrarIndex] = null;
            ActualizarAccionesEnProgresoUI() ;
            return true;
        }

        // Caso contrario retorna false
        return false;
    } 
    
    public bool EliminarInvestigacionDeCola()
    {
        // Se almacenara el index de la unidad que se borrara
        int investigacionABorrarIndex = -1;

        // Se recorre la lista y se va sobreescribiendo la ultima unidad que no es null asi se borra de atras para adelante
        for (int i = 0; i < colaActualInvestigaciones.Length; i++)
        {
            if ( colaActualInvestigaciones[i] != null)
            {
                investigacionABorrarIndex = i;
            }
        }

        // Si el indice es diferente a -1 significa que hay al menos una unidad en entonces se borra y se notifica
        if (investigacionABorrarIndex != -1)
        {
            // Elimina la ultima unidad 
            colaActualInvestigaciones[investigacionABorrarIndex] = null;
            ActualizarAccionesEnProgresoUI() ;
            return true;
        }

        // Caso contrario retorna false
        return false;
    }

    private IEnumerator SpawnearUnidad()
    {
        //Sumar tiempo transcurrido 
        // Esperar un segundo para comprobar si debe seguir sumando 
        TiempoActualUnidadDesarrollo ++;
        
        yield return new WaitForSeconds(1);

        if(TiempoActualUnidadDesarrollo >= colaActualUnidades[0].TiempoRequerido)
        {
            // Spawnear la unidad
            colaActualUnidades[0].SpawnearUnidad();
            //Eliminar de la lista
            // Mover las unidades de la lista una posicion para adelante
            for (int i = 0; i < colaActualUnidades.Length -1; i++)
            {
                colaActualUnidades[i] = colaActualUnidades[i+1];
            }

            //Eliminar siempre la ultima posicion (porque es la unica que podria quedar ocupada y nunca deberia hacerlo)
            colaActualUnidades[colaActualUnidades.Length - 1] = null;

            // Reiniciar el tiempo
            TiempoActualUnidadDesarrollo = 0;
            // Reiniciar booleano.
            spawneandoUnidad = false;
        }
        else
        {
            StartCoroutine(SpawnearUnidad());            
        }

        // Actualizar UI
        ActualizarAccionesEnProgresoUI();
    }

    private IEnumerator Investigar()
    {
        //Sumar tiempo transcurrido 
        // Esperar un segundo para comprobar si debe seguir sumando 
        TiempoActualInvestigacionDesarrollo ++;

        yield return new WaitForSeconds(1);

        if(TiempoActualInvestigacionDesarrollo >= colaActualInvestigaciones[0].TiempoRequerido)
        {
            // Spawnear la unidad
            colaActualInvestigaciones[0].Investigar();
            // Mover las unidades de la lista una posicion para adelante
            for (int i = 0; i < colaActualInvestigaciones.Length -1; i++)
            {
                colaActualInvestigaciones[i] = colaActualInvestigaciones[i];
            }

            //Eliminar siempre la ultima posicion (porque es la unica que podria quedar ocupada y nunca deberia hacerlo)
            colaActualInvestigaciones[colaActualInvestigaciones.Length - 1] = null;

            // Reiniciar el tiempo
            TiempoActualInvestigacionDesarrollo = 0;
            // Reiniciar booleano.
            spawneandoUnidad = false;
        }
        else
        {
            StartCoroutine(Investigar());
        }

        // Actualizar UI
        ActualizarAccionesEnProgresoUI();
    }

    private void ActualizarAccionesEnProgresoUI()
    {
        UIManager.Instance.ConfigurarPanelAccionesEnProgreso(this);
    }
    #endregion
}