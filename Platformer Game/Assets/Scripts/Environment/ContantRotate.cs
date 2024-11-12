using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
    public float Xspeed;
    public float Yspeed;
    public float Zspeed;
    void FixedUpdate()
    {
        transform.Rotate(Xspeed * Time.deltaTime, Yspeed * Time.deltaTime, Zspeed * Time.deltaTime);
    }
}
