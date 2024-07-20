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
    }

    public void UnselectUnit()
    {
        // Obtener una lista de las unidades seleccionadas
        SelectUnitsManager.Instance.UnselectUnit(Unidad);

    }
}
