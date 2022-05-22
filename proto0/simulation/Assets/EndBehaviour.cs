using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EndBehaviour: MonoBehaviour
{
    public Joint hingeJoint1;
    public Joint hingeJoint2;
    public Joint hingeJoint3;
    
    public BoneBehaviour Bone => gameObject.GetComponentInParent<BoneBehaviour>();
    public Transform BoneTransform => gameObject.transform.parent;
    public int Direction => this == Bone.endDown ? 1 : -1;
    public Vector3 AnchorPosition
    {
        get
        {
            var position = GetObjectWithRigidBody().transform.InverseTransformPoint(gameObject.transform.position);
            return position;
        }
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
    

    public ConfigurableJoint HingeWith(EndBehaviour otherEnd)
    {
        var obj = GetObjectWithRigidBody();
        var otherObj = otherEnd.GetObjectWithRigidBody();
        
        var joint = obj.AddComponent<ConfigurableJoint>();
        joint.name = $"{name}Joint";
        
        joint.linearLimitSpring = new SoftJointLimitSpring { spring = 0 };
        
        joint.projectionMode = JointProjectionMode.PositionAndRotation;
        joint.projectionAngle = 180;
        joint.projectionDistance = 10;
        
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        // Free rotation around Z axis
        
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;
        
        joint.anchor = AnchorPosition;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = otherEnd.AnchorPosition;
        joint.connectedBody = otherObj.GetComponent<Rigidbody>();
        
        joint.enablePreprocessing = false;
        joint.enableCollision = false;
        
        if (!hingeJoint1)
            hingeJoint1 = joint;
        else if (!hingeJoint2)
            hingeJoint2 = joint;
        else if (!hingeJoint3)
            hingeJoint3 = joint;
        else throw new NotImplementedException();
        
        return joint;
    }

    public GameObject GetObjectWithRigidBody()
    {
        var obj = gameObject;
        
        while (obj != null && obj.GetComponent<Rigidbody>() == null)
        {
            obj = obj.transform.parent.GameObject();
        }  

        return obj;
    }
}