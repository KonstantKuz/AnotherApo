using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHashes : MonoBehaviour
{
    public static int MeleeHash;
    public static int SwordAttackHash;
    public static int AimingHash;
    public static int CrouchingHash;
    public static int ShiftingHash;
    public static int Jumphash;
    public static int CoverHash;
    public static int CoverSideHash;
    public static int VerticalHash;
    public static int HorizontalHash;
    public static int Mouse_XHash;
    public static int Mouse_YHash;


    private void Awake()
    {
        MeleeHash = Animator.StringToHash("Melee");
        SwordAttackHash = Animator.StringToHash("SwordAttack");
        AimingHash = Animator.StringToHash("Aiming");
        CrouchingHash = Animator.StringToHash("Crouching");
        ShiftingHash = Animator.StringToHash("Shifting");
        Jumphash = Animator.StringToHash("Jump");
        CoverHash = Animator.StringToHash("Cover");
        CoverSideHash = Animator.StringToHash("Cover_Side");
        VerticalHash = Animator.StringToHash("Vertical_Move");
        HorizontalHash = Animator.StringToHash("Horizontal_Move");
        Mouse_XHash = Animator.StringToHash("Mouse_X");
        Mouse_YHash = Animator.StringToHash("Mouse_Y");
    }
}
