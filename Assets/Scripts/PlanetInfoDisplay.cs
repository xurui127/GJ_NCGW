
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetInfoDisplay : MonoBehaviour
{
    private PlanetStatus planetData;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Canvas canvas;

    [SerializeField] private TMP_Text planetName;
    [SerializeField] private TMP_Text water;
    [SerializeField] private TMP_Text energy;
    [SerializeField] private TMP_Text habi;

    [SerializeField] private GameObject displayPanel;
    [SerializeField] private GameObject progressPanel;
    private bool isScanned = false;


    [SerializeField] private Slider progressBar;
    [SerializeField] private float fillSpeed = 0.03f;
    [SerializeField] private float maxFillSpeed = 0.1f;
    [SerializeField] private float delayMin = 1f;
    [SerializeField] private float delayMax = 2f;
    private float targetValue = 1f;

    private void Update()
    {
        DetectPlanetHover();
    }

    public void SetPlanetData(PlanetStatus data)
    {
        planetData = data;
    }

    public void DisplayUI()
    {
        if (planetData == null) return;
        planetName.text = $"Name: {planetData.planetName}";
        water.text = $"Water: {(planetData.waterLevel * 100).ToString("F1")}%";
        energy.text = $"Energy: {(planetData.energyLevel * 100).ToString("F1")}%";
        habi.text = $"Habitability: {(planetData.habitability * 100).ToString("F1")}%";
        Debug.Log($"Temp: {planetData.realTemp}");
        Debug.Log($"Real Habi: {planetData.realHabitability}");
        Debug.Log($"hasHazard: {planetData.hasHazard}");
        

    }


    private void DetectPlanetHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && !GameManager.Instance.isWarping)
        {
            if (isScanned) return;

            if (!isScanned)
            {
                displayPanel.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (isScanned) return;
                isScanned = true;
                displayPanel.SetActive(false);

                StartCoroutine(FillProgressBar());
            }
        }

    }

    private IEnumerator FillProgressBar()
    {
        displayPanel.SetActive(false);
        progressPanel.SetActive(true);

        while (progressBar.value < targetValue)
        {
            float step = Random.Range(fillSpeed, maxFillSpeed);

            progressBar.value = Mathf.Min(progressBar.value + step, targetValue);


            yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
        }

        progressPanel.SetActive(false);
        displayPanel.SetActive(true);
        DisplayUI();
    }
}
