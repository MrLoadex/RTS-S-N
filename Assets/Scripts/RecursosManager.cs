using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursosManager : Singleton<RecursosManager>
{
    [SerializeField] private int gold;
    [SerializeField] private int wood;
    [SerializeField] private int stone;
    [SerializeField] private int metal;

    public int Gold => gold;
    public int Wood => wood;
    public int Stone => stone;
    public int Metal => metal;

    public bool ComprobarConstruccion(EdificioBlueprint edificio)
    {
        if (edificio == null) return false;

        if (edificio.CostoOro > gold || edificio.CostoMadera > wood || edificio.CostoPiedra > stone || edificio.CostoMetal > metal)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
