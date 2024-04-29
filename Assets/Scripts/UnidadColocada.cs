
using UnityEngine;
using UnityEngine.UI;

public class UnidadColocada : MonoBehaviour
{
    public Sprite Icono;
    public string Name;
    [TextArea(3, 10)] public string Descripcion;
    
    public virtual void SeleccionarUnidad()
    {
        
    }
}
