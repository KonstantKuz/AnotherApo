using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHashes : MonoBehaviour
{
    public static int AimingHash;
    public static int CrouchingHash;
    public static int SprintHash;
    public static int CoverHash;
    public static int CoverSideHash;
    public static int VerticalHash;
    public static int HorizontalHash;
    public static int Mouse_XHash;
    public static int Mouse_YHash;


    private void Awake()
    {
        AimingHash = Animator.StringToHash("Aiming");
        CrouchingHash = Animator.StringToHash("Crouching");
        SprintHash = Animator.StringToHash("Sprint");
        CoverHash = Animator.StringToHash("Cover");
        CoverSideHash = Animator.StringToHash("Cover_Side");
        VerticalHash = Animator.StringToHash("Vertical_Move");
        HorizontalHash = Animator.StringToHash("Horizontal_Move");
        Mouse_XHash = Animator.StringToHash("Mouse_X");
        Mouse_YHash = Animator.StringToHash("Mouse_Y");
    }
}
