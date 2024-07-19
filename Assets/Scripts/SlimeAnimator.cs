using System;
using System.Collections;
using UnityEngine;

enum AccionDeAnimacion
{
    Descansar,
    Desplazar,
    Atacar,
}

public class SlimeAnimator : MonoBehaviour
{
    public static Action<UnidadMovilColocada> EventoUnidadEnMovimiento;

    [SerializeField] private float tamañoMin = 0.5f;
    [SerializeField] private float tamañoMax = 1.2f;
    [SerializeField] private float velocidadCambio = 1.0f; // Velocidad de cambio de tamaño
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private GameObject arma; // El arma que rota al atacar
    [SerializeField] private float rotacionArma = 30f; // Grados de rotación del arma
    [SerializeField] private float amplitudSalto = 0.1f; // Altura de los pequeños saltos
    [SerializeField] private float velocidadDescanso = 0.2f; // Velocidad de la animación de descanso
    [SerializeField] private float velocidadSalto = 2.0f; // Velocidad de los saltos
    [SerializeField] private LayerMask capaTerreno; // Capa del terreno para el raycast
    [SerializeField] private float correccionAlturaPersonaje = 0.2f; // Capa del terreno para el raycast

    private AccionDeAnimacion accionActual;
    private bool desplazandose = false;
    private Vector3 tamañoOriginal;
    private Vector3 posicionAnterior;
    Quaternion rotacionDeArmaInicial;

    void Start()
    {
        rotacionDeArmaInicial = arma.transform.localRotation;
        tamañoOriginal = bodyTransform.localScale;
        posicionAnterior = transform.position;
    }

    void Update()
    {
        // Obtiene la actividad actual de la unidad de combate
        Actividad actividadActual = gameObject.GetComponent<UnitCombat>().ActivadadActual;

        // Comprueba la actividad actual de la unidad de combate y ajusta la animación
        if (actividadActual == Actividad.Atacar && accionActual != AccionDeAnimacion.Atacar)
        {
            if (accionActual != AccionDeAnimacion.Atacar)
            {
                Atacar(arma);
            }
        }

        // Comprueba si la posición ha cambiado
        if (transform.position != posicionAnterior)
        {
            if (!desplazandose)
            {
                Desplazarse();
            }
        }
        // Si la posición no ha cambiado, detén la animación de desplazamiento
        else if (desplazandose)
        {
            StopCoroutine(AnimarDesplazamiento());
            desplazandose = false;
            bodyTransform.localScale = tamañoOriginal;
            accionActual = AccionDeAnimacion.Descansar;
            Descansar();
        }
        // Actualiza la posición anterior. Se retrasa 1ms para evitar que la animación se corte permanentemente
        StartCoroutine(RetrasadorComprobarPos(transform.position));
    }

    IEnumerator RetrasadorComprobarPos(Vector3 nuevaPos)
    {
        yield return new WaitForSeconds(0.1f);
        posicionAnterior = nuevaPos;
    }

    void Desplazarse()
    {
        StartCoroutine(AnimarDesplazamiento());
    }

    IEnumerator AnimarDesplazamiento()
    {
        desplazandose = true;
        accionActual = AccionDeAnimacion.Desplazar;

        float progreso = 0f;

        while (transform.position != posicionAnterior) // Continúa mientras la unidad esté desplazándose
        {
            // Avisar que la unidad se esta moviendo
            EventoUnidadEnMovimiento?.Invoke(gameObject.GetComponent<UnidadMovilColocada>());

            // Simula pequeños saltos al moverse
            float salto = Mathf.Sin(progreso * Mathf.PI) * amplitudSalto; // Movimiento de salto sinusoidal

            // Realizar un raycast hacia abajo para detectar el terreno
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, capaTerreno))
            {
                float alturaTerreno = hitInfo.point.y;
                bodyTransform.position = new Vector3(transform.position.x, alturaTerreno + salto + correccionAlturaPersonaje, transform.position.z);
            }

            // Deformación para el despegue y aterrizaje
            if (salto > 0)
            {
                bodyTransform.localScale = Vector3.Lerp(new Vector3(tamañoMin, tamañoMax, tamañoMin), new Vector3(tamañoMax, tamañoMax, tamañoMax), salto);
            }
            else
            {
                bodyTransform.localScale = Vector3.Lerp(new Vector3(tamañoMax, tamañoMax, tamañoMax), new Vector3(tamañoMin, tamañoMax, tamañoMin), -salto);
            }

            progreso += Time.deltaTime * velocidadSalto;
            if (progreso > 2f) // Resetea el progreso cuando pasa un ciclo completo
            {
                progreso = 0f;
            }

            yield return null;
        }

        // Al terminar de desplazarse, vuelve al tamaño original
        bodyTransform.localScale = tamañoOriginal;
        desplazandose = false;
        accionActual = AccionDeAnimacion.Descansar;
    }

    void Descansar()
    {
        StartCoroutine(AnimarDescanso());
    }

    IEnumerator AnimarDescanso()
    {
        accionActual = AccionDeAnimacion.Descansar;
        float progreso = 0f;
        bool achicando = true;

        // Animación de respiración mientras descansa
        while (true)
        {
            if (achicando)
            {
                progreso += Time.deltaTime * velocidadDescanso;
                bodyTransform.localScale = Vector3.Lerp(new Vector3(tamañoMax, tamañoMax, tamañoMax), new Vector3(tamañoMin, tamañoMax, tamañoMin), progreso);
                if (progreso >= 1f)
                {
                    achicando = false;
                    progreso = 0f;
                }
            }
            else
            {
                progreso += Time.deltaTime * velocidadDescanso;
                bodyTransform.localScale = Vector3.Lerp(new Vector3(tamañoMin, tamañoMax, tamañoMin), new Vector3(tamañoMax, tamañoMax, tamañoMax), progreso);
                if (progreso >= 1f)
                {
                    achicando = true;
                    progreso = 0f;
                }
            }

            yield return null;
        }
    }

    void Atacar(GameObject arma)
    {
        StartCoroutine(AnimarAtaque(arma));
    }

    IEnumerator AnimarAtaque(GameObject arma)
    {
        accionActual = AccionDeAnimacion.Atacar;

        // Rotar el arma hacia atrás
        Quaternion rotacionObjetivo = Quaternion.Euler(rotacionArma, 0, 0) * rotacionDeArmaInicial;

        // Rotar el arma hacia adelante
        float progreso = 0f;
        while (progreso < 1f)
        {
            progreso += Time.deltaTime * velocidadCambio;
            arma.transform.localRotation = Quaternion.Lerp(rotacionDeArmaInicial, rotacionObjetivo, progreso);
            yield return null;
        }

        progreso = 0f;
        while (progreso < 1f)
        {
            progreso += Time.deltaTime * velocidadCambio;
            arma.transform.localRotation = Quaternion.Lerp(rotacionObjetivo, rotacionDeArmaInicial, progreso);
            yield return null;
        }

        // Asegurarse de que el arma esté en su posición original al final de la animación
        arma.transform.localRotation = rotacionDeArmaInicial;

        // Volver a estado de descanso después del ataque
        accionActual = AccionDeAnimacion.Descansar;
    }

    #region Eventos
    private void OnEnable() 
    {
        RecursoColocado.EventoRecursoExtrayendose += ResponderEventoRecursoExtrayendose;
        EdificioColocado.EventoConstruyendoEdificio += ResponderEventoConstruyendoEdificio;
    }

    private void OnDisable() 
    {
        RecursoColocado.EventoRecursoExtrayendose -= ResponderEventoRecursoExtrayendose;
        EdificioColocado.EventoConstruyendoEdificio -= ResponderEventoConstruyendoEdificio;
    }

    private void ResponderEventoRecursoExtrayendose(UnidadMovilColocada unidadExtractora, TipoRecurso tipoRecurso)
    {
        if (unidadExtractora != gameObject.GetComponent<UnidadMovilColocada>()) return;
       
        Atacar(arma);
    }

    private void ResponderEventoConstruyendoEdificio(UnidadMovilColocada unidadConstructora)
    {
        if (unidadConstructora != gameObject.GetComponent<UnidadMovilColocada>()) return;
        Atacar(arma);
    }
    #endregion
}
