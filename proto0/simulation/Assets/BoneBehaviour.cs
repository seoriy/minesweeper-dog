using System;
using Unity.VisualScripting;
using UnityEngine;

public class BoneBehaviour : MonoBehaviour
{
    private readonly float Thickness = 0.05f;
        
    public EndBehaviour endUp;
    public GameObject bone;
    public EndBehaviour endDown;
        
    public float length;

    private void Awake()
    {
        bone = Create("Bar");
        var endUpObject = Create("EndUp");
        endUp = endUpObject.AddComponent<EndBehaviour>();
            
        var endDownObject = Create("EndDown");
        endDown = endDownObject.AddComponent<EndBehaviour>();
    }

    GameObject Create(string tagSuffix)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        var o = gameObject;
        obj.name = $"{o.name}{tagSuffix}";
        obj.transform.SetParent(o.transform);
            
        // bone.AddComponent<Rigidbody>();
        // bone.GetComponent<Rigidbody>().useGravity = false;
        // obj.AddComponent<MeshFilter>();
        obj.AddComponent<CapsuleCollider>();
        // obj.AddComponent<MeshRenderer>();
            
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