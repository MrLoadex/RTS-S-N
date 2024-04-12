using UnityEngine;

public class BuilderManager : Singleton<BuilderManager>
{

    [SerializeField] private EdificioBlueprint[] edificiosList;
    //public EdificioBlueprint[] EdificiosList => edificiosList;
    
    // Actualizar Los Edificios Disponibles
    private void Start() 
    {
        UIManager.Instance.InicializarPanelConstruccion(edificiosList);
    }

    public void ColocarObjeto(EdificioBlueprint edificio)
    {
        if (edificio == null) return;
        bool recursosSuficientes = RecursosManager.Instance.ComprobarConstruccion(edificio);
        if (!recursosSuficientes){return; }

        //Instantiate(edificio.EdificioColocadoPrefab,)

    }
    
}
