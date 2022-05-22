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
        _blueLong = this.CreateBone("boneBlueLong", 65.7f, true);
        _blueShort = this.CreateBone("boneBlueShort", 49f, true);
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
                _driverPoint, _greenLink.endDown);
            
            Extensions.ComposeTriangle(
                _redShort.endDown, _redBar.endDown,
                _redShort.endUp, _redLong.endDown);

            Extensions.ComposeTriangle(
                _legMountPoint, _greenTrianglesLink.endUp,
                _driverPoint, _orangeLink.endUp);
            
            Extensions.ComposeTriangle(
                _greenTrianglesLink.endDown, _blueBar.endDown,
                _redBar.endUp, _purpleTrianglesLink.endDown);
            
            Extensions.ComposeTriangle(
                _blueBar.endUp, _blueLong.endUp,
                _blueBar.endDown, _blueShort.endUp);
        }
        
        SetupLinkages();
    }

    private void SetupLinkages()
    {
        _redBar.gameObject.transform.SetParent(gameObject.transform, true);

        Extensions.SetParent(_redLong, _redBar.gameObject);
        Extensions.SetParent(_redShort, _redBar.gameObject);

        _blueBar.gameObject.transform.SetParent(gameObject.transform, true);
        
        Extensions.SetParent(_blueLong, _blueBar.gameObject);
        Extensions.SetParent(_blueShort, _blueBar.gameObject);

        _purpleTrianglesLink.gameObject.AddComponent<Rigidbody>();
        _greenTrianglesLink.gameObject.AddComponent<Rigidbody>();
        _greenLink.gameObject.AddComponent<Rigidbody>();
        _orangeLink.gameObject.AddComponent<Rigidbody>();
        _blueBar.gameObject.AddComponent<Rigidbody>();
        _redBar.gameObject.gameObject.AddComponent<Rigidbody>();

        _greenTrianglesLink.endDown.HingeWith(_blueBar.endDown);
        _greenTrianglesLink.endUp.HingeWith(_legMountPoint);
        _redBar.endDown.HingeWith(_legMountPoint);
        
        _purpleTrianglesLink.endDown.HingeWith(_redBar.endUp);
        _purpleTrianglesLink.endUp.HingeWith(_blueBar.endUp);
        
        _greenLink.endUp.HingeWith(_redLong.endDown);
        _greenLink.endDown.HingeWith(_driverPoint);
        _orangeLink.endUp.HingeWith(_driverPoint);
        _orangeLink.endDown.HingeWith(_blueShort.endUp);
    }
}
