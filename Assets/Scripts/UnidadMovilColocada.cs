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
    public bool Aliado = true;


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
        
        // Abrir su panel

        //Prepararse para moverse
    }

}
