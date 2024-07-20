using TMPro;
using UnityEngine;

public class PuntajeTarjeta : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userNameTMP;
    [SerializeField] private TextMeshProUGUI scoreTMP;

    public void configurarTarjeta(string userName, int score)
    {
        if (name == null) return;
        userNameTMP.text = userName;
        scoreTMP.text = score.ToString();
    }
}
