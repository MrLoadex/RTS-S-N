using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccionDeEdificio : MonoBehaviour
{
    public TipoAccion Tipo;
    public Sprite Icono;
    public string ID;
    public string Name;
    [TextArea(3, 10)] public string Descripcion;
    public int TiempoRequerido;
    public int CostoOro;
    public int CostoMadera;
    public int CostoPiedra;
    public int CostoMetal;
    public UnidadMovilColocada unidadPorSpawnearPrefab;
    
    [HideInInspector] public EdificioColocado edificioDueño = null;
    
    public void Configurar(AccionDeEdificioConfig accionConfig )
    {
        Tipo = accionConfig.Tipo;
        Icono = accionConfig.Icono;
        ID = accionConfig.ID;
        Name = accionConfig.Name;
        Descripcion = accionConfig.Descripcion;
        TiempoRequerido = accionConfig.TiempoRequerido;
        CostoOro = accionConfig.CostoOro;
        CostoMadera = accionConfig.CostoMadera;
        CostoPiedra = accionConfig.CostoPiedra;
        CostoMetal = accionConfig.CostoMetal;
        unidadPorSpawnearPrefab = accionConfig.UnidadPorSpawnearPrefab;


    }

    public void Accionar()
    {
        if(Tipo == TipoAccion.CreacionDeUnidad)
        {
            //spawnear uniadad si es una unidad
            AgregarUnidadALaCola();
        }
        else if(Tipo == TipoAccion.Investigacion)
        {
            //Desbloquear edificios si es una investigacion
            AgregarInvestigacionALaCola();

        }

    }

    private void AgregarUnidadALaCola()
    {
        // Comprobar si tiene suficientes recursos.
        if (!RecursosManager.Instance.ComprobarRecursosSuficientesAccion(this))
        {
            return;
        }


        // Comprobar si hay espacio en la cola y agregar a la cola si se puede
        if (!edificioDueño.ComprobarYAgregarUnidadCola(this))
        {
            return;
        }

        // Consumir recursos    
        RecursosManager.Instance.ConsumirAccion(this);
    }

    private void AgregarInvestigacionALaCola()
    {
        // Comprobar si tiene suficientes recursos.
        if (!RecursosManager.Instance.ComprobarRecursosSuficientesAccion(this))
        {
            return;
        }

        // Comprobar si hay espacio en la cola y agregar a la cola si se puede
        if (!edificioDueño.ComprobarYAgregarInvestigacionCola(this))
        {
            return;
        }
 
        // Consumir recursos    
        RecursosManager.Instance.ConsumirAccion(this);
    }

    public void SpawnearUnidad()
    {
        // Configurar Unidad
        unidadPorSpawnearPrefab.Icono = Icono;
        unidadPorSpawnearPrefab.Name = Name;
        unidadPorSpawnearPrefab.Descripcion = Descripcion;

        // Spawnear Unidad
        UnidadMovilColocada unidadInstanciada = Instantiate(unidadPorSpawnearPrefab);
        unidadInstanciada.transform.position = edificioDueño.SpawnUnitPosition.position;
    }

    public void Investigar()
    {
        // Desbloquear Edificios
    }

    #region Eliminar Acciones en cola
    // Violando SOLID, pero es una soilucion rapida y efectiva
    public void EliminarUltimaUnidadDeCola()
    {
        if (edificioDueño == null) return;
        edificioDueño.EliminarUltimaUnidadDeCola();
    }

    public void EliminarUltimaInvDeCola()
    {
        if (edificioDueño == null) return;
        edificioDueño.EliminarInvestigacionDeCola();
    }
    #endregion
}