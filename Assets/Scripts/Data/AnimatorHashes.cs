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
    public static int JumpHash;
    public static int LandingHash;
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
        JumpHash = Animator.StringToHash("Jump");
        LandingHash = Animator.StringToHash("Landing");
        CoverHash = Animator.StringToHash("Cover");
        CoverSideHash = Animator.StringToHash("Cover_Side");
        VerticalHash = Animator.StringToHash("Vertical_Move");
        HorizontalHash = Animator.StringToHash("Horizontal_Move");
        Mouse_XHash = Animator.StringToHash("Mouse_X");
        Mouse_YHash = Animator.StringToHash("Mouse_Y");
        
        LayerMasks.CacheLayers();
    }
}

public class LayerMasks
{
    public static LayerMask interactionLayer;
    public static LayerMask coverLayer;

    public static void CacheLayers()
    {
        interactionLayer = LayerMask.NameToLayer("InteractionLayer");
    }
}