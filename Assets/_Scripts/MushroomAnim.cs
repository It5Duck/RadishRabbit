using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomAnim : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    const string SHROOM_BOUNCE = "RMushroomBounce";

    public void PlayAnimation()
    {
        //Play the animation
        animator.Play(SHROOM_BOUNCE);
    }
}
