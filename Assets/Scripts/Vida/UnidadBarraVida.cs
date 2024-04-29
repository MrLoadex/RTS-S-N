using UnityEngine;
using UnityEngine.UI;

public class UnidadBarraVida : MonoBehaviour
{
    [SerializeField] private Image barraVida;

    private float saludActual;
    private float saludMax;

    private void Update() 
    {

        transform.LookAt(Camera.main.transform);
        barraVida.fillAmount = Mathf.Lerp(barraVida.fillAmount, 
                               saludActual / saludMax,  10f * Time.deltaTime);
    }

    

    public void ModificarSalud(float pSaludActual, float pSaludMax)
    {
        saludActual = pSaludActual;
        saludMax = pSaludMax;
    }
}
