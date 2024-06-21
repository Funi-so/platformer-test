using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenericButton : MonoBehaviour
{
    public string SceneName;
    public void GoToScene()
    {
        SceneManager.LoadScene(SceneName);
    } 

    public void Quit()
    {
        Application.Quit();
    }
}
