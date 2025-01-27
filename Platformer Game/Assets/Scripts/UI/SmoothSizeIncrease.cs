using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SmoothSizeIncrease : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform visualRect;
    private Vector3 initialScale;
    public float hoverScaleFactor = 1.7f;
    public float scaleSpeed = 10f;

    private bool isHovering = false;

    void Awake()
    {
        visualRect = transform.GetComponent<RectTransform>();
        initialScale = visualRect.localScale;
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
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
    void OnDisable(){
        isHovering = false;
    }
}
