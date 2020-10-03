using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMUBot : Enemy
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    private Vector3 movementVelocity;
    private Vector3 verticalVelocity;
    
    public override void Start()
    {
        base.Start();
        
        pathUpdPeriod = 2f;
        UpdatePathPeriodically();
    }

    private void UpdatePathPeriodically()
    {
        StartCoroutine(UpdatePathToPlayerPeriodically());
        IEnumerator UpdatePathToPlayerPeriodically()
        {
            while (true)
            {
                UpdatePath(player.transform.position);
                yield return new WaitForSeconds(pathUpdPeriod);
            }
        }
    }

    public override void CustomUpdate()
    {
        if (player == null)
            return;
        
        ApplyGravity();
        
        RotateTowards(player.transform.position);

        CleanPassedNodes();
        if (!HasPath() || PathFullyPassed())
        {
            return;
        }

        CalculateMoveValuesToNextNode();
        MoveToNextNode();
    }

    private void ApplyGravity()
    {
        verticalVelocity += Physics.gravity * Time.deltaTime;
        verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, Physics.gravity.y, 100);
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    public override void MoveToNextNode()
    {
        Move(verticalMoveValue, horizontalMoveValue);
    }

    private void Move(float verticalValue, float horizontalValue)
    {
        float interpolatedVertical =
            Mathf.Lerp(animator.GetFloat(AnimatorHashes.VerticalHash), verticalValue, Time.deltaTime * 2f);
        
        float interpolatedHorizontal =
            Mathf.Lerp(animator.GetFloat(AnimatorHashes.HorizontalHash), horizontalValue, Time.deltaTime *  2f);
        
        animator.SetFloat(AnimatorHashes.VerticalHash, interpolatedVertical);
        animator.SetFloat(AnimatorHashes.HorizontalHash, interpolatedHorizontal);
        
        movementVelocity = Vector3.zero;
        
        movementVelocity.x = interpolatedHorizontal;
        movementVelocity.z = interpolatedVertical;
        
        movementVelocity = transform.TransformDirection(movementVelocity);
        movementVelocity *= movementSpeed;
        
        controller.Move(movementVelocity * Time.deltaTime);
    }
}
