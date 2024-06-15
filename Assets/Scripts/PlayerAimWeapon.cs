using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerAimWeapon : MonoBehaviour
{
    private Transform aimTransform;
    private Animator aimChildAnimator;

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        aimChildAnimator = aimTransform.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
            HandleAiming();
            HandleShooting();
    }


    private void HandleAiming()
    {
        Vector3 mousePosition = MouseUtils.GetMouseWorldPosition2D();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 aimlocalScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            aimlocalScale.y = -1f;
        }
        else
        {
            aimlocalScale.y = +1f;
        }
        aimTransform.localScale = aimlocalScale;
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = MouseUtils.GetMouseWorldPosition2D();
            aimChildAnimator.SetTrigger("Shoot");
        }
    }

}
