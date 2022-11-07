using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class ArmMouseTracking : MonoBehaviour
{
    [SerializeField] public CCDSolver2D solver;
    [SerializeField] private GameObject shoulderBone;
    [SerializeField] private GameObject target;

    public void FixedUpdate()
    {
        Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(target.transform.position);
        targetScreenPosition.z = 0;
        Vector3 targetToMouseDir = Input.mousePosition - targetScreenPosition;

        Vector3 targetToMe = transform.position - target.transform.position;
        targetToMe.z = 0;

        Vector3 newTargetToMe = Vector3.RotateTowards(targetToMe, targetToMouseDir, 1, 0f);

        transform.position = target.transform.position + 10 * newTargetToMe.normalized;
    }
}
