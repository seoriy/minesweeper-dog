using UnityEngine;

public class BoneBehaviour : MonoBehaviour
{
    private const float Thickness = 0.05f;

    public EndBehaviour endUp;
    public GameObject bone;
    public EndBehaviour endDown;
        
    public float length;
    // private HingeJoint joint;

    private void Awake()
    {
        // Debug.Log($"{name} Awake!");
        bone = Create("Bar");
        var endUpObject = Create("EndUp");
        endUp = endUpObject.AddComponent<EndBehaviour>();
        
        var endDownObject = Create("EndDown");
        endDown = endDownObject.AddComponent<EndBehaviour>();
        
        // JointWithOwnEnds();

        // bone.GetComponent<Rigidbody>().isKinematic = true;
        // endUp.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        // endDown.gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void MakeFixedBoneFromDownToUp() => MakeFixedBone(endDown, endUp);
    
    public void MakeFixedBoneFromUpToDown() => MakeFixedBone(endUp, endDown);

    private void MakeFixedBone(EndBehaviour parentEnd, EndBehaviour childEnd)
    {
        bone.gameObject.FixedConnectToBody(parentEnd.gameObject);
        childEnd.gameObject.FixedConnectToBody(bone.gameObject);
    }
    //
    // public void JointWithOwnEnds()
    // {
    //     // Debug.Log($"{name} Connect Joints!");
    //     var boneRigidbody = bone.GetComponent<Rigidbody>();
    //     
    //     endDown.ConnectJoints(boneRigidbody);
    //     endUp.ConnectJoints(boneRigidbody);
    // }

    private GameObject Create(string tagSuffix)
    {
        var o = gameObject;
        
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        obj.name = $"{o.name}{tagSuffix}";
        obj.transform.SetParent(o.transform);
        
        //obj.AddComponent<Rigidbody>();
        // obj.GetComponent<Rigidbody>().useGravity = false;
        // To prevent broken physics, we don't need to collide bones with joint cylinders and other bones
        obj.GetComponent<CapsuleCollider>().enabled = false;
        return obj;
    }

    public void OnValidate()
    {
        var boneDirection = gameObject.transform.right;
        var endRotation = Quaternion.AngleAxis(90f, boneDirection);
            
        var halfLength = length / 2;
        bone.transform.localScale = new Vector3(Thickness, halfLength, Thickness);

        var transform1 = endUp.gameObject.transform;
        transform1.localPosition = Vector3.up * halfLength;
        transform1.localScale = new Vector3(Thickness, Thickness/2, Thickness);
        transform1.rotation = endRotation;

        var transform2 = endDown.gameObject.transform;
        transform2.localPosition = Vector3.down * halfLength;
        transform2.localScale = new Vector3(Thickness, Thickness/2, Thickness);
        transform2.rotation = endRotation;
    }

    public void TurnOffKinematic()
    {
        // bone.GetComponent<Rigidbody>().isKinematic = false;
        // endUp.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        // endDown.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
}