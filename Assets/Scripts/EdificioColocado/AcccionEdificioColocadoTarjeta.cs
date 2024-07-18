using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccionEdificioTarjeta : MonoBehaviour
{
    [SerializeField] private Image ButtonIcono;
    private AccionDeEdificio accion;

    public void ConfigurarTarjeta(AccionDeEdificio pAccion) //Llamado desde....
    {
        accion = pAccion;
        
        ButtonIcono.sprite = pAccion.Icono;
    }

    public void MostrarDescripcion()
    {
        //Muestra la descripcion
        UIManager.Instance.ActualizarPanelDescripcionAccionEdificio(accion);
        UIManager.Instance.AbrirCerrarPanelDescripcionAccionEdificio(true);
    }

    public void OcultarDescripcion()
    {
        UIManager.Instance.AbrirCerrarPanelDescripcionAccionEdificio(false);
    }

    public void Accionar()
    {
        //Comprueba si alcanzan los recursos y ejecuta la accion
        accion.Accionar();
    }


    #region Eliminar Acciones en cola
    // Violando SOLID, pero es una soilucion rapida
    public void EliminarUltimaUnidadDeCola()
    {
        accion.EliminarUltimaUnidadDeCola();
    }

    public void EliminarUltimaInvDeCola()
    {
        accion.EliminarUltimaInvDeCola();
    }

    #endregion
    
    
}
