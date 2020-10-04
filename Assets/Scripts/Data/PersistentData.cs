using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Animations;

public class PersistentData : MonoBehaviour
{
    private void Awake()
    {
        AnimatorHashes.CacheHashes();
        LayerMasks.CacheLayers();
    }
}

public class AnimatorHashes
{
    public static int MeleeHash;
    public static int SwordAttackHash;
    public static int AimingHash;
    public static int CrouchingHash;
    public static int ShiftingHash;
    public static int JumpHash;
    public static int LandingHash;
    public static int DeathHash;
    public static int DeathTypeHash;
    public static int CoverHash;
    public static int CoverSideHash;
    public static int VerticalHash;
    public static int HorizontalHash;
    public static int Mouse_XHash;
    public static int Mouse_YHash;
    
    public static class SpiderHashes
    {
        public static int Jump;
        public static int SpeedMultiplier;
    }

    public static void CacheHashes()
    {
        MeleeHash = Animator.StringToHash("Melee");
        SwordAttackHash = Animator.StringToHash("SwordAttack");
        AimingHash = Animator.StringToHash("Aiming");
        CrouchingHash = Animator.StringToHash("Crouching");
        ShiftingHash = Animator.StringToHash("Shifting");
        JumpHash = Animator.StringToHash("Jump");
        LandingHash = Animator.StringToHash("Landing");
        DeathHash = Animator.StringToHash("Death");
        DeathTypeHash = Animator.StringToHash("DeathType");
        CoverHash = Animator.StringToHash("Cover");
        CoverSideHash = Animator.StringToHash("Cover_Side");
        VerticalHash = Animator.StringToHash("Vertical_Move");
        HorizontalHash = Animator.StringToHash("Horizontal_Move");
        Mouse_XHash = Animator.StringToHash("Mouse_X");
        Mouse_YHash = Animator.StringToHash("Mouse_Y");

        SpiderHashes.Jump = Animator.StringToHash("Jump");
        SpiderHashes.SpeedMultiplier = Animator.StringToHash("SpeedMultiplier");
        
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

public class Constants
{
    public const string Cover = "Cover";

    public const string PoolTrail = "Trail";
    public const string PoolSmallFlash = "MuzzleFlash";
    public const string PoolMidFlash = "MidMuzzleFlash";
    public const string PoolMidExplosion = "MidExplosion";
    public const string PoolBigExplosion = "BigExplosion";
    
    public const string GeneralGraph = "General";
    public const string UMUBotGraph = "UMUBot";

    public class BulletCountsToDie
    {
        public const int Durashka = 5;
        public const int SpiderAst = 5;
        public const int UMUGun = 15;
    }
}