using UnityEngine;

public class MovimientoCamaraMouse : MonoBehaviour
{
    public float sensibilidad = 0.1f;   // Ajusta la sensibilidad del movimiento
    public float suavizado = 5f;         // Ajusta la suavidad del movimiento
    public float anguloMaximoVertical = 45f; // Ángulo máximo en grados

    private Vector2 movimiento;
    private Vector2 velocidadMovimiento;
    private float anguloVerticalActual = 0f;

    void Update()
    {
        // Obtener el movimiento del mouse
        movimiento.x = Input.GetAxis("Mouse X") * sensibilidad;
        movimiento.y = Input.GetAxis("Mouse Y") * sensibilidad;

        // Suavizar el movimiento
        velocidadMovimiento = Vector2.Lerp(velocidadMovimiento, movimiento, suavizado * Time.deltaTime);

        // Limitar la rotación vertical
        anguloVerticalActual -= velocidadMovimiento.y;
        anguloVerticalActual = Mathf.Clamp(anguloVerticalActual, -anguloMaximoVertical, anguloMaximoVertical);

        // Aplicar el movimiento a la cámara (solo rotación horizontal)
        transform.Rotate(Vector3.up * velocidadMovimiento.x, Space.World);
        transform.localEulerAngles = new Vector3(anguloVerticalActual, transform.localEulerAngles.y, 0f);
    }
}
