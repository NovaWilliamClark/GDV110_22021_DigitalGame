using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.IK;

public class ArmMouseTracking : MonoBehaviour
{
    [SerializeField] public CCDSolver2D solver;
    [SerializeField] private GameObject shoulderBone;
    [SerializeField] private GameObject target;

    [SerializeField] private float distance;

    private CharacterController cc;
    
    void Start()
    {
        cc = GetComponentInParent<CharacterController>();
    }
    
    public void FixedUpdate()
    {
        // Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(Input.mousePosition);
        // targetScreenPosition.z = transform.position.z;
        // Vector3 targetToMouseDir = Input.mousePosition - targetScreenPosition;
        // Vector3 targetToMe = shoulderBone.transform.position - target.transform.position;
        // targetToMe.z = 0;
        //
        // Vector3 newTargetToMe = Vector3.RotateTowards(targetToMe, targetToMouseDir, 1, 0f);
        //
        // transform.position = target.transform.position + 10 * newTargetToMe.normalized;
        
        var mouseRaw = Mouse.current.position.ReadValue();
        var mouse = new Vector3(mouseRaw.x, mouseRaw.y, Vector3.Distance(transform.position, Camera.main.transform.position));
        var mousePos = Camera.main.ScreenToWorldPoint(mouse);

        var flip = cc.IsFacingLeft() ? -1 : 1;
        mousePos.x = shoulderBone.transform.position.x + (distance * flip);

            var dir = mousePos - shoulderBone.transform.position;
        dir.Normalize();

        target.transform.position = shoulderBone.transform.position + (distance * dir);
        transform.position = target.transform.position;
    }
}
