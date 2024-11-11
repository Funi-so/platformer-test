using UnityEngine;

public class FollowDamped : MonoBehaviour
{
    public Transform target;

    public float speed;
    public float maxSpeed;
    public float damping;
    private Vector3 velocity = new Vector3();

    void FixedUpdate()
    {
        velocity = target.position - transform.position;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        var n1 = velocity * damping * damping * Time.deltaTime;
        //var n2 = 1 + damping * Time.deltaTime;
        velocity = n1;

        transform.Translate(velocity * speed * Time.deltaTime);
    }
}
