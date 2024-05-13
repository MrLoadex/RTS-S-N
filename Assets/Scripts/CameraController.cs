using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float maxZoom = 20;
    [SerializeField] private float minZoom = 2;

    Camera cam;
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
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0, Space.World);
        }
    }
}
