using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text energy;
    public TMP_Text population;
    public TMP_Text malfunctions;

    private ShipStatus shipStatus;


    public void Start()
    {
        shipStatus = GameManager.Instance.shipStatus;
        shipStatus.OnEnergyChanged += UpdateEnergyUI;
        shipStatus.OnPopulationChanged += UpdatePopulationUI;
        shipStatus.OnMalfunctionsChanged += UpdateMalfunctionsUI;
        UpdataUI();
    }
    public void SetText(TMP_Text text, string prefix, string value)
    {
        text.text = $"{prefix}{value}";
    }

    private void UpdataUI()
    {
        SetText(energy, "Energy: ", shipStatus.Energy.ToString());
        SetText(population, "Population: ", shipStatus.Population.ToString());
        SetText(malfunctions, "Malfunction: ", shipStatus.Malfunctions.ToString());

    }
    private void UpdateEnergyUI()
    {
        energy.text = $"Energy: {shipStatus.Energy}";
    }

    private void UpdatePopulationUI()
    {
        population.text = $"Population: {shipStatus.Population}";
    }

    private void UpdateMalfunctionsUI()
    {
        malfunctions.text = $"Malfunction: {shipStatus.Malfunctions}%";
    }



}
