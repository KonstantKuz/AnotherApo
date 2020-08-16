using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIKOverride : MonoCached
{
    private Animator anim;
    
    private Transform leftFoot;
    private Transform rightFoot;

    private Vector3 newRFPos;
    private Vector3 newLFPos;

    private Quaternion newRFRot;
    private Quaternion newLFRot;

    private RaycastHit rhit;
    private RaycastHit lhit;
    public float weight;

    public Vector3 offsetY;

    private Vector3 rfstart;
    private Vector3 lfstart;

    public float maxydiff;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);

        newRFRot = rightFoot.rotation;
        newLFRot = leftFoot.rotation;
        
        rfstart = rightFoot.position;
        lfstart = leftFoot.position;
    }

    // Update is called once per frame
    public override void CustomUpdate()
    {
        if (Physics.Raycast(rightFoot.position + Vector3.up, Vector3.down, out rhit, 3f))
        {
            newRFPos = rhit.point;
            //newRFRot = Quaternion.FromToRotation(_transform.up, rhit.normal) * _transform.rotation;
        }
        if (Physics.Raycast(leftFoot.position + Vector3.up, Vector3.down, out lhit, 3f))
        {
            newLFPos = lhit.point;
            //newLFRot = Quaternion.FromToRotation(_transform.up, lhit.normal) * _transform.rotation;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if((leftFoot.position - lfstart).magnitude < maxydiff)
        {
            //anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, weight);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weight);
        }
        if ((rightFoot.position - rfstart).magnitude < maxydiff)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, weight);
            //anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, weight);
        }

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, newLFPos + offsetY);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, newRFPos + offsetY);
        //anim.SetIKRotation(AvatarIKGoal.RightFoot, newRFRot);
        //anim.SetIKRotation(AvatarIKGoal.LeftFoot, newLFRot);
    }
}
