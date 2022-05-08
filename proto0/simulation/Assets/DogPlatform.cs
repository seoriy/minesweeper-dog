using System;
using System.ComponentModel;
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
    
    private ArticulationBody _driver;
    
    private LegBehaviour _legLeft;
    private LegBehaviour _legRight;
    
    public float targetVelocity;
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
        _rotorBar.endDown.ShiftTo(_rotationCenter);

        _barLongLeft.Rotate(-90f);
        _barLongLeft.endUp.ShiftTo(_barShort.endDown);

        _barLongRight.Rotate(90f);
        _barLongRight.endUp.ShiftTo(_barShort.endDown);
    }

    private void MakeLinkage()
    {
        _barLongLeft.endUp.FixedConnectToBody(_barShort.endDown);
        _barLongRight.endUp.FixedConnectToBody(_barShort.endDown);
        
        _driver = _rotorBar.endDown.HingeConnectToBody(_rotationCenter);
        _rotationCenter.GetOrAddArticulationBody().immovable = true;
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
        // _legRight.transform.Rotate(Vector3.up, 180);
        // _legRight.transform.Rotate(Vector3.right, 180);
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
    }

    private void DecreaseSpeed()
    {
        targetVelocity -= velocityStep;
        _driver.xDrive = DriverXDrive();
    }

    private void IncreaseSpeed()
    {
        targetVelocity += velocityStep;
        _driver.xDrive = DriverXDrive();
    }

    private ArticulationDrive DriverXDrive()
    {
        Debug.Log($"VELOCITY = {targetVelocity}");
        return new ArticulationDrive()
        {
            damping = 1000000000000,
            stiffness = 0,
            targetVelocity = targetVelocity,
            forceLimit = 1000000000000000
        };
    }
}