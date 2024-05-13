
using UnityEngine;

public enum Team
{
    Aliado,
    Enemigo
}


public class UnidadColocada : MonoBehaviour
{
    [HideInInspector] public Sprite Icono;
    public string Name;
    [HideInInspector] public string Descripcion;
    public Team Equipo = Team.Aliado;
    public UnidadVida VidaSystem { get; private set; }
    
    protected virtual void Start() 
    {
        VidaSystem = GetComponent<UnidadVida>();
    }

    public virtual void SeleccionarUnidad()
    {
        
    }
}
