using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private PlayerWeaponHolder weaponHolder;

    private void OnEnable()
    {
        weaponHolder.gun.SetDamageValue(Constants.DamagePerHit.Player);

        // PlayerInput.OnWeaponSwitched += SwitchWeapon;
        // PlayerInput.OnSwordAttacked += SwordAttack;
        GameBeatSequencer.OnBPM += delegate
        {
            if (PlayerInput.Firing)
            {
                weaponHolder.gun.Fire();
            }
        };
    }

    // private void SwitchWeapon(bool Melee)
    // {
    //     weaponHolder.SwitchWeapons(Melee);
    // }
    //
    // private void SwordAttack()
    // {
    //     animator.SetTrigger(AnimatorHashes.SwordAttackHash);
    // }
}
