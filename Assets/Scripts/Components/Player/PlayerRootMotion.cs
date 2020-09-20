using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRootMotion : MonoBehaviour
{

    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private void Awake()
    {
        originalLocalPosition = animator.transform.localPosition;
        originalLocalRotation = animator.transform.localRotation;
    }

    private void Update()
    {
        if (!PlayerInput.Melee)
        {
            return;
        }

        controller.enabled = false;

        transform.position = animator.transform.position;

        animator.transform.RotateAround(animator.transform.position, animator.transform.forward,
                                        -originalLocalRotation.eulerAngles.z);
        animator.transform.RotateAround(animator.transform.position, animator.transform.right,
                                        -originalLocalRotation.eulerAngles.x);
        animator.transform.RotateAround(animator.transform.position, animator.transform.up,
                                        -originalLocalRotation.eulerAngles.y);

        transform.rotation = animator.transform.rotation;

        transform.position += -transform.right * originalLocalPosition.x;
        transform.position += -transform.up * originalLocalPosition.y;
        transform.position += -transform.forward * originalLocalPosition.z;

        animator.transform.localRotation = originalLocalRotation;
        animator.transform.localPosition = originalLocalPosition;

        controller.enabled = true;
        
    }
}