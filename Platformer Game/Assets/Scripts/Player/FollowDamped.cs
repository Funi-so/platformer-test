using UnityEngine;

public class FollowDamped : MonoBehaviour
{
    public Transform target;

    public float speed;
    public float maxSpeed;
    //public float minDistanceThreshold;
    public float damping;
    private Vector3 velocity = new Vector3();

    void FixedUpdate()
    {
        velocity = target.position - transform.position;
        //if (velocity.magnitude > minDistanceThreshold)
        { 
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            velocity = velocity * damping * damping * Time.deltaTime;
            transform.Translate(velocity * speed * Time.deltaTime);
        }
    }
}
