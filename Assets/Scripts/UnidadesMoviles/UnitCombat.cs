using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CombatType
{
    Melee,
    Distancia
}

public enum Comportamiento
{
    Pasivo,
    Agresivo,
    Neutral,
    Defensivo
}

public enum Estado
{
    Atacando,
    Defendiendo,
    Siguiendo,
    Descansando
}

public class UnitCombat : MonoBehaviour
{

    [Header ("Config")]
    [SerializeField] private CombatType tipoDeAtaque;
    [SerializeField] private float daño = 10;
    [SerializeField] private float EsperaEntreAtaque = 0.5f;
    [SerializeField] private Arrow flechaPrefab;
    [SerializeField] private float rangoMelee = 1f;
    [SerializeField] private float rangoDistancia = 5f;


    // PROPS
    public CombatType TipoDeAtaque =>tipoDeAtaque;
    public float RangoMelee => rangoMelee;
    public float RangoDistancia => rangoDistancia;
    public Comportamiento comportamientoActual;

    private List<UnidadColocada> unidadesEnemigasDivisadas = new List<UnidadColocada>();

    float rangoActual; // Almacena el rango actual (melee o distancia) para mayor comodidad
    Estado estado = Estado.Descansando;
    Vector3 posicionOriginal;

    private void Start() 
    {

        // Si se esta atacando una unidad
        if (tipoDeAtaque == CombatType.Melee)
        {
            rangoActual = rangoMelee;
        }
        else
        {
            rangoActual = rangoDistancia;
        }
    }

    private void Update() 
    {
        // Si se esta viendo a algun enemigo y no se esta en pasivo
        if(unidadesEnemigasDivisadas.Count > 0 && comportamientoActual != Comportamiento.Pasivo)
        {
            switch (comportamientoActual)
            {
                case Comportamiento.Neutral:
                AtacarSiEsVisible();
                break;
                case Comportamiento.Agresivo:
                ObservarSeguirYAtacar();
                break;
                case Comportamiento.Defensivo:
                //
                break;
                case Comportamiento.Pasivo:
                // No hace nada
                break;
            }
        }
    }

    private void AtacarSiEsVisible()
    {

        if (estado == Estado.Descansando)
        {
            UnidadColocada enemigoMasCercano = null;
            float distanciaMasCercana = Mathf.Infinity;

            foreach (var enemigo in unidadesEnemigasDivisadas)
            {
                // Calcular la distancia al enemigo
                float distanciaAlEnemigo = (enemigo.transform.position - transform.position).magnitude;

                // Si es visible y está más cerca que el anteriormente más cercano
                if (distanciaAlEnemigo < distanciaMasCercana)
                {
                    enemigoMasCercano = enemigo;
                    distanciaMasCercana = distanciaAlEnemigo;
                }
            }

            // Si no se encontró un enemigo visible más cercano
            if (enemigoMasCercano != null)
            {
                // Limpiar variables
                FinalizarAtaque();
                // Guardar la posicion original
                posicionOriginal = transform.position;
                estado = Estado.Siguiendo;
                StartCoroutine(AtaqueYSeguimientoCortoNeutral(enemigoMasCercano));
            }
        }
    }

    private IEnumerator AtaqueYSeguimientoCortoDefensivo(UnidadColocada enemigo)
    {
        SeguirYAtacar(enemigo.GetComponent<UnidadMovilColocada>());

        yield return new WaitForSeconds(0.5f);

        // Si se movio demasiado de su posicion original
        if((posicionOriginal - transform.position).magnitude > rangoDistancia + rangoMelee || enemigo == null)
        {
            // Mover a la posicion original
            GetComponent<UnidadMovilColocada>().MoverUnidad(posicionOriginal);
            // Finalizar la defensa
            estado = Estado.Descansando;
        }
        else
        {
            // Continuar con al coroutine
            StartCoroutine(AtaqueYSeguimientoCortoDefensivo(enemigo));
        }
    }

    private IEnumerator AtaqueYSeguimientoCortoNeutral(UnidadColocada enemigo)
    {
        SeguirYAtacar(enemigo.GetComponent<UnidadMovilColocada>());

        yield return new WaitForSeconds(0.5f);

        // Si se movio demasiado de su posicion original
        if((posicionOriginal - transform.position).magnitude > rangoDistancia + rangoMelee || enemigo == null)
        {
            // Mover a la posicion original
            GetComponent<UnidadMovilColocada>().MoverUnidad(posicionOriginal);
            // Finalizar ataque
            FinalizarAtaque();

            yield return new WaitForSeconds(5f);
            estado = Estado.Descansando;
        }
        else
        {
            // Continuar con al coroutine
            StartCoroutine(AtaqueYSeguimientoCortoNeutral(enemigo));
        }
    }

    private void ObservarSeguirYAtacar()
    {
        UnidadColocada enemigoMasCercano = null;
        float distanciaMasCercana = Mathf.Infinity;

        foreach (var enemigo in unidadesEnemigasDivisadas)
        {
            // Calcular la distancia al enemigo
            float distanciaAlEnemigo = (enemigo.transform.position - transform.position).sqrMagnitude;

            // Si es el enemigo más cercano hasta ahora
            if (distanciaAlEnemigo < distanciaMasCercana)
            {
                enemigoMasCercano = enemigo;
                distanciaMasCercana = distanciaAlEnemigo;
            }
        }

        // Si se encontró un enemigo cercano
        if (enemigoMasCercano != null)
        {
            SeguirYAtacar(enemigoMasCercano);
        }
    }

    void SeguirYAtacar(UnidadColocada enemigo)
    {
        if (enemigo == null) return;
        // Calcular la distancia al enemigo
        float distanciaAlEnemigo = Mathf.Sqrt((enemigo.transform.position - transform.position).sqrMagnitude);

        // Si el enemigo está fuera del rango
        if (distanciaAlEnemigo > rangoActual)
        {
            if (estado == Estado.Atacando) FinalizarAtaque(); // Si está atacando, se finaliza el ataque

            // Calcular la dirección hacia el enemigo
            Vector3 direccionAlEnemigo = (enemigo.transform.position - transform.position).normalized;
            // Calcular el punto objetivo manteniendo una distancia mínima del enemigo
            Vector3 posicionObjetivo = enemigo.transform.position - direccionAlEnemigo * (rangoActual - 0.5f);
            // Moverse hacia el punto objetivo
            UnidadMovilColocada estaUnidad = GetComponent<UnidadMovilColocada>();
            estaUnidad.MoverUnidad(posicionObjetivo);
        }
        else
        {
            // Atacar al enemigo más cercano
            ComenzarAtaque(enemigo.VidaSystem);
        }
    }

    public void ComenzarAtaque(UnidadVida unidadAtacada)
    {
        // Comprobacion para evitar que cambie de objetivo 
        if(estado == Estado.Atacando) return;
        
        // Obtener su unidad de combate
        UnitCombat unidadDeCombateAtacada = unidadAtacada.GetComponent<UnidadMovilColocada>()?.CombatSystem;

        UnidadColocada unidadColocadaPropia = GetComponent<UnidadColocada>();
        //calcular la distancia
        float distanciaAlObjetivo = (transform.position - unidadAtacada.transform.position).magnitude;

        //Comprobar si esta en rango de ataque
        // Decidir el modo
        if (tipoDeAtaque == CombatType.Melee && distanciaAlObjetivo <= rangoMelee)
        {
            estado = Estado.Atacando; // Activar modo de ataque
            if (unidadColocadaPropia != null) unidadDeCombateAtacada.NotificarAtaque(unidadColocadaPropia); // Notificar del ataque a la unidad atacada
            StartCoroutine(AtacarMele(unidadAtacada));
        }
        else if (tipoDeAtaque == CombatType.Distancia && distanciaAlObjetivo <= rangoDistancia)
        {
            estado = Estado.Atacando;
            if (unidadColocadaPropia != null) unidadDeCombateAtacada.NotificarAtaque(unidadColocadaPropia);
            StartCoroutine(DispararFlecha(unidadAtacada));
        }

    }

    public void FinalizarAtaque()
    {
        //Pierde el foco
        estado = Estado.Descansando;
    }

    private IEnumerator AtacarMele(UnidadVida unidadAtacada)
    {
        unidadAtacada.RecibirDaño(daño);
        yield return new WaitForSeconds(EsperaEntreAtaque);

        // Si continua en rango lo sigue atacando
        // Calcular la distancia
        float distanciaAlObjetivo = (transform.position - unidadAtacada.transform.position).magnitude;
        if(distanciaAlObjetivo <= rangoMelee)
        {
            StartCoroutine(AtacarMele(unidadAtacada));
        }
        else estado = Estado.Descansando;
    }

    private IEnumerator DispararFlecha(UnidadVida unidadAtacada)
    {
        // Spawn y configuracion de la flecha
        Arrow flecha = Instantiate(flechaPrefab, transform.position, Quaternion.identity);
        flecha.UnidadObjetivo = unidadAtacada;
        flecha.Equipo = Team.Aliado;
        yield return new WaitForSeconds(EsperaEntreAtaque);
        
        // Calcular la distancia
        float distanciaAlObjetivo = (transform.position - unidadAtacada.transform.position).magnitude;
        // Si continua en rango lo sigue atacando
        if(distanciaAlObjetivo <= rangoDistancia)
        {
            StartCoroutine(DispararFlecha(unidadAtacada));
        }
        else FinalizarAtaque();
    }

    private void OnDrawGizmos() 
    {
        if(tipoDeAtaque == CombatType.Melee)
        {
            Gizmos.DrawWireSphere(transform.position,rangoMelee);

        }
        else if (tipoDeAtaque == CombatType.Distancia)
        {
            Gizmos.DrawWireSphere(transform.position,rangoDistancia);
        }
    }

    public void DivisarEnemigo(UnidadColocada enemigoDivisado)
    {
        unidadesEnemigasDivisadas.Add(enemigoDivisado);
    }

    public void PerderEnemigo(UnidadColocada unidadPerdida)
    {
        unidadesEnemigasDivisadas.Remove(unidadPerdida);
    }

    public void CambiarComportamiento(Comportamiento nuevoComportamiento)
    {   
        //Finalizar ataque actual
        FinalizarAtaque();
        //Asignar el nuevo comportamiento
        comportamientoActual = nuevoComportamiento;
    } 

    public void NotificarAtaque(UnidadColocada unidadAtacante)
    {   
        if(unidadAtacante == null) return;

        if(comportamientoActual == Comportamiento.Defensivo || comportamientoActual == Comportamiento.Neutral)
        {
            if(estado != Estado.Defendiendo)
            {
                // Finalizar ataque actual
                FinalizarAtaque();
                // Empezar a defenderse
                estado = Estado.Defendiendo;
                // Guardar la posicion original
                posicionOriginal = transform.position;
                StartCoroutine(AtaqueYSeguimientoCortoDefensivo(unidadAtacante));
            }
        }
    }
}
