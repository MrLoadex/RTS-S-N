using UnityEngine;
using UnityEngine.UI;

public class EdificioBlueprintButton : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Image icono;

    private EdificioBlueprint edificio;  


    public void Congigurar(EdificioBlueprint pEdificio)
    {
        edificio = pEdificio;
        icono.sprite = edificio.Icono;
    }

    public void MostrarInformacion()
    {
        UIManager.Instance.MostrarInformacionConstruccion(edificio);
    }

    public void OcultarInfo()
    {
        UIManager.Instance.OcultarInfoConstruccion();
    }

    public void ColocarEdificio()
    {

        BuilderManager.Instance.ColocarObjeto(edificio);

    }
}
