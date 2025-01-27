using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GenericButton : MonoBehaviour
{
    public string SceneName;
    RectTransform rect;
    public Text text;
    void Start(){
        rect = GetComponent<RectTransform>();
    }
    public void GoToScene()
    {
        SceneManager.LoadScene(SceneName);
    } 

    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeSize(float changeAmt){
        rect.sizeDelta = rect.sizeDelta * changeAmt;
    }

    public void ChangeFontSize(int size){
        text.fontSize = size;
    }
}
