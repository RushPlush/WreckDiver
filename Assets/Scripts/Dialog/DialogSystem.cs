using System;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private float dialogMinDuration = 5f;
    [SerializeField] private float durationPerChar = 0.1f;
    [SerializeField] private string startUpDialog = "I lost connection to my ship, and it seems my oxygen is running out.";
    private float timer;
    private bool isDialogActive = false;
    private string currentDialog;

    private void Start()
    {
        dialogBox.SetActive(isDialogActive);
        dialogText.gameObject.SetActive(isDialogActive);
        SetDialog(startUpDialog);
    }
    
    public void SetDialog(string dialog)
    {
        currentDialog = dialog;
        dialogText.text = currentDialog;
        isDialogActive = true;
        dialogBox.SetActive(isDialogActive);
        dialogText.gameObject.SetActive(isDialogActive);
        
        timer = dialogMinDuration + (currentDialog.Length * durationPerChar);
    }

    private void FixedUpdate()
    {
        if(timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0)
            {
                isDialogActive = false;
                dialogBox.SetActive(isDialogActive);
                dialogText.gameObject.SetActive(isDialogActive);
            }
        }
    }
}
