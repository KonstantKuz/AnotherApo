using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoCached
{
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

    private static bool firing;
    public static bool Firing
    {
        get { return firing; }
    }

    private static bool melee;
    public static bool Melee
    {
        get { return melee; }
    }

    public static Action OnSwordAttacked;
    public static Action<bool> OnWeaponSwitched;
    public static Action OnJumped;
    public static Action OnDashed;
    public static Action OnBeatRegenerate;
    
    private string VerticalName = "Vertical";
    private string HorizontalName = "Horizontal";
    private string MouseXName = "Mouse X";
    private string MouseYName = "Mouse Y";

    private void Start()
    {
        GameStarter.OnGameStarted += delegate
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        };
    }
    
    public override void CustomUpdate()
    {
        if (!GameStarter.IsGameStarted)
        {
            return;
        }
        
        vertical = Input.GetAxis(VerticalName);
        horizontal = Input.GetAxis(HorizontalName);
        mouseX = Mathf.Lerp(mouseX, Input.GetAxis(MouseXName) * MouseXSensiv, Time.deltaTime * MouseXSmooth);
        mouseY = Mathf.Lerp(mouseY, Input.GetAxis(MouseYName) * MouseYSensiv, Time.deltaTime * MouseYSmooth);
        mouseX = Mathf.Clamp(mouseX, -1, 1);
        mouseY = Mathf.Clamp(mouseY, -0.3f, 0.7f);

        if (Input.GetMouseButton(0))
        {
            firing = true;
        }
        else
        {
            firing = false;
        }

        if (melee && Input.GetMouseButtonDown(0))
        {
            OnSwordAttacked();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            //SwitchWeapon();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnDashed();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumped();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Tilde))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnBeatRegenerate();
        }
    }

    private void SwitchWeapon()
    {
        melee = !melee;
        OnWeaponSwitched(melee);
    }
}
