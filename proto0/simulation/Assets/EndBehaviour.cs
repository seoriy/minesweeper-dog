using UnityEngine;

public class EndBehaviour: MonoBehaviour
{
    public void ShiftTo(EndBehaviour targetEnd)
    {
        var t = gameObject.transform;
            
        var diff = targetEnd.gameObject.transform.position - t.position;
        t.parent.position += diff;
    }

    public BoneBehaviour Bone => gameObject.GetComponentInParent<BoneBehaviour>();

    public Transform BoneTransform => gameObject.transform.parent;

    public int Direction => this == Bone.endDown ? 1 : -1;

    public Quaternion BoneRotation
    {
        get =>
            this == Bone.endDown
                ? BoneTransform.rotation
                : Quaternion.Inverse(BoneTransform.transform.rotation);

        set => BoneTransform.rotation = 
            this == Bone.endDown
                ? value
                : Quaternion.Inverse(value);
    }
}