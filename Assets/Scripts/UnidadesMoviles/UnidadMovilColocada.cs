using System;
using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] private GameObject targetIndicatorPrefab;

    protected override void Start() 
    {
        base.Start();
        CombatSystem = GetComponent<UnitCombat>();
        
        // Instanciar la vision
        UnidadVision vision = Instantiate(visionPrefab, transform);
        vision.unidadDueña = this;
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

    public override void SeleccionarComoObjetivo()
    {
        var targetIndicatorGO = Instantiate(targetIndicatorPrefab, gameObject.transform);
        StartCoroutine(ElimiarObjetoConRetraso(targetIndicatorGO, 0.3f));
    }
    
    IEnumerator ElimiarObjetoConRetraso(GameObject objeto, float retraso)
    {
        yield return new WaitForSeconds(retraso);
        if (objeto != null || !objeto.IsDestroyed())
        {
            Destroy(objeto);
        }

    }

}