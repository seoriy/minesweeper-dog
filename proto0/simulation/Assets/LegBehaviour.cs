using System;
using Unity.VisualScripting;
using UnityEngine;

public class LegBehaviour : MonoBehaviour
{
    public float boneSizeScale = 0.01f;
   
    //Jansen linkage scheme https://en.wikipedia.org/wiki/Jansen%27s_linkage#/media/File:Jansen's_Linkage.svg
    private BoneBehaviour _barLong;
    private BoneBehaviour _barShort;
    private BoneBehaviour _rotorBar;
    
    private GameObject _blueTriangle;
    private BoneBehaviour _blueBar;
    private BoneBehaviour _blueLong;
    private BoneBehaviour _blueShort;

    private BoneBehaviour _greenLink;
    private BoneBehaviour _orangeLink;
    private BoneBehaviour _purpleTrianglesLink;
    private BoneBehaviour _greenTrianglesLink;

    private GameObject _redTriangle;
    private BoneBehaviour _redBar;
    private BoneBehaviour _redLong;
    private BoneBehaviour _redShort;

    private EndBehaviour _rotationCenter;
    private EndBehaviour _legMountPoint;
    private EndBehaviour _driverPoint;
    
    public HingeJoint rotationHinge;
    private ArticulationBody _driver;

    private void InitBones()
    {
        var fixedTriangle = new GameObject("fixedTriangle");
        fixedTriangle.transform.SetParent(gameObject.transform);

        _barShort = CreateBone("boneShort", 7.8f, fixedTriangle);
        _barLong = CreateBone("boneLong", 38f, fixedTriangle);
        _rotorBar = CreateBone("boneBar", 15, gameObject);

        _rotationCenter = _barShort.endUp;
        _rotationCenter.gameObject.name = "rotationCenter";
        
        _legMountPoint = _barLong.endDown;
        _legMountPoint.gameObject.name = "legMountPoint";
        
        _driverPoint = _rotorBar.endUp;
        _driverPoint.gameObject.name = "driverPoint";

        _greenLink = CreateBone("boneGreenLink", 50f, gameObject);
        _orangeLink = CreateBone("boneOrangeLink", 61.9f, gameObject);
        _purpleTrianglesLink = CreateBone("bonePurpleTrianglesLink", 39.4f, gameObject);
        _greenTrianglesLink = CreateBone("boneGreenTrianglesLink", 39.3f, gameObject);
        
        _blueTriangle = new GameObject("blueTriangle");
        _blueTriangle.transform.SetParent(gameObject.transform);
        _blueBar = CreateBone("boneBlue", 36.7f, _blueTriangle);
        _blueLong = CreateBone("boneBlueLong", 65.7f, _blueTriangle);
        _blueShort = CreateBone("boneBlueShort", 49f, _blueTriangle);
        
        _redTriangle = new GameObject("redTriangle");
        _redTriangle.transform.SetParent(gameObject.transform);
        
        _redBar = CreateBone("boneRed", 40.1f, _redTriangle);
        _redLong = CreateBone("boneRedLong", 55.8f, _redTriangle);
        _redShort = CreateBone("boneRedShort", 41.5f, _redTriangle);
    }

    private void Awake()
    {
        InitBones();
        
        // set motor
        _rotationCenter.ShiftTo(gameObject.transform.position);
        _rotorBar.endDown.ShiftTo(_rotationCenter);
        
        _barLong.Rotate(-90f);
        _barLong.endUp.ShiftTo(_barShort.endDown);
        // _barLong.endUp.FixedJointWith(_barShort.endDown);
        
        Extensions.ComposeTriangle(
            _driverPoint, _greenLink.endDown,
            _legMountPoint, _redShort.endDown);
        
        Extensions.ComposeTriangle(
            _redShort.endUp, _redLong.endDown,
            _redShort.endDown, _redBar.endDown);

        Extensions.ComposeTriangle(
            _driverPoint, _orangeLink.endUp,
            _legMountPoint, _greenTrianglesLink.endUp);
        
        Extensions.ComposeTriangle(
            _redBar.endUp, _purpleTrianglesLink.endDown,
            _greenTrianglesLink.endDown, _blueBar.endDown);
        
        Extensions.ComposeTriangle(
            _blueBar.endDown, _blueShort.endUp,
            _blueBar.endUp, _blueLong.endUp);
        
        // _barShort.bone.SetActive(false);
        // _barLong.bone.SetActive(false);
        //
        // _barLong.endUp.gameObject.SetActive(false);
        // _barShort.endDown.gameObject.SetActive(false);
        //
        // _rotationCenter.gameObject.SetActive(true);
        // _legMountPoint.gameObject.SetActive(true);

        SetupLinkages();
        
        // rotationHinge = _rotationCenter.HingeWith(_rotorBar.endDown);
        
        // AddFixedJoints();
        // AddHingeJoints();
        
        TurnOffKinematic();
    }

    private void SetupLinkages()
    {
        _barShort.MakeFixedBoneFromUpToDown();
        _barLong.endUp.FixedConnectToBody(_barShort.endDown);
        _barLong.MakeFixedBoneFromUpToDown();

        _rotationCenter.gameObject.GetComponent<ArticulationBody>().immovable = true;
        _driver =  _rotorBar.endDown.HingeConnectToBody(_rotationCenter);
        _rotorBar.MakeFixedBoneFromDownToUp();
        
        _greenLink.endDown.HingeConnectToBody(_driverPoint);
        _greenLink.MakeFixedBoneFromDownToUp();

        _orangeLink.endUp.HingeConnectToBody(_driverPoint);
        _orangeLink.MakeFixedBoneFromUpToDown();
        
        var redTriangleDrivePoint = _redLong.endDown;
        
        // red
        redTriangleDrivePoint.HingeConnectToBody(_greenLink.endUp);
        _redLong.MakeFixedBoneFromDownToUp();
        
        _redShort.endUp.FixedConnectToBody(redTriangleDrivePoint);
        _redShort.MakeFixedBoneFromUpToDown();

        _redBar.endDown.FixedConnectToBody(_redShort.endDown);
        _redBar.MakeFixedBoneFromDownToUp();
        
        // blue
        _blueBar.endDown.HingeConnectToBody(_orangeLink.endDown);
        _blueBar.MakeFixedBoneFromDownToUp();

        _blueShort.endUp.FixedConnectToBody(_blueBar.endDown);
        _blueShort.MakeFixedBoneFromUpToDown();

        _blueLong.endUp.FixedConnectToBody(_blueBar.endUp);
        _blueLong.MakeFixedBoneFromUpToDown();

        // triangle links
        // _legMountPoint.GetComponent<ArticulationBody>().immovable = true;
        _redShort.endDown.HingeWith(_legMountPoint);
        
        _greenTrianglesLink.endUp.HingeConnectToBody(_redBar.endDown);
        _greenTrianglesLink.MakeFixedBoneFromUpToDown();
        _greenTrianglesLink.endDown.HingeWith(_blueBar.endDown);
        
        _purpleTrianglesLink.endDown.HingeConnectToBody(_redBar.endUp);
        _purpleTrianglesLink.MakeFixedBoneFromDownToUp();
        _purpleTrianglesLink.endUp.HingeWith(_blueBar.endUp);
    }

    // private void ConnectBody(Component component, Component parent)
    // {
    //     component.gameObject.transform.SetParent(parent.gameObject.transform, true);
    // } 
    //
    // private void AddHingeJoints()
    // {
    //     // _rotationCenter.GetComponent<Rigidbody>().isKinematic = true;
    //     // _legMountPoint.GetComponent<Rigidbody>().isKinematic = true;
    //     
    //     // _rotationCenter.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    //     // _legMountPoint.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    //     
    //     rotationHinge = _rotorBar.endDown.HingeWith(_rotationCenter);
    //     
    //     _GreenLink.endDown.HingeWith(_driverPoint);
    //     _OrangeLink.endUp.HingeWith(_driverPoint);
    //     
    //     _OrangeLink.endUp.HingeWith(_BlueBar.endDown);
    //     _OrangeLink.endUp.HingeWith(_GreenTrianglesLink.endDown);
    //     
    //     _GreenTrianglesLink.endUp.HingeWith(_legMountPoint);
    //     _RedBar.endDown.HingeWith(_legMountPoint);
    //     
    //     _GreenLink.endUp.HingeWith(_RedShort.endUp);
    //     
    //     _PurpleTrianglesLink.endDown.HingeWith(_RedBar.endUp);
    //     _PurpleTrianglesLink.endUp.HingeWith(_BlueBar.endUp);
    // }
    //
    // private void AddFixedJoints()
    // {
    //     // bones 
    //     _barLong.JointWithOwnEnds();
    //     _barShort.JointWithOwnEnds();
    //     _rotorBar.JointWithOwnEnds();
    //     
    //     _BlueBar.JointWithOwnEnds();
    //     _BlueLong.JointWithOwnEnds();
    //     _BlueShort.JointWithOwnEnds();
    //     
    //     _GreenLink.JointWithOwnEnds();
    //     _OrangeLink.JointWithOwnEnds();
    //     
    //     _PurpleTrianglesLink.JointWithOwnEnds();
    //     _GreenTrianglesLink.JointWithOwnEnds();
    //     
    //     _RedBar.JointWithOwnEnds();
    //     _RedLong.JointWithOwnEnds();
    //     _RedShort.JointWithOwnEnds();
    //     
    //     // fixed triangle
    //     _barLong.endUp.FixedJointWith(_barShort.endDown);
    //     
    //     // red fixed triangle
    //     _RedShort.endUp.FixedJointWith(_RedLong.endDown);
    //     _RedShort.endDown.FixedJointWith(_RedBar.endDown);
    //     _RedLong.endDown.FixedJointWith(_RedBar.endDown);
    //     
    //     // blue fixed triangle
    //     _BlueBar.endDown.FixedJointWith(_BlueShort.endUp);
    //     _BlueBar.endUp.FixedJointWith(_BlueLong.endUp);
    //     _BlueShort.endUp.FixedJointWith(_BlueLong.endUp);
    // }

    private BoneBehaviour CreateBone(string boneTag, float size, GameObject parent)
    {
        var boneObject = new GameObject();
        boneObject.name = boneTag;
        boneObject.transform.SetParent(gameObject.transform);

        var bone = boneObject.AddComponent<BoneBehaviour>();
        bone.length = size * boneSizeScale;
        bone.OnValidate();
        
        return bone;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            // gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Debug.Log("Go!");

            _driver.xDrive = new ArticulationDrive()
            {
                damping = 100000,
                stiffness = 0,
                targetVelocity = 100,
                forceLimit = 10000000
            };
            
            // _driver.mot
            // var motor = rotationHinge.motor;
            // motor.force = 100;
            // motor.targetVelocity = 1f;
            // motor.freeSpin = true;
            // rotationHinge.motor = motor;
            // rotationHinge.useMotor = true;
            
            TurnOffKinematic();
        }
        
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Stop!");
            rotationHinge.useMotor = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _orangeLink.gameObject.SetActive(!_orangeLink.gameObject.activeSelf);
            _greenLink.gameObject.SetActive(!_greenLink.gameObject.activeSelf);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _redTriangle.SetActive(!_redTriangle.activeSelf);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _purpleTrianglesLink.gameObject.SetActive(!_purpleTrianglesLink.gameObject.activeSelf);
            _greenTrianglesLink.gameObject.SetActive(!_greenTrianglesLink.gameObject.activeSelf);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _blueTriangle.SetActive(!_blueTriangle.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _barShort.gameObject.SetActive(!_barShort.gameObject.activeSelf);
            _barLong.gameObject.SetActive(!_barLong.gameObject.activeSelf);
        }
    }

    private void TurnOffKinematic()
    {
        // _driverPoint.gameObject.GetComponent<ArticulationBody>().immovable = true;
        _barLong.TurnOffKinematic();
        _barShort.TurnOffKinematic();
        _rotorBar.TurnOffKinematic();
        _blueBar.TurnOffKinematic();
        _blueLong.TurnOffKinematic();
        _blueShort.TurnOffKinematic();
        _greenLink.TurnOffKinematic();
        _orangeLink.TurnOffKinematic();
        _purpleTrianglesLink.TurnOffKinematic();
        _greenTrianglesLink.TurnOffKinematic();
        _redBar.TurnOffKinematic();
        _redLong.TurnOffKinematic();
        _redShort.TurnOffKinematic();
    }
}
