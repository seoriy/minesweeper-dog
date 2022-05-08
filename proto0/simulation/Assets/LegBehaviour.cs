using UnityEngine;

public class LegBehaviour : MonoBehaviour
{
    //Jansen linkage scheme https://en.wikipedia.org/wiki/Jansen%27s_linkage#/media/File:Jansen's_Linkage.svg
    
    private BoneBehaviour _blueBar;
    private BoneBehaviour _blueLong;
    private BoneBehaviour _blueShort;

    private BoneBehaviour _greenLink;
    private BoneBehaviour _orangeLink;
    private BoneBehaviour _purpleTrianglesLink;
    private BoneBehaviour _greenTrianglesLink;

    private BoneBehaviour _redBar;
    private BoneBehaviour _redLong;
    private BoneBehaviour _redShort;

    private EndBehaviour _rotationCenter;
    private EndBehaviour _legMountPoint;
    private EndBehaviour _driverPoint;
    
    private ArticulationBody _driver;

    private void InitBones()
    {
        _greenLink = this.CreateBone("boneGreenLink", 50f);
        _orangeLink = this.CreateBone("boneOrangeLink", 61.9f);
        _purpleTrianglesLink = this.CreateBone("bonePurpleTrianglesLink", 39.4f);
        _greenTrianglesLink = this.CreateBone("boneGreenTrianglesLink", 39.3f);
        _blueBar = this.CreateBone("boneBlue", 36.7f);
        _blueLong = this.CreateBone("boneBlueLong", 65.7f);
        _blueShort = this.CreateBone("boneBlueShort", 49f);
        _redBar = this.CreateBone("boneRed", 40.1f);
        _redLong = this.CreateBone("boneRedLong", 55.8f);
        _redShort = this.CreateBone("boneRedShort", 41.5f);
    }

    public void Setup(EndBehaviour legMountPoint, EndBehaviour driverPoint, bool leftLeg = true)
    {
        InitBones();

        _driverPoint = driverPoint;
        _legMountPoint = legMountPoint;

        if (leftLeg)
        {
            
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
        }
        else
        {
            Extensions.ComposeTriangle(
                _legMountPoint, _redShort.endDown,
                _driverPoint, _greenLink.endDown
                );
            
            Extensions.ComposeTriangle(
                _redShort.endDown, _redBar.endDown,
                _redShort.endUp, _redLong.endDown
                );

            Extensions.ComposeTriangle(
                _legMountPoint, _greenTrianglesLink.endUp,
                _driverPoint, _orangeLink.endUp
                );
            
            Extensions.ComposeTriangle(
                _greenTrianglesLink.endDown, _blueBar.endDown,
                _redBar.endUp, _purpleTrianglesLink.endDown
                );
            
            Extensions.ComposeTriangle(
                _blueBar.endUp, _blueLong.endUp,
                _blueBar.endDown, _blueShort.endUp
                );
        }
        
        SetupLinkages();
    }

    private void SetupLinkages()
    {
        _greenLink.endDown.HingeConnectToBody(_driverPoint);
        _orangeLink.endUp.HingeConnectToBody(_greenLink.endDown);
        
        var redTriangleDrivePoint = _redLong.endDown;
        
        // red
        redTriangleDrivePoint.HingeConnectToBody(_greenLink.endUp);
        
        _redShort.endUp.FixedConnectToBody(redTriangleDrivePoint);
        _redBar.endDown.FixedConnectToBody(_redShort.endDown);
        
        // blue
        _blueBar.endDown.HingeConnectToBody(_orangeLink.endDown);
        _blueShort.endUp.FixedConnectToBody(_blueBar.endDown);
        _blueLong.endUp.FixedConnectToBody(_blueBar.endUp);

        // triangle links
        _redShort.endDown.HingeWith(_legMountPoint);
        
        _greenTrianglesLink.endUp.HingeConnectToBody(_redBar.endDown);
        _greenTrianglesLink.endDown.HingeWith(_blueBar.endDown);
        
        _purpleTrianglesLink.endDown.HingeConnectToBody(_redBar.endUp);
        _purpleTrianglesLink.endUp.HingeWith(_blueBar.endUp);
    }
}
