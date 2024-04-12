using System;
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


    [Header("Recursos Necesarios")]
    public int CostoOro;
    public int CostoMadera;
    public int CostoPiedra;
    public int CostoMetal;

    //public Tecnologia[] TecnologiasNecesarias;


}
