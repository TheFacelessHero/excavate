using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform anchor;
    [SerializeField] float length = 1;
    [SerializeField] float maxDistance = 0;
    [SerializeField] float minDistance = 0;

    private void FixedUpdate()
    {

        //PointTowards
        Vector3 targ;
        targ = target.position - transform.position;

        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //MoveTowards

        transform.position = anchor.position + transform.right * Mathf.Clamp(Vector2.Distance(target.position, anchor.position) - length, minDistance, maxDistance);
    }

}
