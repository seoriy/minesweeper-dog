using System;
using System.Numerics;
using UnityEditor.VersionControl;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public static class Extensions
{
    public static void ShiftTo(this GameObject target, GameObject from, GameObject to)
    {
        var diff = to.transform.position - from.transform.position;
        target.transform.position += diff;
    }

    public static void ComposeTriangle(
        EndBehaviour a1, EndBehaviour b1,
        EndBehaviour a2, EndBehaviour c2)
    {
        var baseDirection = a2.gameObject.transform.position - a1.gameObject.transform.position;
        var a = Math.Abs(baseDirection.magnitude);  
        var b =  b1.Bone.length;
        var c =  c2.Bone.length;
            
        var beta = CalculateAngle(a, c, b);
        var gamma = CalculateAngle(a, b, c);
        
        var angle1 = Vector3.SignedAngle(b1.Direction * Vector3.up, baseDirection, Vector3.forward);
        b1.BoneTransform.rotation =
            Quaternion.Euler(0, 0, angle1)
             * Quaternion.Euler(0, 0, b1.Direction * -Convert.ToSingle(gamma));
        
        b1.ShiftTo(a1);

        var angle2 = Vector3.SignedAngle(c2.Direction * Vector3.up, -baseDirection, Vector3.forward);
        
        c2.BoneTransform.rotation = 
            Quaternion.Euler(0, 0, angle2)
            * Quaternion.Euler(0, 0, c2.Direction * Convert.ToSingle(beta));
                        
        c2.ShiftTo(a2);
    }

    public static double CalculateAngle(BoneBehaviour triangleBase, BoneBehaviour triangleConnectedSide,
        BoneBehaviour triangleOppositeSide)
    {
        var a = Math.Abs(triangleOppositeSide.length);
        var b = Math.Abs(triangleConnectedSide.length);
        var c = Math.Abs(triangleBase.length);

        return CalculateAngle(c, b, a);
    }

    private static double CalculateAngle(float c, float b, float a)
    {
        double angle = 0;

        double dividend = b * b + c * c - a * a;
        double divisor = 2 * b * c;
        angle = Math.Acos(dividend / divisor) * Mathf.Rad2Deg;
        
        return angle ;
    }

    public static void Rotate(this Component component, double angleX, Vector3 axis = default) 
        => Rotate(component.gameObject, angleX, axis);
        
    public static void Rotate(this GameObject gameObject, double angleX, Vector3 axis = default)
        => gameObject.transform.Rotate(axis == default ? Vector3.forward : axis, Convert.ToSingle(angleX), Space.World);
}