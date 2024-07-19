using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum AccionesConSonido
{
    Descansar,
    Desplazar,
    ImpactarPiedraOMadera,
    ImpactarMetalUOro,
    ImpactarSlime,
}

[Serializable]
struct SonidoYAccion
{
    public AccionesConSonido accion;
    public AudioClip sonido;
}

public class UnidadSonora : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Fuente de audio para reproducir los sonidos
    [SerializeField] private List<SonidoYAccion> sonidosYAcciones; // Lista de sonidos y acciones

    // Diccionario para un acceso rápido a los sonidos por acción
    private Dictionary<AccionesConSonido, AudioClip> diccionarioSonidos;

    void Start()
    {
        // Avisarle al sonido manager de su existencia
        SonidoManager.Instance.addUnidadSonora(this);

        // Inicializar el diccionario
        diccionarioSonidos = new Dictionary<AccionesConSonido, AudioClip>();
        
        foreach (SonidoYAccion sonidoYAccion in sonidosYAcciones)
        {
            diccionarioSonidos[sonidoYAccion.accion] = sonidoYAccion.sonido;
        }
    }

    void Update()
    {
        // Este método puede ser utilizado para escuchar eventos y reproducir sonidos
    }

    // Método para reproducir un sonido basado en la acción
    void ReproducirSonido(AccionesConSonido accion)
    {
        if (diccionarioSonidos.TryGetValue(accion, out AudioClip sonido))
        {
            //if (audioSource.isPlaying && audioSource.clip.name == sonido.name) return;
            if (audioSource.isPlaying) return;
            audioSource.PlayOneShot(sonido);
        }
        else
        {
            Debug.LogWarning($"No se encontró sonido para la acción: {accion}");
        }
    }

    // Ejemplo de cómo podrías llamar a este método desde otro script
    void EjecutarAccion(AccionesConSonido accion)
    {
        ReproducirSonido(accion);
    }

    #region Eventos
    private void OnEnable() 
    {
        RecursoColocado.EventoRecursoExtrayendose += ResponderEventoRecursoExtrayendose;
        EdificioColocado.EventoConstruyendoEdificio += ResponderEventoConstruyendoEdificio;
        UnitsManager.EventoUnidadControladaPorUsuario += ResponderEventoUnidadControladaPorUsuario;
    }

    private void OnDisable() 
    {
        RecursoColocado.EventoRecursoExtrayendose -= ResponderEventoRecursoExtrayendose;
        EdificioColocado.EventoConstruyendoEdificio -= ResponderEventoConstruyendoEdificio;
    }

    private void ResponderEventoRecursoExtrayendose(UnidadMovilColocada unidadExtractora, TipoRecurso tipoRecurso)
    {
        if (this == null) return;
        if (unidadExtractora != gameObject.GetComponent<UnidadMovilColocada>()) return;
       
        switch (tipoRecurso)
        {
            case TipoRecurso.Madera :
                EjecutarAccion(AccionesConSonido.ImpactarPiedraOMadera);
                break;
            case TipoRecurso.Piedra :
                EjecutarAccion(AccionesConSonido.ImpactarPiedraOMadera);
                break;
            case TipoRecurso.Metal :
                EjecutarAccion(AccionesConSonido.ImpactarMetalUOro);
                break;
            case TipoRecurso.Oro :
                EjecutarAccion(AccionesConSonido.ImpactarMetalUOro);
                break;
        }

    }

    private void ResponderEventoConstruyendoEdificio(UnidadMovilColocada unidadConstructora)
    {
        if (this == null) return;
        if (unidadConstructora != gameObject.GetComponent<UnidadMovilColocada>()) return;
        EjecutarAccion(AccionesConSonido.ImpactarPiedraOMadera);
    }
    
    private void ResponderEventoUnidadControladaPorUsuario(UnidadMovilColocada unidadDesplazada)
    {
        if (this == null) return;
        if (unidadDesplazada != gameObject.GetComponent<UnidadMovilColocada>()) return;
        EjecutarAccion(AccionesConSonido.Desplazar);
    }
    #endregion
}
