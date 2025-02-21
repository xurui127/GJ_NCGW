using System;

[Serializable]
public class ShipStatus
{
    private float energy;
    private int population;
    private float malfunctions;

    public event Action OnEnergyChanged; 
    public event Action OnPopulationChanged;
    public event Action OnMalfunctionsChanged;
    public float Energy
    {
        get => energy;
        set
        {
            if (Math.Abs(energy - value) > 0.01f)
            {
                energy = Math.Max(0, value);
                OnEnergyChanged?.Invoke();
            }
        }
    }

    public int Population
    {
        get => population;
        set
        {
            if (population != value)
            {
                population = Math.Max(0, value);
                OnPopulationChanged?.Invoke();
            }
        }
    }

    public float Malfunctions
    {
        get => malfunctions;
        set
        {
            if (Math.Abs(malfunctions - value) > 0.01f)
            {
                malfunctions = Math.Max(0, value);
                OnMalfunctionsChanged?.Invoke();
            }
        }
    }

    public ShipStatus(int population, float energy, float malfunc)
    {
        this.energy = energy;
        this.population = population;
        this.malfunctions = malfunc;
    }

    
}
