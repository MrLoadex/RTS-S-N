using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnidadVida : VidaBase
{
    public static Action<float> EventoUnidadDerrotada;

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

    private void DesactivarEnemigo()
    {
        _enemigoBarraVidaCreada.gameObject.SetActive(false);
    }
}
