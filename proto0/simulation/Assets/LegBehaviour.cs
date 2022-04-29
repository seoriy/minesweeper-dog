using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = System.Object;

public class LegBehaviour : MonoBehaviour
{
    //Jansen linkage scheme https://en.wikipedia.org/wiki/Jansen%27s_linkage#/media/File:Jansen's_Linkage.svg
    public float boneSizeScale = 0.01f;
    private BoneBehaviour _barLong;
    private BoneBehaviour _barShort;
    private BoneBehaviour _rotorBar;
    
    private GameObject _blueTriangle;
    private BoneBehaviour _boneBlueBar;
    private BoneBehaviour _boneBlueLong;
    private BoneBehaviour _boneBlueShort;

    private BoneBehaviour _boneGreenLink;
    private BoneBehaviour _boneOrangeLink;
    private BoneBehaviour _bonePurpleTrianglesLink;
    private BoneBehaviour _boneGreenTrianglesLink;

    private GameObject _redTriangle;
    private BoneBehaviour _boneRedBar;
    private BoneBehaviour _boneRedLong;
    private BoneBehaviour _boneRedShort;

    private EndBehaviour _rotationCenter;
    private EndBehaviour _legMountPoint;
    private EndBehaviour _driverPoint;

    private void InitBones()
    {
        var fixedTriangle = new GameObject("fixedTriangle");
        fixedTriangle.transform.SetParent(gameObject.transform);

        _barShort = CreateBone("boneShort", 7.8f, fixedTriangle);
        _rotorBar = CreateBone("boneBar", 15, fixedTriangle);
        _barLong = CreateBone("boneLong", 38f, fixedTriangle);

        _rotationCenter = _barShort.endUp;
        _legMountPoint = _barLong.endDown;
        _driverPoint = _rotorBar.endUp;

        _boneGreenLink = CreateBone("boneGreenLink", 50f, gameObject);
        _boneOrangeLink = CreateBone("boneOrangeLink", 61.9f, gameObject);
        _bonePurpleTrianglesLink = CreateBone("bonePurpleTrianglesLink", 39.4f, gameObject);
        _boneGreenTrianglesLink = CreateBone("boneGreenTrianglesLink", 39.3f, gameObject);

        _blueTriangle = new GameObject("blueTriangle");
        _blueTriangle.transform.SetParent(gameObject.transform);
        _boneBlueBar = CreateBone("boneBlue", 36.7f, _blueTriangle);
        _boneBlueLong = CreateBone("boneBlueLong", 65.7f, _blueTriangle);
        _boneBlueShort = CreateBone("boneBlueShort", 49f, _blueTriangle);

        _redTriangle = new GameObject("redTriangle");
        _redTriangle.transform.SetParent(gameObject.transform);

        _boneRedBar = CreateBone("boneRed", 40.1f, _redTriangle);
        _boneRedLong = CreateBone("boneRedLong", 55.8f, _redTriangle);
        _boneRedShort = CreateBone("boneRedShort", 41.5f, _redTriangle);
    }

    private void OnEnable()
    {
        InitBones();
        
        // set motor
        _barShort.endUp.ShiftTo(gameObject.transform.position);
        _rotorBar.endDown.ShiftTo(_barShort.endUp);
        _barLong.Rotate(-90f);
        _barLong.endUp.ShiftTo(_barShort.endDown);
        
        Extensions.ComposeTriangle(
            _driverPoint, _boneGreenLink.endDown,
            _legMountPoint, _boneRedShort.endDown);
        
        Extensions.ComposeTriangle(
            _boneRedShort.endUp, _boneRedLong.endDown,
            _boneRedShort.endDown, _boneRedBar.endDown);
        
        Extensions.ComposeTriangle(
            _driverPoint, _boneOrangeLink.endUp,
            _legMountPoint, _boneGreenTrianglesLink.endUp);
        
        Extensions.ComposeTriangle(
            _boneRedBar.endUp, _bonePurpleTrianglesLink.endDown,
            _boneGreenTrianglesLink.endDown, _boneBlueBar.endDown);
        
        //  _boneBlueBar.Rotate(90f);
        Extensions.ComposeTriangle(
            _boneBlueBar.endDown, _boneBlueShort.endUp,
            _boneBlueBar.endUp, _boneBlueLong.endUp);
        
        _barShort.gameObject.SetActive(false);
        _barLong.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _boneOrangeLink.gameObject.SetActive(!_boneOrangeLink.gameObject.activeSelf);
            _boneGreenLink.gameObject.SetActive(!_boneGreenLink.gameObject.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _redTriangle.SetActive(!_redTriangle.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _bonePurpleTrianglesLink.gameObject.SetActive(!_bonePurpleTrianglesLink.gameObject.activeSelf);
            _boneGreenTrianglesLink.gameObject.SetActive(!_boneGreenTrianglesLink.gameObject.activeSelf);
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

    private BoneBehaviour CreateBone(string boneTag, float size, GameObject parent)
    {
        var boneObject = new GameObject();
        boneObject.name = boneTag;
        boneObject.transform.SetParent(parent.transform);

        var bone = boneObject.AddComponent<BoneBehaviour>();
        bone.length = size * boneSizeScale;
        bone.OnValidate();
        
        return bone;
    }
}
