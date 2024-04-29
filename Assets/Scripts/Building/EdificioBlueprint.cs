using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu]
public class EdificioBlueprint : ScriptableObject
{
    [Header("Config")]
    public Sprite Icono;
    public string Name;
    [TextArea(3, 10)] public string Descripcion;
    public EdificioColocado EdificioColocadoPrefab;
    public List<AccionDeEdificioConfig> unidadesDisponiblesConfigs;
    public List<AccionDeEdificioConfig> investigacionesDisponiblesConfigs;
    public int colaMaximaAcciones = 5;

    [Header("Recursos Necesarios")]
    public int CostoOro;
    public int CostoMadera;
    public int CostoPiedra;
    public int CostoMetal;
    
    [Header("Tiempo Necesario")]
    public int TiempoDeConstruccion;
}
