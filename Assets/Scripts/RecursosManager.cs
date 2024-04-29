using UnityEngine;

public class RecursosManager : Singleton<RecursosManager>
{
    [SerializeField] private int oro;
    [SerializeField] private int madera;
    [SerializeField] private int piedra;
    [SerializeField] private int metal;
    [SerializeField] private int habitantesActual;
    [SerializeField] private int habitantesDispo;

    public int Oro => oro;
    public int Madera => madera;
    public int Piedra => piedra;
    public int Metal => metal;
    public int HabitantesActual => habitantesActual;
    public int HabitantesDispo => habitantesDispo;

    private void Start() 
    {
        ActualizarUI();
    }

    public bool ComprobarRecursosSuficientesEdificio(EdificioBlueprint edificio)
    {
        if (edificio == null) return false;

        if (edificio.CostoOro > oro || edificio.CostoMadera > madera || edificio.CostoPiedra > piedra || edificio.CostoMetal > metal)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool ComprobarRecursosSuficientesAccion(AccionDeEdificio accion)
    {
        if (accion == null) return false;

        if (accion.CostoOro > oro || accion.CostoMadera > madera || accion.CostoPiedra > piedra || accion.CostoMetal > metal)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ColocarEdificio(EdificioBlueprint edificio)
    {
        oro -= edificio.CostoOro;
        madera -= edificio.CostoMadera;
        piedra -= edificio.CostoPiedra;
        metal -= edificio.CostoMetal;
        ActualizarUI();
    }

    public void ConsumirAccion(AccionDeEdificio accion)
    {
        oro -= accion.CostoOro;
        madera -= accion.CostoMadera;
        piedra -= accion.CostoPiedra;
        metal -= accion.CostoMetal;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        UIManager.Instance.ActualizarRecursosDisponibles(oro, madera, piedra, metal);
    }

}
