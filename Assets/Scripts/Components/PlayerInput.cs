using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoCached
{
    public float VerticalSensiv;
    public float HorizontalSensiv;

    public float MouseXSmooth;
    public float MouseYSmooth;

    public float MouseXSensiv;
    public float MouseYSensiv;

    public static float Vertical;
    public static float Horizontal;
    public static float MouseX;
    public static float MouseY;
    public static bool Aiming;
    public static bool Firing;
    public static bool Crouching;
    public static bool Melee;
    public static Action SwordAttack;

    private string VerticalName = "Vertical";
    private string HorizontalName = "Horizontal";
    private string MouseXName = "Mouse X";
    private string MouseYName = "Mouse Y";

    public override void CustomUpdate()
    {
        Vertical = Input.GetAxis(VerticalName);
        Horizontal = Input.GetAxis(HorizontalName);
        MouseX = Mathf.Lerp(MouseX, Input.GetAxis(MouseXName) * MouseXSensiv, Time.deltaTime * MouseXSmooth);
        MouseY = Mathf.Lerp(MouseY, Input.GetAxis(MouseYName) * MouseYSensiv, Time.deltaTime * MouseYSmooth);

        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Aim();
        }

        if (Input.GetMouseButton(0))
        {
            Firing = true;
        }
        else
        {
            Firing = false;
        }

        if (Input.GetMouseButtonDown(0) && Melee)
        {
            SwordAttack();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchAttackType();
        }
        
        MouseX = Mathf.Clamp(MouseX, -1, 1);
        MouseY = Mathf.Clamp(MouseY, -0.3f, 0.7f);
    }

    private void SwitchAttackType()
    {
        Melee = !Melee;
    }

    public void Aim()
    {
        Aiming = !Aiming;
    }

    public void Crouch()
    {
        Crouching = !Crouching;
    }
}
