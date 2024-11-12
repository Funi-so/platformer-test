using UnityEngine;
using UnityEngine.UI;

public class MainHUD : MonoBehaviour
{
    public GameObject pauseMenu;
    public KeyCode menuKey;
    bool mainEnabled = false;
    
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(menuKey))
        {
            if (!DialogController.controller.isTalking)
            {
                if (!mainEnabled) 
                {
                    mainEnabled = true;
                    pauseMenu.SetActive(true);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    Time.timeScale = 0f;
                }
                else
                {
                    mainEnabled = false;
                    pauseMenu.SetActive(false);
                    Time.timeScale = 1f;
                }
            } else { DialogController.controller.FinishDialog(); }
        }
    }
    public void AddIndexCount(int value)
    {
        DialogController.controller.dialogIndex += value;
        DialogController.controller.Dialog();
    }
}
