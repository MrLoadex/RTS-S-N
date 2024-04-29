using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    //[Header("Panel Principal")]

    [Header("Recursos")]
    [SerializeField] private TextMeshProUGUI oroTMP;
    [SerializeField] private TextMeshProUGUI maderaTMP;
    [SerializeField] private TextMeshProUGUI piedraTMP;
    [SerializeField] private TextMeshProUGUI metalTMP;

    [Header("Paneles")]
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject buildPanel;
    [SerializeField] private GameObject edificioPanel;
    [SerializeField] private GameObject edificioEnConstruccionPanel;
    [SerializeField] private GameObject decripcionEdificioPanel;

    [Header("Building")]
    [SerializeField] private Transform buildersContenedor;
    [SerializeField] private EdificioBlueprintTarjeta edificioBotonPrefab;

    [Header("Build Info")]
    [SerializeField] private GameObject buildPanelInfo;
    [SerializeField] private TextMeshProUGUI buildNameTMP;
    [SerializeField] private TextMeshProUGUI buildDescripcionTMP;
    [SerializeField] private TextMeshProUGUI buildCostoOroCantidadTMP;
    [SerializeField] private TextMeshProUGUI buildCostoMaderaCantidadTMP;
    [SerializeField] private TextMeshProUGUI buildCostoPiedraCantidadTMP;
    [SerializeField] private TextMeshProUGUI buildCostoMetalCantidadTMP;

    [Header("Edificio En Construccion Config")]
    [SerializeField] private Image iconoEdificioEnConstruccion;
    [SerializeField] private TextMeshProUGUI nameEdificioEnConstruccionTMP;
    [SerializeField] private TextMeshProUGUI descripcionEdificioEnConstruccionTMP;
    [SerializeField] private Image estadoEdificioEnConstruccionIMG;

    [Header("Edificio Config")]
    [SerializeField] private Image iconoEdificio;
    [SerializeField] private TextMeshProUGUI nameEdificioTMP;
    [SerializeField] private TextMeshProUGUI descripcionEdificioTMP;

    [Header("Acciones Edificio Config")]
    [SerializeField] private GameObject descripcionAccionEdificioPanel;
    [SerializeField] private Image accionIcono;
    [SerializeField] private TextMeshProUGUI accionName;
    [SerializeField] private TextMeshProUGUI accionDescripion;
    [SerializeField] private AccionEdificioTarjeta accionEdificioTarjetaPrefab;
    [SerializeField] private Transform contenedorTarjetasUnidadPrefab;
    [SerializeField] private Transform contenedorTarjetasInvPrefab;

    [Header("Acciones Edificio En Progreso Config")]
    [SerializeField] private AccionEdificioTarjeta accionEnProgresoTarjetaPrefab;
    [SerializeField] private GameObject unidadEnProgresoPanel;
    [SerializeField] private GameObject InvestEnProgresoPanel;
    [SerializeField] private Image estadoUnidadEnProgresoIMG;
    [SerializeField] private Image estadoInvestEnProgresoIMG;
    [SerializeField] private Transform contenedorTarjetasUnidProgres;
    [SerializeField] private Transform contenedorTarjetasInvProgres;

    [Header("Costo accion de Edificio")]
    [SerializeField] private TextMeshProUGUI accionEdificioCostoOroTMP;
    [SerializeField] private TextMeshProUGUI accionEdificioCostoMaderaTMP;
    [SerializeField] private TextMeshProUGUI accionEdificioCostoPiedraTMP;
    [SerializeField] private TextMeshProUGUI accionEdificioCostoMetalTMP;

    // Guarda la ultima unidad seleccionada para saber si debe seguir actualizando la interface de construccion
    private UnidadColocada ultimaUnidadSeleccionada;
    // Listas para saber si se actualizan las colas del edificio
    private AccionDeEdificio[] colaActualUnidades = new AccionDeEdificio[5]; // Se harckodea 5 por comodidad
    private AccionDeEdificio[] colaActualInvestigaciones = new AccionDeEdificio[5];


    public void ActualizarRecursosDisponibles(int oro, int madera, int piedra, int metal)
    {
        oroTMP.text = oro.ToString();
        maderaTMP.text = madera.ToString();
        piedraTMP.text = piedra.ToString();
        metalTMP.text = metal.ToString();
    }

    public void SeleccionarUnidad(UnidadColocada unidadSeleccionada)
    {
        // Actualizar ultima unidad seleccionada
        ultimaUnidadSeleccionada = unidadSeleccionada;

        if (unidadSeleccionada is EdificioColocado)
        {
            // Extraer el componente de edificio
            EdificioColocado edificio = unidadSeleccionada.GetComponent<EdificioColocado>();

            //Configurar UI
            ConfigurarEdificioSeleccionadoUI(edificio);
        }
    }

    #region Edificio Seleccionado

    private void ConfigurarEdificioSeleccionadoUI(EdificioColocado edificio)
    {
        if(edificio.EstadoConstruccion == EstadoEdificio.Construido)
        {
            ConfigurarEdificioConsturidoUI(edificio);
            ConfigurarPanelUnidadesEnProgreso(edificio);
        }
        else if(edificio.EstadoConstruccion == EstadoEdificio.EnConstruccion)
        {
            ConfigurarEdificioEnConstrucionUI(edificio);
        }

    }

    private void ConfigurarEdificioEnConstrucionUI(EdificioColocado edificio)
    {
        //Actualizar la informacion del edificio seleccionado
        iconoEdificioEnConstruccion.sprite = edificio.Icono;
        nameEdificioEnConstruccionTMP.text = edificio.Name;
        descripcionEdificioEnConstruccionTMP.text = edificio.Descripcion;

        // Actualizar barra de estado
        if (edificio.TiempoActualConstruccion != 0) // If para evitar division entre ceros
        {
            estadoEdificioEnConstruccionIMG.fillAmount = ((float)edificio.TiempoActualConstruccion / (float)edificio.TiempoDeConstruccion);
        }
        else
        {
            estadoEdificioEnConstruccionIMG.fillAmount = (edificio.TiempoActualConstruccion);
        }
        // Abrir el panel
        AbrirCerrarPanelEdificioEnConstruccion(true);
        // Mantener actualizada la UI
        StartCoroutine(MantenerActualizadoEdificioConstruccionUI(edificio));
    }

    private void ConfigurarEdificioConsturidoUI(EdificioColocado edificio)
    {
        //Actualizar la informacion del edificio seleccionado
        iconoEdificio.sprite = edificio.Icono;
        nameEdificioTMP.text = edificio.Name;
        descripcionEdificioTMP.text = edificio.Descripcion;

        // Destruir todas las tarjetas que hay en Unidades
        for (int i = contenedorTarjetasUnidadPrefab.childCount - 1; i >= 0; i--)
        {
            // Obtener el hijo actual
            GameObject hijo = contenedorTarjetasUnidadPrefab.GetChild(i).gameObject;

            // Destruir el hijo
            Destroy(hijo);
        }
        // Destruir todas las tarjetas que hay en Investigaciones
        for (int i = contenedorTarjetasInvPrefab.childCount - 1; i >= 0; i--)
        {
            // Obtener el hijo actual
            GameObject hijo = contenedorTarjetasInvPrefab.GetChild(i).gameObject;

            // Destruir el hijo
            Destroy(hijo);
        }

        // INSTANCIAR Y CONFIGURAR TARJETAS :
        if(edificio.UnidadesDisponibles.Count > 0)// Comprobacion de seguridad
        {
            foreach (AccionDeEdificio unidadDispo in edificio.UnidadesDisponibles)
            {
                AccionEdificioTarjeta tarjetaInstanciada = Instantiate(accionEdificioTarjetaPrefab,contenedorTarjetasUnidadPrefab);
                tarjetaInstanciada.ConfigurarTarjeta(unidadDispo);
            }
        }
        if(edificio.InvestigacionesDisponibles.Count > 0) // Comprobacion de seguridad
        {
            foreach (AccionDeEdificio investigacion in edificio.InvestigacionesDisponibles)
            {
                AccionEdificioTarjeta tarjetaInstanciada = Instantiate(accionEdificioTarjetaPrefab,contenedorTarjetasInvPrefab);
                tarjetaInstanciada.ConfigurarTarjeta(investigacion);
            }
        }
    
        // Abrir el panel
        AbrirCerrarPanelEdificioConstruido(true);
    }

    public void ActualizarPanelDescripcionAccionEdificio(AccionDeEdificio accion)
    {
        accionIcono.sprite = accion.Icono;
        accionName.text = accion.Name;
        accionDescripion.text = accion.Descripcion;
        accionEdificioCostoOroTMP.text = accion.CostoOro.ToString();
        accionEdificioCostoMaderaTMP.text = accion.CostoMadera.ToString();
        accionEdificioCostoPiedraTMP.text = accion.CostoPiedra.ToString();
        accionEdificioCostoMetalTMP.text = accion.CostoMetal.ToString();
    }

    public void AbrirCerrarPanelDescripcionAccionEdificio(bool estado)
    {
        descripcionAccionEdificioPanel.SetActive(estado);
    }

    IEnumerator MantenerActualizadoEdificioConstruccionUI(EdificioColocado edificio)
    {
        if (ultimaUnidadSeleccionada != edificio)
        {
            StopCoroutine(MantenerActualizadoEdificioConstruccionUI(edificio));
        }
        else
        {
            // Actualizar barra de estado
            if (edificio.TiempoActualConstruccion != 0) // If para evitar division entre ceros
            {
                estadoEdificioEnConstruccionIMG.fillAmount = ((float)edificio.TiempoActualConstruccion / (float)edificio.TiempoDeConstruccion);
            }
            else
            {
                estadoEdificioEnConstruccionIMG.fillAmount = (edificio.TiempoActualConstruccion);
            }
            yield return new WaitForSeconds(1);

            // Mientras el ultimo edificio colocado sea el mismo y este siga en construccion se mantiene actualizada la UI.
            if (ultimaUnidadSeleccionada == edificio && edificio.EstadoConstruccion == EstadoEdificio.EnConstruccion)
            {
                StartCoroutine(MantenerActualizadoEdificioConstruccionUI(edificio));
            }
            else if( ultimaUnidadSeleccionada == edificio && edificio.EstadoConstruccion == EstadoEdificio.Construido)
            {
                //Si es el mismo edificio, pero ya esta construido entonces se abre la interface correcta
                ConfigurarEdificioConsturidoUI(edificio);
                CerrarPaneles();
                AbrirCerrarPanelEdificioConstruido(true);
            }
        }


    }

    //ACCIONES EN PROGRESO

    public void ConfigurarPanelAccionesEnProgreso(EdificioColocado edificio)
    {
        // Comprobaciones de seguridad
        if(edificio == null) return;
        
        ConfigurarPanelUnidadesEnProgreso(edificio);
        ConfigurarPanelInvEnProgreso(edificio);

        

    }

    private void ConfigurarPanelUnidadesEnProgreso(EdificioColocado edificio)
    {
        //Si la unidad no es la seleccionada no se hace nada.
        if (ultimaUnidadSeleccionada != edificio) return;

        // Si no hay unidades se cierra el panel
        if(edificio.colaActualUnidades[0] == null)
        {
            //Desactivar el panel
            unidadEnProgresoPanel.SetActive(false);
            return;
        }
        else if(unidadEnProgresoPanel.activeSelf == false)
        {
            unidadEnProgresoPanel.SetActive(true);
        }


        // Realizar comparacion de listas
        bool listasIguales = false;
        for (int i = 0; i < edificio.colaActualUnidades.Length; i++)
        {
            if(edificio.colaActualUnidades[i] != colaActualUnidades[i])
            {
                listasIguales = false;
                break;
            }
            else
            {
                listasIguales = true;
            }
        }


        // Si las listas son distintas se limpia y se vuelve a instanciar. Si son iguales solo se actualiza el progreso
        if(!listasIguales)
        {
            // Recorre todos los hijos del padre y los destruye
            for (int i = contenedorTarjetasUnidProgres.childCount - 1; i >= 0; i--)
            {
                // Destruye el hijo actual
                Destroy(contenedorTarjetasUnidProgres.GetChild(i).gameObject);
            }
            
            // Igualar lista del edificio con la de control
            for (int i = 0; i < edificio.colaActualUnidades.Length; i++)
            {
                colaActualUnidades[i] = edificio.colaActualUnidades[i];
            }

            // Instanciar todos los objetos
            for (int i = 0; i < edificio.colaActualUnidades.Length; i++)
            {

                if (edificio.colaActualUnidades[i] != null)
                {
                    AccionEdificioTarjeta accion = Instantiate(accionEnProgresoTarjetaPrefab,contenedorTarjetasUnidProgres);
                    accion.ConfigurarTarjeta(edificio.colaActualUnidades[i]);
                }
                else
                {
                    break;
                }
                // Instanciar y configurar la tarjeta
            }
        }

        // Actualizar barra de estado
        if (edificio.TiempoActualUnidadDesarrollo != 0) // If para evitar division entre ceros
        {
            // Se actualiza la barra
            estadoUnidadEnProgresoIMG.fillAmount = ((float)edificio.TiempoActualUnidadDesarrollo / (float)edificio.colaActualUnidades[0].TiempoRequerido);
        }
        else
        {
            estadoUnidadEnProgresoIMG.fillAmount = (0);
        }
    }

    private void ConfigurarPanelInvEnProgreso(EdificioColocado edificio)
    {
        if(edificio.colaActualInvestigaciones[0] == null)
        {
            //Desactivar el panel
            InvestEnProgresoPanel.SetActive(false);
            return;
        }
    }

    #endregion

    #region Building
    public void InicializarPanelConstruccion(EdificioBlueprint[] buildersDsiponibles)
    {
        // Instanciar Edificios
        for (int i = 0; i < buildersDsiponibles.Length; i++)
        {
            EdificioBlueprintTarjeta edificio = Instantiate(edificioBotonPrefab, buildersContenedor);
            edificio.Congigurar(buildersDsiponibles[i]);
        }
    }
    
    public void MostrarInformacionConstruccion(EdificioBlueprint edificio)
    {
        // Configurar panel
        buildNameTMP.text = edificio.Name;
        buildDescripcionTMP.text = edificio.Descripcion;
        buildCostoOroCantidadTMP.text = edificio.CostoOro.ToString();
        buildCostoMaderaCantidadTMP.text = edificio.CostoMadera.ToString();
        buildCostoPiedraCantidadTMP.text = edificio.CostoPiedra.ToString();
        buildCostoMetalCantidadTMP.text = edificio.CostoMadera.ToString();
        
        // Activar Panel
        buildPanelInfo.SetActive(true);
    }

    public void OcultarInfoConstruccion()
    {
        buildPanelInfo.SetActive(false);
    }
    #endregion

    #region Paneles
    public void AbrirCerrarPanelEdificioConstruido(bool estado)
    {
        CerrarPaneles();
        edificioPanel.SetActive(estado);
        
    }

    public void AbrirCerrarPanelConstruccion(bool estado)
    {
        CerrarPaneles();
        buildPanel.SetActive(estado);
        
    }
    
    public void AbrirCerrarPanelGeneral(bool estado)
    {
        CerrarPaneles();
        generalPanel.SetActive(estado);
    }

    public void AbrirCerrarPanelEdificioEnConstruccion(bool estado)
    {
        CerrarPaneles();
        edificioEnConstruccionPanel.SetActive(estado);
    }

    private void CerrarPaneles()
    {
        edificioPanel.SetActive(false);
        edificioEnConstruccionPanel.SetActive(false);
        buildPanel.SetActive(false);
        generalPanel.SetActive(false);
    }
    #endregion

}

