using System;
using UnityEngine;

public class EndBehaviour: MonoBehaviour
{
    public GameObject hingeJoint1;
    public GameObject hingeJoint2;
    public GameObject hingeJoint3;
    
    
    public BoneBehaviour Bone => gameObject.GetComponentInParent<BoneBehaviour>();

    public Transform BoneTransform => gameObject.transform.parent;

    public int Direction => this == Bone.endDown ? 1 : -1;

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
    
    public Joint HingeWith(EndBehaviour otherEnd)
    {
        var otherObj = otherEnd.gameObject;
        
        var obj = new GameObject();
        // obj.name = $"HL-{name}-{otherObj.name}";
        obj.name = $"HL{name}";
        obj.transform.SetParent(otherObj.transform, true);
        obj.transform.localPosition = otherEnd.AnchorPosition;
        
        var rigid = obj.AddComponent<Rigidbody>();
        // rigid.mass = 0.0001f;
        rigid.maxDepenetrationVelocity = 0f;
        rigid.drag = 20;
        rigid.angularDrag = 20;
        
        //rigid.isKinematic = true;

        //obj.AddComponent<EndFolllower>().target = otherEnd;
        var fixedJoint1 = obj.AddComponent<ConfigurableJoint>();
        fixedJoint1.autoConfigureConnectedAnchor = false;
        fixedJoint1.anchor = Vector3.zero;
        fixedJoint1.connectedAnchor = otherEnd.AnchorPosition;
        
        fixedJoint1.projectionMode = JointProjectionMode.PositionAndRotation;
        fixedJoint1.projectionAngle = 180;
        fixedJoint1.projectionDistance = 10;
        fixedJoint1.angularXMotion = ConfigurableJointMotion.Locked;
        fixedJoint1.angularYMotion = ConfigurableJointMotion.Locked;
        fixedJoint1.angularZMotion = ConfigurableJointMotion.Locked;
        fixedJoint1.xMotion = ConfigurableJointMotion.Locked;
        fixedJoint1.yMotion = ConfigurableJointMotion.Locked;
        fixedJoint1.zMotion = ConfigurableJointMotion.Locked;
        fixedJoint1.rotationDriveMode = RotationDriveMode.XYAndZ;
        fixedJoint1.enablePreprocessing = false;
        
        fixedJoint1.linearLimitSpring = new SoftJointLimitSpring()
        {
            spring = 0
        };
        fixedJoint1.connectedArticulationBody = otherEnd.GetOrAddArticulationBody();
        //
        // fixedJoint1.massScale = 0.00001f;
        // fixedJoint1.connectedMassScale = 1000f;

        var joint = obj.AddComponent<ConfigurableJoint>();
        joint.projectionMode = JointProjectionMode.PositionAndRotation;
        joint.projectionAngle = 180;
        joint.projectionDistance = 10;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        // joint.angularZMotion = ConfigurableJointMotion.Locked;
        
        joint.linearLimitSpring = new SoftJointLimitSpring()
        {
            spring = 0
        };
        
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;
        
        // joint.massScale = 0.00001f;
        joint.connectedMassScale = 1000f;

        joint.name = $"{name}Joint";
        
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = AnchorPosition;
        joint.enablePreprocessing = false;
        
        joint.enableCollision = false;
        joint.connectedArticulationBody = GetOrAddArticulationBody();
        
        if (!hingeJoint1)
            otherEnd.hingeJoint1 = obj;
        else if (!hingeJoint2)
            otherEnd.hingeJoint2 = obj;
        else if (!hingeJoint3)
            otherEnd.hingeJoint3 = obj;
        else throw new NotImplementedException();
        
        return joint;
    }
    
    public ArticulationBody HingeConnectToBody(EndBehaviour parent)
    {
        var childBody = GetOrAddArticulationBody(); 
        var parentBody = parent.GetOrAddArticulationBody();
        
        childBody.gameObject.transform.SetParent(parentBody.gameObject.transform);
        
        childBody.jointType = ArticulationJointType.RevoluteJoint;
        SetAnchors(parent, childBody);
        
        childBody.anchorRotation = Quaternion.Euler(0, 90, 0);
        childBody.parentAnchorRotation = Quaternion.Euler(0, 90, 0);

        return childBody;
    }

    private Vector3 AnchorPosition => gameObject.GetComponent<ArticulationBody>() == null
        ? gameObject.transform.localPosition
        : Vector3.zero;

    public ArticulationBody FixedConnectToBody(EndBehaviour parent)
    {
        var childBody = GetOrAddArticulationBody(); 
        var parentBody = parent.GetOrAddArticulationBody();
        
        childBody.gameObject.transform.SetParent(parentBody.gameObject.transform);

        childBody.jointType = ArticulationJointType.FixedJoint;
        SetAnchors(parent, childBody);
        
        return childBody;
    }
    
    public ArticulationBody GetOrAddArticulationBody()
    {
        var body = gameObject.GetComponent<ArticulationBody>();
        if (body == null)
        {
            body = Bone.gameObject.GetComponent<ArticulationBody>();
        }
        
        if (body == null)
        {
            body = Bone.gameObject.AddComponent<ArticulationBody>();
        }

        return body;
    }

    private void SetAnchors(EndBehaviour parent, ArticulationBody childBody)
    {
        childBody.anchorPosition = AnchorPosition;
        childBody.parentAnchorPosition = parent.AnchorPosition;
    }
}