using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuHandler : MonoBehaviour
{
    private GameObject mainMenuPanel;
    private GameObject creditsPanel;
    private bool isCreditsPanelActive = false;

    private void Start()
    {
        mainMenuPanel = transform.GetChild(0).gameObject;
        creditsPanel  = transform.GetChild(1).gameObject;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ToggleCredits()
    {
        isCreditsPanelActive = !isCreditsPanelActive;
        mainMenuPanel.SetActive(!isCreditsPanelActive);
        creditsPanel.SetActive(isCreditsPanelActive);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
