using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrainingUIManager : MonoBehaviour
{
    [SerializeField] GameObject statsDisplay;
    [SerializeField] Button trainingButton;
    [SerializeField] GameObject trainingOptions;
    [SerializeField] CanvasGroup dayCounter;
    [SerializeField] TextMeshProUGUI counter;
    [SerializeField] Slider progressSlider;

    [Header("STATS UI")]
    [SerializeField] Slider[] sliderUI;

    PlayerStatsManager psm;

    public int daysRemaining = 10;

    void Start()
    {
        psm = PlayerStatsManager.Instance;
        // ViewUIElements(statsDisplay.GetComponent<CanvasGroup>(), false);
        // ViewUIElements(trainingButton.GetComponent<CanvasGroup>(), false);
        ViewUIElements(trainingOptions.GetComponent<CanvasGroup>(), false);
        ViewUIElements(dayCounter, false);
        UpdateStatsUI(psm.GetAllStats());
    }

    public void OnTrainingButtonClicked()
    {
        if(daysRemaining <= 0)
        {
            SkipToNextDay();
        }

        ViewUIElements(trainingOptions.GetComponent<CanvasGroup>(), true);
    }

    public void OnBackButtonClicked(CanvasGroup currentCanvas)
    {
        ViewUIElements(currentCanvas, false);
    }

    public void OnStatIncreaseButtonClicked(int index)
    {
        ViewUIElements(trainingOptions.GetComponent<CanvasGroup>(), false);
        StartCoroutine(SkipDay(index));
    }

    public void ViewUIElements(CanvasGroup canvasgroup, bool isEnable)
    {
        canvasgroup.alpha = isEnable ? 1 : 0;
        canvasgroup.interactable = isEnable;
        canvasgroup.blocksRaycasts = isEnable;
    }

    public void UpdateStatsUI(int[] values)
    {
        StartCoroutine(UpdateStatsUICoroutine(values, 0.5f));
    }

    private IEnumerator UpdateStatsUICoroutine(int[] values, float duration)
    {
        float elapsed = 0f;

        float[] startValues = new float[sliderUI.Length];

        for (int i = 0; i < sliderUI.Length; i++)
        {
            startValues[i] = sliderUI[i].value;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            for (int i = 0; i < sliderUI.Length; i++)
            {
                sliderUI[i].value = Mathf.Lerp(startValues[i], values[i], t);
            }

            yield return null;
        }

        // Ensure the sliders end at the exact target values.
        for (int i = 0; i < sliderUI.Length; i++)
        {
            sliderUI[i].value = values[i];
        }
    }


    IEnumerator IncreaseWithDelay(int index, float previous, float value, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            //source.volume = Mathf.Lerp(0.5f, 1f, elapsed / duration);
            sliderUI[index].value = Mathf.Lerp(previous, value, elapsed / duration);
            yield return null;
        }

        sliderUI[index].value = value;
    }


    /*IEnumerator SkipDay()
    {
        progressSlider.value = 0;
        daysRemaining = Mathf.Clamp(daysRemaining - 1, 0, 10);
        counter.text = daysRemaining.ToString();
        yield return new WaitForSeconds(1);

        dayCounter.alpha = 1;
        dayCounter.interactable = true;
        dayCounter.blocksRaycasts = true;

        yield return new WaitForSeconds(3);
        progressSlider.value = 1;

        dayCounter.alpha = 0;
        dayCounter.interactable = false;
        dayCounter.blocksRaycasts = false;
    }*/

    void SkipToNextDay()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }

    IEnumerator SkipDay(int index)
    {
        progressSlider.value = 0;

        daysRemaining = Mathf.Clamp(daysRemaining - 1, 0, 10);
        counter.text = daysRemaining.ToString();

        dayCounter.interactable = true;
        dayCounter.blocksRaycasts = true;

         

        yield return new WaitForSeconds(1f);

        dayCounter.alpha = 1;


        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            progressSlider.value = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        progressSlider.value = 1f;

        dayCounter.alpha = 0;
        dayCounter.interactable = false;
        dayCounter.blocksRaycasts = false;

        switch (index)
        {
            case 0: psm.IncreaseStrength(); break;
            case 1: psm.IncreaseEndurance(); break;
            case 2: psm.IncreaseSpeed(); break;
            case 3: psm.IncreaseAccuracy(); break;
            case 4: psm.IncreaseAggression(); break;
            case 5: psm.IncreaseFocus(); break;
            case 6: psm.IncreaseRecovery(); break;
        }

        if(daysRemaining < 1)
        {
            trainingButton.GetComponentInChildren<TextMeshProUGUI>().text = "FIGHT!!!";
        }

        UpdateStatsUI(psm.GetAllStats());
    }

}
