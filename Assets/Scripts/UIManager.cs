using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text energy;
    public TMP_Text population;
    public TMP_Text malfunctions;

    private ShipStatus shipStatus;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 5f; 
    [SerializeField] GameObject endPanel;
    [SerializeField] public TMP_Text explainText;

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

    public void RestartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = fadeImage.color;

        fadeImage.raycastTarget = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        endPanel.gameObject.SetActive(true );
    }

    public void StartFading()
    {
        StartCoroutine(FadeIn());
    }
    public void SetEnergyText()
    {
        explainText.text = $"Your ship has run out of energy.\n" +
                           $"Drifting aimlessly, \n" +
                           $"you and your citizens fade into the endless void...";
    }
    public void SetPopuText()
    {
        explainText.text = $"There is no one left to save.\n" +
                           $"Your mission has ended in failure.";
    }

    public void SetMalfText()
    {
        explainText.text = $"Critical system failure!\n" +
                           $"With no way to repair the ship,\n" +
                           $"you and your citizens are doomed to float forever...";
    }

    public void SetHazardText()
    {
        explainText.text = $"The analysis suggested this planet was safe...\n" +
                           $"But it was wrong. Unknown creatures emerged from the shadows and attacked.\n" +
                           $"There were no survivors.";
    }

}
