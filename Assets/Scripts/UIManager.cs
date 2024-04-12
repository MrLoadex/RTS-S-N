using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{

    //[Header("Recursos")]

    [Header("Paneles")]
    [SerializeField] private GameObject buildPanel;

    [Header("Building")]
    [SerializeField] private Transform buildersContenedor;
    [SerializeField] private EdificioBlueprintButton edificioBotonPrefab;

    [Header("Build Info")]
    [SerializeField] private GameObject buildPanelInfo;
    [SerializeField] private TextMeshProUGUI buildNameTMP;
    [SerializeField] private TextMeshProUGUI buildDescripcionTMP;
    [SerializeField] private TextMeshProUGUI buildCostoOroCantidadTMP;
    [SerializeField] private TextMeshProUGUI buildCostoMaderaCantidadTMP;
    [SerializeField] private TextMeshProUGUI buildCostoPiedraCantidadTMP;
    [SerializeField] private TextMeshProUGUI buildCostoMetalCantidadTMP;



    #region Building
    public void AbrirCerrarPanelConstruccion(bool estado)
    {
        buildPanel.SetActive(estado);
    }

    public void InicializarPanelConstruccion(EdificioBlueprint[] buildersDsiponibles)
    {
        // Instanciar Edificios
        for (int i = 0; i < buildersDsiponibles.Length; i++)
        {
            EdificioBlueprintButton edificio = Instantiate(edificioBotonPrefab, buildersContenedor);
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

}

