using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnidadVision : MonoBehaviour
{

    [HideInInspector] public UnidadMovilColocada unidadDueña;

    private void OnTriggerEnter(Collider other) 
    {
        UnidadColocada unidadDivisada = other.gameObject.GetComponent<UnidadColocada>();
        
        if(unidadDivisada ==null || unidadDivisada.Equipo == unidadDueña.Equipo) { return; }

        unidadDueña.CombatSystem.DivisarEnemigo(unidadDivisada);
    }

    private void OnTriggerExit(Collider other) 
    {
        UnidadColocada unidadDivisada = other.gameObject.GetComponent<UnidadColocada>();
        
        if(unidadDivisada ==null || unidadDivisada.Equipo == unidadDueña.Equipo) { return; }

        unidadDueña.CombatSystem.PerderEnemigo(unidadDivisada);  
    }
}
