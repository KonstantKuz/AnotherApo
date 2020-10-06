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
    public static int SwordAttackHash;
    public static int AimingHash;
    public static int JumpHash;
    public static int LandingHash;
    public static int DeathHash;
    public static int DeathTypeHash;
    public static int VerticalHash;
    public static int HorizontalHash;
    
    public static class SpiderHashes
    {
        public static int SpeedMultiplier;
    }

    public static void CacheHashes()
    {
        SwordAttackHash = Animator.StringToHash("SwordAttack");
        AimingHash = Animator.StringToHash("Aiming");
        JumpHash = Animator.StringToHash("Jump");
        LandingHash = Animator.StringToHash("Landing");
        DeathHash = Animator.StringToHash("Death");
        DeathTypeHash = Animator.StringToHash("DeathType");
        VerticalHash = Animator.StringToHash("Vertical_Move");
        HorizontalHash = Animator.StringToHash("Horizontal_Move");

        SpiderHashes.SpeedMultiplier = Animator.StringToHash("SpeedMultiplier");
        
        LayerMasks.CacheLayers();
    }
}

public class LayerMasks
{
    public static LayerMask Ground;
    public static LayerMask Player;
    public static void CacheLayers()
    {
        Ground = LayerMask.NameToLayer("Ground");
        Player = LayerMask.NameToLayer("Player");
    }
    // yourLayerMask |= (1 << LayerMask.NameToLayer("Default"));
    // yourLayerMask |= (1 << LayerMask.NameToLayer("AnotherLayer"));
    // yourLayerMask |= (1 << LayerMask.NameToLayer("Water"));
}

public class Constants
{
    public const string PoolBulletTrail = "BulletTrail";
    
    public const string PoolFlashSmall = "MuzzleFlashSmall";
    public const string PoolFlashMid = "MuzzleFlashMid";
    
    public const string PoolExplosionMid = "ExplosionMid";
    public const string PoolExplosionBig = "ExplosionBig";
    
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

    public class HealPerExplosion
    {
        public const int Durashka = -500;
        public const int UMUGun = -250;
        public const int UMUSmall = -500;
        public const int UMUBig = -1000;
    }

    public const float ExplosionRadiusSmall = 4f;
    public const float ExplosionRadiusBig = 8f;

    public const string EmissionProperty = "_EmissionColor";
    public const string EmissionKeyword = "_EMISSION";
}