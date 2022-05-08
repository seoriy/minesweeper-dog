using UnityEngine;
using Component = System.ComponentModel.Component;

public class BoneBehaviour : MonoBehaviour
{
    public const float BoneSizeScale = 0.01f;
    private const float Thickness = 0.05f;

    public EndBehaviour endUp;
    public GameObject bone;
    public EndBehaviour endDown;
        
    public float length;

    private void Awake()
    {
        bone = Create("Bar");
        
        var endUpObject = Create("-UP");
        endUp = endUpObject.AddComponent<EndBehaviour>();
        
        var endDownObject = Create("-DOWN");
        endDown = endDownObject.AddComponent<EndBehaviour>();
    }

    public void MakeSolidBone()
    {
        gameObject.AddComponent<ArticulationBody>();
    }

    public void MakeFixedBoneFromDownToUp() => MakeFixedBone(endDown, endUp);
    
    public void MakeFixedBoneFromUpToDown() => MakeFixedBone(endUp, endDown);

    private void MakeFixedBone(EndBehaviour parentEnd, EndBehaviour childEnd)
    {
        bone.gameObject.FixedConnectToBody(parentEnd.gameObject);
        childEnd.gameObject.FixedConnectToBody(bone.gameObject);
    }

    private GameObject Create(string tagSuffix)
    {
        var o = gameObject;
        
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        obj.name = $"{o.name}{tagSuffix}";
        obj.transform.SetParent(o.transform);
        
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
    

}