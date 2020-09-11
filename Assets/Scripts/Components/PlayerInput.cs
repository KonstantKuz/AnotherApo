using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoCached
{
    [SerializeField] private float VerticalSensiv;
    [SerializeField] private float HorizontalSensiv;

    [SerializeField] private float MouseXSmooth;
    [SerializeField] private float MouseYSmooth;

    [SerializeField] private float MouseXSensiv;
    [SerializeField] private float MouseYSensiv;

    private static float vertical;
    public static float Vertical
    {
        get { return vertical; }
    }

    private static float horizontal;
    public static float Horizontal
    {
        get { return horizontal; }
    }

    private static float mouseX;
    public static float MouseX
    {
        get { return mouseX; }
    }

    private static float mouseY;
    public static float MouseY
    {
        get { return mouseY; }
    }

    private static bool aiming;
    public static bool Aiming
    {
        get { return aiming; }
    }

    private static bool firing;
    public static bool Firing
    {
        get { return firing; }
    }

    private static bool crouching;
    public static bool Crouching
    {
        get { return crouching; }
    }

    private static bool shifting;
    public static bool Shifting
    {
        get { return shifting; }
    }

    private static bool melee;
    public static bool Melee
    {
        get { return melee; }
    }

    public static Action OnSwordAttacked;
    public static Action OnWeaponSwitched;
    public static Action OnJumped;

    private string VerticalName = "Vertical";
    private string HorizontalName = "Horizontal";
    private string MouseXName = "Mouse X";
    private string MouseYName = "Mouse Y";

    public override void CustomUpdate()
    {
        vertical = Input.GetAxis(VerticalName);
        horizontal = Input.GetAxis(HorizontalName);
        mouseX = Mathf.Lerp(mouseX, Input.GetAxis(MouseXName) * MouseXSensiv, Time.deltaTime * MouseXSmooth);
        mouseY = Mathf.Lerp(mouseY, Input.GetAxis(MouseYName) * MouseYSensiv, Time.deltaTime * MouseYSmooth);
        mouseX = Mathf.Clamp(mouseX, -1, 1);
        mouseY = Mathf.Clamp(mouseY, -0.3f, 0.7f);
        
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            SwitchAiming();
        }

        if (Input.GetMouseButton(0))
        {
            firing = true;
        }
        else
        {
            firing = false;
        }

        if (Input.GetMouseButtonDown(0) && melee)
        {
            OnSwordAttacked();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCrouching();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if(aiming)
                SwitchAiming();
            SwitchWeapon();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Shift();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumped();
        }
    }

    private void SwitchWeapon()
    {
        melee = !melee;
        OnWeaponSwitched();
    }

    private void SwitchAiming()
    {
        aiming = !aiming;
    }

    private void SwitchCrouching()
    {
        crouching = !crouching;
    }

    private void Shift()
    {
        shifting = !shifting;
    }
}
