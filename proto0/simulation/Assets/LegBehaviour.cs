using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

public class LegBehaviour : MonoBehaviour
{
    public float boneSizeScale = 0.01f;
    private BoneBehaviour barLong;
    private BoneBehaviour barShort;
    private BoneBehaviour rotorBar;

    private void OnEnable()
    {
        CreateBlueTriangle();
        CreateRedTriangle();
    }

    private void CreateBlueTriangle()
    {
        var triangle = new GameObject("blueTriangle");
        triangle.transform.SetParent(gameObject.transform);

        var boneBar = CreateBone("boneBlue", 36.7f, triangle);
        var boneLong = CreateBone("boneBlueLong", 65.7f, triangle);
        var boneShort = CreateBone("boneBlueShort", 49f, triangle);
        
        boneBar.Rotate(90f);
        Extensions.ComposeTriangle(
            boneBar.endDown, boneShort.endUp,
            boneBar.endUp, boneLong.endUp);
    }
    
    private void CreateRedTriangle()
    {
        var redTriangle = new GameObject("redTriangle");
        

        redTriangle.transform.SetParent(gameObject.transform);

        var boneBar = CreateBone("boneRed", 40.1f, redTriangle);
        var boneLong = CreateBone("boneRedLong", 55.8f, redTriangle);
        var boneShort = CreateBone("boneRedShort", 41.5f, redTriangle);
        
        boneBar.Rotate(90f);
        Extensions.ComposeTriangle(
            boneBar.endDown, boneShort.endDown,
            boneBar.endUp, boneLong.endDown);
        
        redTriangle.transform.position += new Vector3(0f, 0.3f, 0f);
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
