using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;

    PlayerStatsManager psm;

    void Start()
    {
        psm = PlayerStatsManager.Instance;

        if (psm == null)
            return;

        resultText.text = psm.result;
    }

    public void OnRematchButtonClicked()
    {
        if (psm != null)
        {
            psm.SetStrength(1);
            psm.SetEndurance(1);
            psm.SetAgility(1);
            psm.SetAggression(5);
            psm.SetAccuracy(5);
            psm.SetFocus(5);
            psm.SetRecovery(10);
        }

        SceneManager.LoadScene(0);
    }
}
