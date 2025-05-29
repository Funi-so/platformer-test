using UnityEngine;

public class EnableOnStart : MonoBehaviour
{
    public GameObject obj;

    void Start()
    {
        obj.SetActive(true);   
    }
}
