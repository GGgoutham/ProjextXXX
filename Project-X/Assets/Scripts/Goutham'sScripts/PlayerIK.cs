using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{

    public Transform LeftHandIKTarget;
    public Transform RightHandIKTarget;
    public Transform LeftElbowIKTarget;
    public Transform RightElbowIKTarget;

    [Range(0, 1f)]
    public float HandIKAmmount = 1f;
    [Range(0, 1f)]
    public float ElbowIKAmmount = 1f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnAnimatorIK(int layerIndex)
    {
        if (LeftHandIKTarget != null)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, HandIKAmmount);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, HandIKAmmount);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);

        }
        if (RightElbowIKTarget != null)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, HandIKAmmount);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, HandIKAmmount);
            animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);

        }
        if (LeftElbowIKTarget != null)
        {
            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowIKTarget.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ElbowIKAmmount);

        }
        if (RightElbowIKTarget != null)
        {
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowIKTarget.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ElbowIKAmmount);

        }
    }

    public void Setup(Transform GunParent)
    {
        Transform[] allChildern = GunParent.GetComponentsInChildren<Transform>();

        LeftElbowIKTarget = allChildern.FirstOrDefault(child =>child.name =="LeftElbow");
        RightElbowIKTarget = allChildern.FirstOrDefault(child => child.name == "RightElbow");
        LeftHandIKTarget = allChildern.FirstOrDefault(child => child.name == "LeftHand");
        RightHandIKTarget = allChildern.FirstOrDefault(child => child.name == "RightHand");
    }


}
