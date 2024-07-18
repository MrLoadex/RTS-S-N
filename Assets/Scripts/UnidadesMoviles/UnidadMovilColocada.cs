using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public enum TipoUnidadMovil
{
    Aldeano,
    Guerrero
}

public class UnidadMovilColocada : UnidadColocada
{
    public TipoUnidadMovil Tipo;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private UnidadVision visionPrefab;
    public UnitCombat CombatSystem { get; private set; }
    [SerializeField] private GameObject auraSeleccionObject;

    protected override void Start() 
    {
        base.Start();
        CombatSystem = GetComponent<UnitCombat>();
        
        // Instanciar la vision
        UnidadVision vision = Instantiate(visionPrefab, transform);
        vision.unidadDueña = this;
    }

    private void Update() 
    {
    }
    //Mover unidad
    public void MoverUnidad(Vector3 posicionObjetivo)
    {
        agent.SetDestination(posicionObjetivo);
    }

    public override void SeleccionarUnidad()
    {
        auraSeleccionObject?.SetActive(true);
    }
    
    public override void DeseleccionarUnidad()
    {
        // Solo los aliados tienen un aura de seleccion
        if (Equipo != Team.Aliado) return;
        auraSeleccionObject.SetActive(false);
    }
}