using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private GameObject _winPanel;

    public void UpdateProgress(float percent)
    {
        _progressText.text = percent.ToString("F0") + "%";
    }

    public void ShowWin()
    {
        _winPanel.SetActive(true);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}