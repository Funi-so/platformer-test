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
            if (!mainEnabled) 
            {
                mainEnabled = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                mainEnabled = false;
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
}
