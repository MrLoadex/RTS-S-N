using UnityEngine;

public enum TipoAccion
{
    Investigacion,
    CreacionDeUnidad
}

[CreateAssetMenu]
public class AccionDeEdificioConfig : ScriptableObject
{
    public TipoAccion Tipo;
    public Sprite Icono;
    public string ID;
    public string Name;
    [TextArea(3, 10)] public string Descripcion;
    public int TiempoRequerido;
    public int CostoOro;
    public int CostoMadera;
    public int CostoPiedra;
    public int CostoMetal;
    public UnidadMovilColocada UnidadPorSpawnearPrefab;


}