using System;

[Serializable]
public class ShipStatus
{
    public float energy;
    public int population;
    public float malfunctions;

    public ShipStatus(int population, float energy, float malfunc)
    {
        this.energy = energy;
        this.population = population;
        this.malfunctions = malfunc;
    }
}
