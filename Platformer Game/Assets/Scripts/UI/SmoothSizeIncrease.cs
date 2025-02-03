using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SmoothSizeIncrease : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform visualRect;
    private Vector3 initialScale;
    public float hoverScaleFactor = 1.7f;
    public float scaleSpeed = 10f;

    public bool startScaled;

    private bool isHovering = false;
    private bool looking = true;

    void Awake()
    {
        visualRect = transform.GetComponent<RectTransform>();
        initialScale = visualRect.localScale;

        KeepBig(startScaled);
    }


    void Update()
    {
        if (isHovering)
        {
            visualRect.localScale = Vector3.Lerp(visualRect.localScale, initialScale * hoverScaleFactor, Time.deltaTime * scaleSpeed);
        }
        else
        {
            visualRect.localScale = Vector3.Lerp(visualRect.localScale, initialScale, Time.deltaTime * scaleSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(looking) isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(looking) isHovering = false;
    }
    void OnDisable(){
        isHovering = false;
    }

    public void KeepBig(bool size){
        looking = !size;
        isHovering = size;
    }
}
