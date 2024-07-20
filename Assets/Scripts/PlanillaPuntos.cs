using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlanillaPuntajes : ScriptableObject
{
    [SerializeField] private int maxPuntajes = 100;
    [SerializeField] private List<Puntaje> puntajes = new List<Puntaje>();
    public List<Puntaje> Puntajes => puntajes;

    public void AddPuntaje(Puntaje nuevoPuntaje)
    {
        // Encuentra la posición donde insertar el nuevo puntaje
        int insertIndex = puntajes.FindIndex(p => p.Valor < nuevoPuntaje.Valor);

        // Si el nuevo puntaje es más alto que alguno de la lista o la lista no está llena, lo insertamos
        if (insertIndex != -1 || puntajes.Count < maxPuntajes)
        {
            if (insertIndex == -1)
            {
                // Si no se encontró un índice, insertar al final
                insertIndex = puntajes.Count;
            }

            puntajes.Insert(insertIndex, nuevoPuntaje);

            // Si la lista excede el máximo permitido, remover el puntaje más bajo
            if (puntajes.Count > maxPuntajes)
            {
                puntajes.RemoveAt(puntajes.Count - 1);
            }
        }
    }

    public int GetPosicion(Puntaje puntaje)
    {
        if(!puntajes.Contains(puntaje)) return -1;
        Puntajes.Sort((p1, p2) => p2.Valor.CompareTo(p1.Valor)); // Ordenar de mayor a menor

        // Encontrar la posición del nuevo puntaje
        int posicion = Puntajes.IndexOf(puntaje) + 1; // Sumar 1 para que la posición empiece en 1

        return posicion;
    }

}

[Serializable]
public struct Puntaje
{
    public string UserName;
    public int Valor;
}