using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class DogPlatform : MonoBehaviour
{
    private BoneBehaviour _barLongLeft;
    private BoneBehaviour _barLongRight;
    private BoneBehaviour _barShort;
    private BoneBehaviour _rotorBar;
    
    private EndBehaviour _rotationCenter;
    private EndBehaviour _legMountPointLeft;
    private EndBehaviour _legMountPointRight;
    private EndBehaviour _driverPoint;

    private HingeJoint _driver;
    
    private LegBehaviour _legLeft;
    private LegBehaviour _legRight;

    public float angle = 0;
    
    public float targetVelocity = 0;
    public float velocityStep = 50;

    public void Awake()
    {
        InitBones();
        PlaceBones();
        MakeLinkage();

        AddLegs();
        
        IncreaseSpeed();
    }
    
    private void InitBones()
    {
        _barShort = this.CreateBone("boneShort", 7.8f);
        _barLongLeft = this.CreateBone("boneLongLeft", 38f);
        _barLongRight = this.CreateBone("boneLongRight", 38f);
        _rotorBar = this.CreateBone("boneBar", 15);

        _rotationCenter = _barShort.endUp;
        _rotationCenter.gameObject.name = "rotationCenter";

        _legMountPointLeft = _barLongLeft.endDown;
        _legMountPointLeft.gameObject.name = "legMountPointLeft";

        _legMountPointRight = _barLongRight.endDown;
        _legMountPointRight.gameObject.name = "legMountPointRight";

        _driverPoint = _rotorBar.endUp;
        _driverPoint.gameObject.name = "driverPoint";
    }

    private void PlaceBones()
    {
        _rotationCenter.ShiftTo(gameObject.transform.position);
        
        _rotorBar.Rotate(angle);
        
        _rotorBar.endDown.ShiftTo(_rotationCenter);

        _barLongLeft.Rotate(-90f);
        _barLongLeft.endUp.ShiftTo(_barShort.endDown);

        _barLongRight.Rotate(90f);
        _barLongRight.endUp.ShiftTo(_barShort.endDown);
    }

    private void MakeLinkage()
    {
        GameObject o = gameObject;

        if (o.GetComponent<Rigidbody>() == null)
        {
            o.AddComponent<Rigidbody>().isKinematic = true;
        }
        
        Extensions.SetParent(_barLongLeft, o);
        Extensions.SetParent(_barLongRight, o);
        Extensions.SetParent(_barShort, o);

        _rotorBar.gameObject.AddComponent<Rigidbody>();
        _rotorBar.endDown.HingeWith(_rotationCenter);

        InitDriver();
    }

    private void InitDriver()
    {
        var obj = _rotorBar.endDown.GetObjectWithRigidBody();
        var otherObj = _rotationCenter.GetObjectWithRigidBody();
        
        var joint = obj.AddComponent<HingeJoint>();
        joint.name = "DRIVER";
        
        joint.axis = _rotationCenter.gameObject.transform.up;
        
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = _rotorBar.endDown.AnchorPosition;
        joint.connectedAnchor = _rotationCenter.AnchorPosition;
        joint.connectedBody = otherObj.GetComponent<Rigidbody>();
        
        joint.enablePreprocessing = false;
        joint.enableCollision = false;
        
        _driver = joint;
    }

    private void AddLegs()
    {
        var leg = new GameObject();
        
        leg.transform.SetParent(gameObject.transform);
        _legLeft = leg.AddComponent<LegBehaviour>();
        _legLeft.Setup(_legMountPointLeft, _driverPoint);
        
        leg = new GameObject();
        leg.transform.SetParent(gameObject.transform);
        
        _legRight = leg.AddComponent<LegBehaviour>();
        _legRight.Setup(_legMountPointRight, _driverPoint, false);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            IncreaseSpeed();
        }
        
        else if (Input.GetKeyDown(KeyCode.A))
        {
            DecreaseSpeed();
        }
        
        else if (Input.GetKeyDown(KeyCode.W))
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void DecreaseSpeed()
    {
        targetVelocity -= velocityStep;
        SetMotor();
    }

    private void SetMotor()
    {
        _driver.useMotor = true;
        _driver.motor = new JointMotor
        {
            force = 100000,
            freeSpin = true,
            targetVelocity = targetVelocity
        };
    }

    private void IncreaseSpeed()
    {
        targetVelocity += velocityStep;
        SetMotor();
    }
}