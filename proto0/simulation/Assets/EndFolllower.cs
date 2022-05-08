using System;
using UnityEngine;

public class EndFolllower : MonoBehaviour
{
    public EndBehaviour target;
    
    private void FixedUpdate()
    {
        gameObject.transform.position = target.gameObject.transform.position;
    }
}