
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetInfoDisplay : MonoBehaviour
{
    private PlanetStatus planetData;

    [SerializeField] private LayerMask layerMask;
    [SerializeField]private Canvas canvas;

    [SerializeField] private TMP_Text planetName;
    [SerializeField] private TMP_Text water;
    [SerializeField] private TMP_Text energy;
    [SerializeField] private TMP_Text habi;

    [SerializeField] private GameObject progressBar;

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
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canvas.gameObject.SetActive(true);
        DisplayUI();
        Debug.Log("Enter");
    }

    private void DetectPlanetHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray ,out hit, Mathf.Infinity, layerMask))
        {
            canvas.gameObject.SetActive(true);
            DisplayUI();
        }
    }

    private IEnumerator ShowProgress()
    {
        yield return null;
    }
}
