using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BodyLookAtTarget : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 lookAtOffset;
    
    private Transform head;
    
    private void Start()
    {
        head = animator.GetBoneTransform(HumanBodyBones.Head);
    }

    private void OnAnimatorIK()
    {
        animator.SetLookAtWeight(1f);
        animator.SetLookAtPosition(target.position + lookAtOffset);
    }
}
