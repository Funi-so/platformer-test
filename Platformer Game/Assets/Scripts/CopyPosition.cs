using TreeEditor;
using UnityEngine;

public class CopyTransforms : MonoBehaviour
{
    public bool rotation;
    public Transform CopyFrom;
    public Vector3 PositionOffset;
    private Quaternion initialRotation;
    void start()
    {
        initialRotation=CopyFrom.rotation;
    }
    void Update()
    {
        transform.position = CopyFrom.position+PositionOffset;
        if(rotation)transform.rotation = CopyFrom.rotation * Quaternion.Inverse(initialRotation);
    }
}
