using UnityEngine;

[System.Serializable]
public class PlayerWeaponHolder : MonoBehaviour
{
    public Gun gun;
    public GameObject sword;
    public Transform gunPlace_In;
    public Transform gunPlace_Out;
    public Transform swordPlace_In;
    public Transform swordPlace_Out;
    

    public void SwitchWeapons(bool Melee)
    {
        if (Melee)
        {
            SetGunOut();
            SetSwordIn();
        }
        else
        {
            SetSwordOut();
            SetGunIn();
        }
    }

    private void SetGunIn()
    {
        gun.transform.parent = gunPlace_In.parent;
        gun.transform.localPosition = gunPlace_In.localPosition;
        gun.transform.localRotation = gunPlace_In.localRotation;
    }

    private void SetGunOut()
    {
        gun.transform.parent = gunPlace_Out.parent;
        gun.transform.localPosition = gunPlace_Out.localPosition;
        gun.transform.localRotation = gunPlace_Out.localRotation;
    }

    private void SetSwordIn()
    {
        sword.transform.parent = swordPlace_In.parent;
        sword.transform.localPosition = swordPlace_In.localPosition;
        sword.transform.localRotation = swordPlace_In.localRotation;
    }

    private void SetSwordOut()
    {
        sword.transform.parent = swordPlace_Out.parent;
        sword.transform.localPosition = swordPlace_Out.localPosition;
        sword.transform.localRotation = swordPlace_Out.localRotation;
    }
}