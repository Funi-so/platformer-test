
using UnityEngine;
using UnityEngine.UI;

public class ShowStartMenu : MonoBehaviour
{
    public Text text;
    public GameObject canvas;
    public Graphic[] rends;

    private bool pressed = false;
    void Start(){
        text.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        text.CrossFadeAlpha(1f, 2f, false);
    }
    void Update()
    {
        if(Input.anyKeyDown && !pressed)
        {
            text.CrossFadeAlpha(0f, 1f, false);

            Invoke("FadeAllIn", 1f);
            pressed = true;
        }
    }

    void FadeAllIn(){
        
            canvas.SetActive(true);
        for(int c = 0; c < rends.Length; c++){
                rends[c].GetComponent<CanvasRenderer>().SetAlpha(0.0f);
                rends[c].CrossFadeAlpha(1f, 2f, false);
            }
    }
}
