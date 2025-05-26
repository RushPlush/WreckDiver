using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    private GameObject mainMenuPanel;
    private GameObject creditsPanel;
    private bool isCreditsPanelActive = false;
    private EventSystem eventSystem;
    [SerializeField] private GameObject mainMenuFocusButton;
    [SerializeField] private GameObject creditsFocusButton;
    private MenuInstructions menuInstructions;

    private void Awake()
    {
        eventSystem = FindFirstObjectByType<EventSystem>();
    }

    private void Start()
    {
        mainMenuPanel = transform.GetChild(0).gameObject;
        creditsPanel  = transform.GetChild(1).gameObject;
        menuInstructions = FindFirstObjectByType<MenuInstructions>();
        if (menuInstructions.StartOnCreditScreen)
        {
            ToggleCredits();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        if (isCreditsPanelActive)
        {
            eventSystem.SetSelectedGameObject(creditsFocusButton);
        }
        else
        {
            eventSystem.SetSelectedGameObject(mainMenuFocusButton);
        }
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
