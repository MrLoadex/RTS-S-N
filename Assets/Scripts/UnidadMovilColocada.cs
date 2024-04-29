using System;
using UnityEngine;


[Serializable]
public enum TipoUnidadMovil
{
    Aldeano,
    Guerrero
}

public class UnidadMovilColocada : UnidadColocada
{
    public TipoUnidadMovil Tipo;
    //Mover unidad
    public void MoverUnidad(Vector3 posicionObjetivo)
    {
        //aplicar logica
    }
}
