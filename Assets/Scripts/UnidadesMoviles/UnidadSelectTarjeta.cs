using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnidadSelectTarjeta : MonoBehaviour
{
    public UnidadMovilColocada Unidad { get; private set; }
    [SerializeField] private Image icono;
    [SerializeField] private Image barraVida;
    // Start is called before the first frame update
    public void ConfigurarTarjeta(UnidadMovilColocada pUnidad)
    {
        Unidad = pUnidad;
        icono.sprite = Unidad.Icono;
        UnidadVida vidaDeUnidad = Unidad.VidaSystem;
        if (vidaDeUnidad == null)
        {
            Debug.Log("Es null");
        }
        //barraVida.fillAmount = (vidaDeUnidad.Salud / vidaDeUnidad.SaludMax);
    }

    public void UnselectUnit()
    {
        // Obtener una lista de las unidades seleccionadas
        List<UnidadMovilColocada> unidadesSelect = SelectManager.Instance.UnidadesMovilesSeleccionadas;

        foreach (var unidad in unidadesSelect)
        {
            // Comprobar si la unidad esta seleccionada
            if(unidad == Unidad)
            {
                // Deseleccionarla, actualizar la interface y finalizar el bucle
                unidadesSelect.Remove(Unidad);
                UIManager.Instance.ActualizarUIUnidadesSeleccionadas(unidadesSelect);
                break;
            }
        }
    }
}
