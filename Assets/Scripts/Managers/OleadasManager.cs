using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Dificultad
{
    Facil,
    Normal,
    Dificil,
}


public class OleadasManager : Singleton<OleadasManager>
{
    public static Action<int> EventoOleadaAdelantada;

    //Enemigos que se spawnearan.
    [SerializeField] private List<UnidadMovilColocada> unidadesASpawnear;
    
    // Puntos de spawn
    [SerializeField] private List<Vector3> puntosDeSpawn = new List<Vector3>();

    // Dificultad
    [SerializeField] private Dificultad dificultad;
    //Configuracion de oleadas
    [SerializeField] private ConfiguracionOleada configuracionOleadaFacil;
    [SerializeField] private ConfiguracionOleada configuracionOleadaNormal;
    [SerializeField] private ConfiguracionOleada configuracionOleadaDificil;
    private ConfiguracionOleada configuracionActual;
    
    // Parametros privados:

    // Enemigos a spawnear para la siguiente oleada
    int cantidadEnemigosASpawnear;

    // Lista de enemigos (Para guardar a todos los que deben ser neutralizados y saber cuando termina la oleada)
    private List<UnidadMovilColocada> enemigosVivos;
    
    // Tiempo hasta la siguiente oleada
    private int segundosParaOleada;
    // Oleada actual
    public int OleadaActual { get; private set; } = 1;

    private void ConfigurarSiguienteOleada()
    {
        //Configurar primer oleada:
        OleadaActual ++;
        cantidadEnemigosASpawnear = (int)(configuracionActual.cantidadDeEnemigos * configuracionActual.multiplicadorEnemigos * OleadaActual - 1);

        segundosParaOleada = (int)(configuracionActual.minutosMinimoParaOleada * 60 * OleadaActual - 1);
        StartCoroutine(ContinuarCuentaRegresiva()); // Iniciar la cuenta regresiva para la siguiente oleada
        ActualizarUI();
    }

    void SpawnearEnemigos()
    {
        if (unidadesASpawnear.Count == 0 || puntosDeSpawn.Count == 0)
        {
            Debug.LogError("No hay unidades o puntos de spawn configurados.");
            return;
        }

        for (int i = 0; i < cantidadEnemigosASpawnear; i++)
        {
            // Elegir una unidad aleatoria de la lista
            int indiceUnidad = UnityEngine.Random.Range(0, unidadesASpawnear.Count);
            UnidadMovilColocada unidad = unidadesASpawnear[indiceUnidad];

            // Elegir un punto de spawn aleatorio
            int indiceSpawn = UnityEngine.Random.Range(0, puntosDeSpawn.Count);
            Vector3 posicionSpawn = puntosDeSpawn[indiceSpawn];

            // Instanciar la unidad en el punto de spawn
            UnidadMovilColocada nuevaUnidad = Instantiate(unidad, posicionSpawn, Quaternion.identity);

            // Agregar la nueva unidad a la lista de enemigos vivos
            enemigosVivos.Add(nuevaUnidad);
            ActualizarUI();
        }
    }

    void ComprobarSiVivenEnemigos(UnidadColocada unidadAComprobar)
    {
        UnidadMovilColocada unidadMovilMuerta = unidadAComprobar.GetComponent<UnidadMovilColocada>();

        if (unidadMovilMuerta != null && enemigosVivos.Contains(unidadMovilMuerta))
        {
            enemigosVivos.Remove(unidadMovilMuerta);

            // No quedan enemigos vivos, iniciar la siguiente oleada
            if (enemigosVivos.Count == 0)
            {
                // Notificar a party manager.
                PartyManager.Instance.SumarPuntoPorOleadaDerrotada();

                ConfigurarSiguienteOleada(); // Configurar la siguiente oleada (ajustar dificultad, etc.)
            }
            else
            {
                // Actualizar la UI para mostrar la cantidad de enemigos restantes
                ActualizarUI();
            }
        }
    }

    void ActualizarUI()
    {
        UIManager.Instance.ConfigurarOleadaUI(OleadaActual, segundosParaOleada, enemigosVivos?.Count ?? 0);
    }

    IEnumerator ContinuarCuentaRegresiva()
    {
        yield return new WaitForSeconds(1);
        
        // Restar un segundo para la siguiente oleada
        segundosParaOleada --;
        //Conmfigurar la interface visual
        ActualizarUI();
        // Si el tiempo es <= 0 entonces comienza la siguiente oleada
        if(segundosParaOleada <= 0)
        {
            SpawnearEnemigos();
        }
        else
        {
            StartCoroutine(ContinuarCuentaRegresiva());
        }
    }

    public void SetDificultad(Dificultad _dificultad)
    {
        dificultad = _dificultad;
       // Inicializar Variables
        enemigosVivos = new List<UnidadMovilColocada>();

        switch (dificultad)
        {
            case Dificultad.Facil:
                configuracionActual = configuracionOleadaFacil;
                break;
            case Dificultad.Normal:
                configuracionActual = configuracionOleadaNormal;
                break;
            case Dificultad.Dificil:
                configuracionActual = configuracionOleadaDificil;
                break;

        }

        //Configurar primer oleada:
        OleadaActual = 1;
        cantidadEnemigosASpawnear = configuracionActual.cantidadDeEnemigos;
        segundosParaOleada = (int)(configuracionActual.minutosMinimoParaOleada * 60);
        StartCoroutine(ContinuarCuentaRegresiva());
    }

    public void AdelantarOleada()
    {
        if (enemigosVivos.Count == 0 && segundosParaOleada > 0)
        {
            EventoOleadaAdelantada?.Invoke(segundosParaOleada); // Invoca el evento pasando el tiempo restante
            segundosParaOleada = 0;
        }
    }

    // Método para dibujar los gizmos en el editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Puedes cambiar el color si quieres

        foreach (Vector3 punto in puntosDeSpawn)
        {
            Gizmos.DrawSphere(punto, 0.5f); // Dibuja una esfera en cada punto de spawn
        }
    }

    #region Eventos
    private void OnEnable() 
    {
        //Suscribir al evento de muerte de unidades
        UnidadVida.EventoUnidadDerrotada += ComprobarSiVivenEnemigos;
    }

    private void OnDisable() 
    {
        UnidadVida.EventoUnidadDerrotada -= ComprobarSiVivenEnemigos;
    }
    #endregion
}

[Serializable]
struct ConfiguracionOleada
{
    public float minutosMinimoParaOleada;
    public float multiplicadorEnemigos;
    public float multiplicadorTiempoMinimo;
    public int cantidadDeEnemigos;
    public int minutosMaxEntreOleadas;
}