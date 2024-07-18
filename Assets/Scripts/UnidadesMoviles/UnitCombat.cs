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

public enum Estado
{
    Pasivo,
    Agresivo,
    Neutral,
    Defensivo
}

public enum Actividad
{
    Atacando,
    Defendiendo,
    Siguiendo,
    Descansando,
    Controlado
}

public class UnitCombat : MonoBehaviour
{
    [Header ("Config")]
    [SerializeField] private CombatType tipoDeAtaque;
    [SerializeField] private float daño = 10;
    [SerializeField] private float EsperaEntreAtaque = 0.5f;
    [SerializeField] private Arrow flechaPrefab;
    [SerializeField] private float rangoAtaque = 1f;
    [SerializeField] private float rangoVision = 10f;


    // PROPS
    //public CombatType TipoDeAtaque =>tipoDeAtaque;
    public float Daño => daño;

    List<UnidadColocada> unidadesEnemigasDivisadas = new List<UnidadColocada>();
    Actividad actividad = Actividad.Descansando;
    Vector3 posicionOriginal;

    public Estado estadoActual;

    private void Update() 
    {
        // Si se esta viendo a algun enemigo y no se esta en pasivo
        if(unidadesEnemigasDivisadas.Count > 0 && estadoActual != Estado.Pasivo)
        {
            switch (estadoActual)
            {
                case Estado.Neutral:
                AtacarSiEsVisible();
                break;
                case Estado.Agresivo:
                ObservarSeguirYAtacar();
                break;
            }
        }
    }

    private void AtacarSiEsVisible()
    {

        if (actividad == Actividad.Descansando)
        {
            UnidadColocada enemigoMasCercano = null;
            float distanciaMasCercana = Mathf.Infinity;

            foreach (var enemigo in unidadesEnemigasDivisadas)
            {
                if (enemigo == null)
                {
                    unidadesEnemigasDivisadas.Remove(enemigo);
                    break;
                }
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
                actividad = Actividad.Descansando;
                // Guardar la posicion original
                posicionOriginal = transform.position;
                actividad = Actividad.Siguiendo;
                StartCoroutine(AtaqueYSeguimientoCortoNeutral(enemigoMasCercano));
            }
        }
    }

    private IEnumerator AtaqueYSeguimientoCortoDefensivo(UnidadColocada enemigo)
    {
        if (enemigo == null) yield break;

        SeguirYAtacar(enemigo.GetComponent<UnidadMovilColocada>());

        yield return new WaitForSeconds(0.5f);

        // Si se movio demasiado de su posicion original
        if((posicionOriginal - transform.position).magnitude > rangoVision|| enemigo == null)
        {
            // Mover a la posicion original
            GetComponent<UnidadMovilColocada>().MoverUnidad(posicionOriginal);
            // Finalizar la defensa
            actividad = Actividad.Descansando;
        }
        else
        {
            // Continuar con al coroutine
            StartCoroutine(AtaqueYSeguimientoCortoDefensivo(enemigo));
        }
    }

    private IEnumerator AtaqueYSeguimientoCortoNeutral(UnidadColocada enemigo)
    {
        if (enemigo == null) yield break;

        SeguirYAtacar(enemigo.GetComponent<UnidadMovilColocada>());

        yield return new WaitForSeconds(0.5f);

        // Si se movio demasiado de su posicion original
        if((posicionOriginal - transform.position).magnitude > rangoVision || enemigo == null)
        {
            // Mover a la posicion original
            GetComponent<UnidadMovilColocada>().MoverUnidad(posicionOriginal);

            yield return new WaitForSeconds(5f);
            actividad = Actividad.Descansando;
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
            if (enemigo == null)
            {
                unidadesEnemigasDivisadas.Remove(enemigo);
                break;
            }
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

        // Si el enemigo está fuera del rango de ataque
        if (distanciaAlEnemigo > rangoAtaque)
        {
            // SEGUIR ENEMIGO

            if (actividad == Actividad.Atacando) actividad = Actividad.Siguiendo; // Si está atacando, 

            // Calcular la dirección hacia el enemigo
            Vector3 direccionAlEnemigo = (enemigo.transform.position - transform.position).normalized;
            // Calcular el punto objetivo manteniendo una distancia mínima del enemigo
            Vector3 posicionObjetivo = enemigo.transform.position - direccionAlEnemigo * (rangoAtaque - 0.5f);
            // Moverse hacia el punto objetivo
            UnidadMovilColocada estaUnidad = GetComponent<UnidadMovilColocada>();
            estaUnidad.MoverUnidad(posicionObjetivo);
        }
        else
        {
            // ATACAR
            ComenzarAtaque(enemigo.VidaSystem);
        }
    }

    public void ComenzarAtaque(UnidadVida unidadAtacada)
    {
        if (unidadAtacada == null) return;
        // Comprobacion para evitar que cambie de objetivo 
        if(actividad == Actividad.Atacando) return;
        
        // Obtener su unidad de combate
        UnitCombat unidadDeCombateAtacada = unidadAtacada.GetComponent<UnidadMovilColocada>()?.CombatSystem;

        UnidadColocada unidadColocadaPropia = GetComponent<UnidadColocada>();
        //calcular la distancia
        float distanciaAlObjetivo = (transform.position - unidadAtacada.transform.position).magnitude;

        //Comprobar si esta en rango de ataque
        // Decidir el modo
        if (tipoDeAtaque == CombatType.Melee && distanciaAlObjetivo <= rangoAtaque)
        {
            actividad = Actividad.Atacando; // Activar modo de ataque
            unidadDeCombateAtacada?.NotificarAtaque(unidadColocadaPropia); // Notificar del ataque a la unidad atacada
            StartCoroutine(AtacarMele(unidadAtacada));
        }
        else if (tipoDeAtaque == CombatType.Distancia && distanciaAlObjetivo <= rangoAtaque)
        {
            actividad = Actividad.Atacando;
            unidadDeCombateAtacada.NotificarAtaque(unidadColocadaPropia);
            StartCoroutine(DispararFlecha(unidadAtacada));
        }

    }

    private IEnumerator AtacarMele(UnidadVida unidadAtacada)
    {
        if (unidadAtacada == null) yield break;

        unidadAtacada.RecibirDaño(daño);
        yield return new WaitForSeconds(EsperaEntreAtaque);

        if (unidadAtacada == null) yield break;
        // Si continua en rango lo sigue atacando
        // Calcular la distancia
        float distanciaAlObjetivo = (transform.position - unidadAtacada.transform.position).magnitude;
        if(distanciaAlObjetivo <= rangoAtaque && distanciaAlObjetivo > 0)
        {
            StartCoroutine(AtacarMele(unidadAtacada));
        }
        else actividad = Actividad.Descansando;
    }

    private IEnumerator DispararFlecha(UnidadVida unidadAtacada)
    {
        if (unidadAtacada == null) yield break;
        // Spawn y configuracion de la flecha
        Arrow flecha = Instantiate(flechaPrefab, transform.position, Quaternion.identity);
        flecha.UnidadVidaObjetivo = unidadAtacada;
        flecha.Equipo = Team.Aliado;
        yield return new WaitForSeconds(EsperaEntreAtaque);
        if (unidadAtacada == null) yield break;
        
        // Calcular la distancia
        float distanciaAlObjetivo = (transform.position - unidadAtacada.transform.position).magnitude;
        // Si continua en rango lo sigue atacando
        if(distanciaAlObjetivo <= rangoAtaque)
        {
            StartCoroutine(DispararFlecha(unidadAtacada));
        }
        else actividad = Actividad.Descansando;
    }

    private void OnDrawGizmos() 
    {
        if(tipoDeAtaque == CombatType.Melee)
        {
            Gizmos.DrawWireSphere(transform.position,rangoAtaque);

        }
        else if (tipoDeAtaque == CombatType.Distancia)
        {
            Gizmos.DrawWireSphere(transform.position,rangoAtaque);
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

    public void CambiarComportamiento(Estado nuevoComportamiento)
    {   
        //Asignar el nuevo comportamiento
        estadoActual = nuevoComportamiento;
    } 

    public void NotificarAtaque(UnidadColocada unidadAtacante)
    {   
        if(unidadAtacante == null) return;

        if(estadoActual == Estado.Defensivo || estadoActual == Estado.Neutral)
        {
            if(actividad == Actividad.Descansando)
            {
                // Empezar a defenderse
                actividad = Actividad.Defendiendo;
                // Guardar la posicion original
                posicionOriginal = transform.position;

                // Detener todas las coroutines
                StopAllCoroutines();
                // Comenzar con la coroutine de defensa
                StartCoroutine(AtaqueYSeguimientoCortoDefensivo(unidadAtacante));
            }
        }
    }

    void AccionDeUsuario(UnidadColocada unidadColocada)
    {
        if (unidadColocada != GetComponent<UnidadColocada>()) return;

        if(actividad == Actividad.Controlado) return;

        StopAllCoroutines();
        StartCoroutine(PasivoPorAlgunosSegundos());
    }

    IEnumerator PasivoPorAlgunosSegundos()
    {
        var estadoAnterior = estadoActual;
        //Poner en pasivo por algunos segundos
        estadoActual = Estado.Pasivo;
        actividad = Actividad.Controlado;

        yield return new WaitForSeconds(3);
        //Volver a ponerse como correpsonde
        estadoActual = estadoAnterior;
        actividad = Actividad.Descansando;
    }

    private void OnEnable() 
    {
        UnitsManager.EventoUnidadControladaPorUsuario += AccionDeUsuario;
    }

    private void OnDisable() 
    {
        UnitsManager.EventoUnidadControladaPorUsuario -= AccionDeUsuario;
    }
}
