using System;
using UnityEngine;

[Serializable]
public class PlanetStatus
{
    public string planetName;
    public PlanetType planetType;
    public float temp;
    public float realTemp;
    public float waterLevel;
    public float energyLevel;
    public float habitability;
    public float realHabitability;
    public bool hasHazard;

    public PlanetStatus(string planetName, PlanetType planetType, float temp, float realTemp, float waterLevel, float energyLevel, float habitability, float realHabitability, bool hasHazard)
    {
        this.planetName = planetName;
        this.planetType = planetType;
        this.temp = temp;
        this.realTemp = realTemp;
        this.waterLevel = waterLevel;
        this.energyLevel = energyLevel;
        this.habitability = habitability;
        this.realHabitability = realHabitability;
        this.hasHazard = hasHazard;
    }

    public static PlanetStatus GenerateRandomPlanet()
    {
        string[] planetNames = { "Icarus", "Eden", "Xylon-3", "Omega-9", "Titan-V", "Nebula-X" };
        string name = planetNames[UnityEngine.Random.Range(0, planetNames.Length)];

        PlanetType type = (PlanetType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(PlanetType)).Length);

        float fakeTemp = UnityEngine.Random.Range(-50f, 50f);
        float realTemp = fakeTemp + UnityEngine.Random.Range(-200f, 300f);

        float water = UnityEngine.Random.Range(0f, 1f);
        float energy = UnityEngine.Random.Range(0f, 1f);

        float realHabitability = Mathf.Clamp01((water * 0.4f) + (energy * 0.3f) - (UnityEngine.Random.value < 0.5f ? 0.5f : 0f));
        float fakeHabitability = Mathf.Clamp01(realHabitability + UnityEngine.Random.Range(-0.2f, 1f));
        bool hazards = UnityEngine.Random.value < 0.5f;

        return new PlanetStatus(name, type, fakeTemp, realTemp, water, energy, fakeHabitability, realHabitability, hazards);

    }

    public PlanetType GetPlanetTypy()
    {
        return planetType;
    }
}
