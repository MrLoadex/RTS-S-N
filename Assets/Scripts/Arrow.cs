using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float Daño;
    public float Velocidad;
    public Team Equipo;
    public UnidadVida UnidadVidaObjetivo;
    private Vector3 posicionObjetivo;

    private void OnEnable() 
    { 
        // Comenzar autodestruccion en 3 segundos (vida maxima de la flecha)
        StartCoroutine(ComenzarAutodestruccion());
    }

    void FixedUpdate() 
    {
        // Entra aqui hasta que tiene una posicion objetivo
        if(posicionObjetivo == null && UnidadVidaObjetivo == null)
        {
            return;
        }
        else if(UnidadVidaObjetivo != null)
        {
            posicionObjetivo = UnidadVidaObjetivo.transform.position;
        }

        // Comenzar a dirigir la flecha
        PointTowardsTargetRigidbody();

        if (UnidadVidaObjetivo.IsDestroyed()) Destroy(gameObject);

        if ((transform.position - UnidadVidaObjetivo?.transform.position)?.sqrMagnitude < 0.1f)
        {
            Impactar();
        }
    }

    private void PointTowardsTargetRigidbody() 
    {
        if (UnidadVidaObjetivo == null) Destroy(gameObject);

        Vector3 direction = (posicionObjetivo - transform.position).normalized;
        transform.forward = direction;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.MovePosition(rb.position + transform.forward * Velocidad * Time.fixedDeltaTime);
    }

    private void Impactar()
    {
        UnidadColocada unidadColocada = UnidadVidaObjetivo?.gameObject.GetComponent<UnidadColocada>();
        // Comprobacion de seguridad
        if (UnidadVidaObjetivo == null || unidadColocada == null || unidadColocada.Equipo == Equipo || unidadColocada.Equipo == Team.Neutral) 
        {
            return;
        }

        // Si impacta contra una unidad colocada y esta es de otro equipo
        if (unidadColocada.Equipo != Equipo)
        {
            // Le hace daño y se destruye
            UnidadVidaObjetivo?.RecibirDaño(Daño);
            Destroy(gameObject); // Usa gameObject para referirte al objeto al que está adjunto el script
        }
    }

    IEnumerator ComenzarAutodestruccion()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
