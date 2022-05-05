using System;
using System.Collections.Generic;
using UnityEngine;

public class EndBehaviour: MonoBehaviour
{
    public FixedJoint fixedJoint;
    // public readonly Dictionary<string, HingeJoint> HingeJoints = new Dictionary<string, HingeJoint>();
    public HingeJoint hingeJoint1;
    public HingeJoint hingeJoint2;
    public HingeJoint hingeJoint3;

    public readonly Dictionary<string, FixedJoint> fixedJoints = new Dictionary<string, FixedJoint>();
    
    // public void ConnectJoints(Rigidbody boneRigidbody)
    // {
    //     // boneRigidbody.angularDrag = 0;
    //     
    //     fixedJoint = gameObject.AddComponent<FixedJoint>();
    //     fixedJoint.enableCollision = false;
    //     // gameObject.GetComponent<Rigidbody>().angularDrag = 0;
    //
    //     fixedJoint.connectedBody = boneRigidbody;
    // }

    // public FixedJoint FixedJointWith(EndBehaviour otherEnd)
    // {
    //     if (fixedJoints.ContainsKey(otherEnd.name))
    //         return fixedJoints[otherEnd.name];
    //     
    //     var joint = gameObject.AddComponent<FixedJoint>();
    //     
    //     joint.name = $"{name}Joint";
    //     joint.enableCollision = false;
    //     joint.connectedBody = otherEnd.gameObject.GetComponent<Rigidbody>();
    //     
    //     fixedJoints[otherEnd.name] = joint;
    //     return joint;
    // }

    public HingeJoint HingeWith(EndBehaviour otherEnd)
    {
        var otherObj = otherEnd.gameObject;
        
        var obj = new GameObject($"link({name}-{otherObj.name})");
        obj.transform.SetParent(otherObj.transform, false);
        
        obj.AddComponent<Rigidbody>();
        obj.AddComponent<FixedJoint>()
            .connectedArticulationBody = otherObj.GetComponent<ArticulationBody>();
        
        var joint = obj.AddComponent<HingeJoint>();
        joint.name = $"{name}Joint";
        joint.axis = new Vector3(0, 1, 0); // gameObject.transform.up;

        joint.anchor = new Vector3(0, 0, 0); //obj.transform.localPosition;  //gameObject.transform.localPosition;//
        joint.autoConfigureConnectedAnchor = true;
        // joint.connectedAnchor = new Vector3(0, 0, 0);
        
        // joint.connectedAnchor = gameObject.transform.localPosition
        joint.enableCollision = false;
        joint.connectedArticulationBody = gameObject.GetComponent<ArticulationBody>();

        if (!hingeJoint1)
            hingeJoint1 = joint;
        else if (!hingeJoint2)
            hingeJoint2 = joint;
        else if (!hingeJoint3)
            hingeJoint3 = joint;
        else throw new NotImplementedException();
        
        return joint;
    }

    public void ShiftTo(EndBehaviour targetEnd)
    {
        var t = gameObject.transform;
            
        var diff = targetEnd.gameObject.transform.position - t.position;
        t.parent.position += diff;
    }
    
    public void ShiftTo(Vector3 position)
    {
        var t = gameObject.transform;
            
        var diff = position - t.position;
        t.parent.position += diff;
    }

    public BoneBehaviour Bone
    {
        get
        {
            GameObject o;
            var bone = (o = gameObject).GetComponentInParent<BoneBehaviour>();
            
            // Debug.Log($"Get {name}.Bone = {bone}; [gameObject] = {o}");
            return bone;
        }
    }

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

    public EndBehaviour OppositeEnd => Bone.endDown == this ? Bone.endUp : this;
}