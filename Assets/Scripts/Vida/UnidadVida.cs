using System;
using UnityEngine;

public class UnidadVida : VidaBase
{
    public static Action<UnidadColocada> EventoUnidadDerrotada;

    [Header("Vida")]
    [SerializeField] private UnidadBarraVida barraVidaPrefab;
    [SerializeField] private Transform barraVidaPosicion;

    private UnidadBarraVida _enemigoBarraVidaCreada;

    protected override void Start()
    {
        base.Start();
        CrearBarraVida();
        
    }

    private void Update() 
    {
        // Si la vida esta al maximo no muestra la barra.
        if (Salud >= saludMax)
        {
            _enemigoBarraVidaCreada.GetComponent<Canvas>().enabled = false;
        }
        else
        {
            _enemigoBarraVidaCreada.GetComponent<Canvas>().enabled = true;
        }
    }

    private void CrearBarraVida()
    {
        _enemigoBarraVidaCreada = Instantiate(barraVidaPrefab, barraVidaPosicion);
        ActualizarBarraVida(Salud, saludMax);
    }

    protected override void ActualizarBarraVida(float vidaActual, float vidaMaxima)
    {
        _enemigoBarraVidaCreada.ModificarSalud(vidaActual,vidaMaxima); 
    }

    protected override void PersonajeDerrotado()
    {
        var unidadColocada = gameObject.GetComponent<UnidadColocada>();
        if (unidadColocada != null)
        {
            unidadColocada.DeseleccionarUnidad();
            EventoUnidadDerrotada?.Invoke(unidadColocada);
        }
        //Se destruye
        Destroy(gameObject);
    }
}
