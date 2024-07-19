using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoRecurso
{
    Madera,
    Piedra,
    Metal,
    Oro,
}

public class RecursoColocado : UnidadColocada
{
    public static Action<UnidadMovilColocada, TipoRecurso> EventoRecursoExtrayendose;

    [SerializeField] private TipoRecurso tipoRecurso;
    public TipoRecurso TipoRecurso=> tipoRecurso;
    public float tiempoEntreRecoleccion = 0.5f;

    private UnidadMovilColocada aldeanoRecolector;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Equipo = Team.Neutral;
    }

    public override void SeleccionarUnidad()
    {
        
    }

    private void OnTriggerEnter(Collider other) 
    {
        // Si colisiona con un aldeano entonces este comienza a extraer el recurso.
        var unidadMovilColocada = other.gameObject.GetComponent<UnidadMovilColocada>();
        
        if (unidadMovilColocada != null && unidadMovilColocada.Tipo == TipoUnidadMovil.Aldeano)
        {
            if(aldeanoRecolector == null)
            {
                // Setear aldeano recolector
                aldeanoRecolector = unidadMovilColocada;

                //comenzae extraccion
                StartCoroutine(ExtraerRecurso());
            }
        }    
    }

    private void OnTriggerExit(Collider other) 
    {
        // Si el aldeano se va entonces se deja de extraer el recurso
        var unidadMovilColocada = other.gameObject.GetComponent<UnidadMovilColocada>();
        
        if (unidadMovilColocada != null && unidadMovilColocada.Tipo == TipoUnidadMovil.Aldeano)
        {
            if(unidadMovilColocada == aldeanoRecolector)
            {
                aldeanoRecolector = null;
            }
        }
    }

    private IEnumerator ExtraerRecurso()
    {
        yield return new WaitForSeconds(tiempoEntreRecoleccion);
        if (aldeanoRecolector != null)
        {
            //Lanzar el evento de recoleccion
            EventoRecursoExtrayendose?.Invoke(aldeanoRecolector, tipoRecurso);
            // Agregar recurso
            RecursosManager.Instance.AgregarRecurso(TipoRecurso, ((int)(aldeanoRecolector.CombatSystem.Daño)));
            // Hacer daño
            VidaSystem.RecibirDaño(aldeanoRecolector.CombatSystem.Daño);
            StartCoroutine(ExtraerRecurso());
        }
    }

}
