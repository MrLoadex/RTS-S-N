using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float rotationSpeed = 100f;
    
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float minZoom = 2f;
    
    [SerializeField] private float rotationDistance = 20f; // Distancia regulable para el punto de rotación
    
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Zoom();
        Rotate();
    }

    void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        // Obtén los vectores forward y right, pero elimina la componente y (altura)
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();  // Normaliza para mantener la consistencia del movimiento

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();  // Normaliza para mantener la consistencia del movimiento

        Vector3 dir = forward * zInput + right * xInput;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void Zoom()
    {
        // Si se esta construyendo se deshabilita el zoom para evitar que la camara se mueva sin intencion
        if (BuilderManager.Instance.Construyendo) return;

        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");

        // Ajusta el orthographicSize de la cámara basado en el desplazamiento de la rueda del mouse
        cam.orthographicSize -= scrollAmount * zoomSpeed;

        // Opcional: Limita el tamaño ortográfico para evitar un zoom demasiado cercano o demasiado lejano
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    void Rotate()
    {
        Vector3 rotationPoint = transform.position + transform.forward * rotationDistance;

        if (Input.GetKey(KeyCode.Q))
        {
            RotateAroundPoint(rotationPoint, rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            RotateAroundPoint(rotationPoint, -rotationSpeed * Time.deltaTime);
        }
    }

    void RotateAroundPoint(Vector3 point, float angle)
    {
        // Gira la cámara alrededor del punto especificado
        transform.RotateAround(point, Vector3.up, angle);
    }

    void OnDrawGizmos()
    {
        // Dibuja un gizmo para representar el punto de rotación
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Vector3 rotationPoint = transform.position + transform.forward * rotationDistance;
            Gizmos.DrawLine(rotationPoint, rotationPoint + Vector3.up * 5f); // Línea vertical en el eje Y
        }
    }
}
