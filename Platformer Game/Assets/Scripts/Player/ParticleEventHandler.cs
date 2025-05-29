using UnityEngine;

public class ParticleEventHandler : MonoBehaviour
{
    public ParticleSystem[] systems;
    public void FootStepEvent(int side)
    {
        systems[side].Play();
    }
}
