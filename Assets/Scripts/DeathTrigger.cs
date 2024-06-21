using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        GameManager.instance.PlayerDeath();
    }
}
