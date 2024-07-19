using System.Collections.Generic;
using UnityEngine;

public class SonidoManager : Singleton<SonidoManager>
{
    List<UnidadSonora> unidadSonoras = new List<UnidadSonora>();

    private void Start() {
        
    }

    public void addUnidadSonora(UnidadSonora unidadSonora)
    {
        unidadSonoras.Add(unidadSonora);
    }
}
