using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GenericButton : MonoBehaviour
{
    [Header("Scene Transition")]
    public string SceneName;
    
    [Header("Fade In / Fade Out")]
    public GameObject rendsParent;
    private Graphic[] rends;
    public float waitTime = 0.5f;
    public float fadeTime = 0.5f;
    public float disableTime = 1.0f;

    [Header("Size Change")]    
    RectTransform rect;
    public Text text;

    void Start(){
        rect = GetComponent<RectTransform>();
        if(rendsParent != null){
            rends = rendsParent.GetComponentsInChildren<Graphic>(true);
        }
    }
    public void GoToScene(float delayTime)
    {
        Invoke("LoadScene", delayTime);
    } 

    private void LoadScene(){
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

    public void EnableAndFadeIn(){
        StartCoroutine(SetObjectActive(rendsParent, true, waitTime));
        StartCoroutine(FadeAll(0.0f, 1.0f, waitTime));
    }

    public void FadeOutAndDisable(){
        StartCoroutine(FadeAll(1.0f, 0.0f, waitTime));
        StartCoroutine(SetObjectActive(rendsParent, false, disableTime));
    }

    IEnumerator FadeAll(float startAlpha, float targetAlpha, float delayTime){
        yield return new WaitForSeconds(delayTime);
        for(int c = 0; c < rends.Length; c++){
                rends[c].GetComponent<CanvasRenderer>().SetAlpha(startAlpha);
                rends[c].CrossFadeAlpha(targetAlpha, fadeTime, true);
            }
    }

    IEnumerator SetObjectActive(GameObject obj, bool act, float delayTime){
        yield return new WaitForSeconds(delayTime);
        obj.SetActive(act);
    }
}
