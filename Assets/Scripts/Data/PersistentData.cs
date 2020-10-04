﻿using System.Collections;
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
    public static LayerMask Ground;
    public static void CacheLayers()
    {
        interactionLayer = LayerMask.NameToLayer("InteractionLayer");
        Ground = LayerMask.NameToLayer("Ground");
    }
}

public class Constants
{
    public const string PoolBulletTrail = "BulletTrail";
    
    public const string PoolFlashSmall = "MuzzleFlashSmall";
    public const string PoolFlashMid = "MuzzleFlashMid";
    
    public const string PoolExplosionMid = "MidExplosion";
    public const string PoolExplosionBig = "BigExplosion";
    
    public const string PoolHitGroundSmall = "HitGroundSmall";
    public const string PoolHitMetal = "HitMetal";

    public class TotalHealth
    {
        public const int Player = 50000;
        public const int Durashka = 1000;
        public const int SpiderAst = 2000;
        public const int UMUGun = 3000;
    }
    
    public class DamagePerHit
    {
        public const int Player = 200;
        public const int Durashka = 100;
        public const int SpiderAst = 500;
        public const int UMUGun = 300;
    }
}