using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float Daño;
    public float Velocidad;
    public Team Equipo;
    public UnidadVida UnidadObjetivo;
    private Vector3 posicionObjetivo;

    private void OnEnable() 
    { 
        // Comenzar autodestruccion en 3 segundos (vida maxima de la flecha)
        StartCoroutine(ComenzarAutodestruccion());
    }

    void FixedUpdate() 
    {
        //Entra aqui hasta que tiene una posicion objetivo
        if(posicionObjetivo == null && UnidadObjetivo == null)
        {
            return;
        }
        else if(UnidadObjetivo != null)
        {
            posicionObjetivo = UnidadObjetivo.transform.position;
        }

        // Comenzar a dirigir la flecha
        PointTowardsTargetRigidbody();

        if ((transform.position - UnidadObjetivo.transform.position).sqrMagnitude < 0.1f)
        {
            Impactar();
        }
    }

    private void PointTowardsTargetRigidbody() 
    {
        Vector3 direction = (posicionObjetivo - transform.position).normalized;
        transform.forward = direction;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.MovePosition(rb.position + transform.forward * Velocidad * Time.fixedDeltaTime);
    }

    private void Impactar()
    {
        // Comprobacion de seguridad
        if (UnidadObjetivo == null || UnidadObjetivo.gameObject.GetComponent<UnidadColocada>().Equipo == Equipo) 
        {
            return;
        }

        // Si impacta contra una unidad colocada y esta es de otro equipo
        if (UnidadObjetivo.gameObject.GetComponent<UnidadColocada>().Equipo  != Equipo)
        {
            // Le hace daño y se destruye
            UnidadObjetivo.GetComponent<UnidadVida>().RecibirDaño(Daño);
            Destroy(gameObject); // Usa gameObject para referirte al objeto al que está adjunto el script
        }
    }

    IEnumerator ComenzarAutodestruccion()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
