using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //To prevent unnoticable errors with strings, set the string to a const var
    private const string IS_WALKING = "isWalking";
    private Animator animator;
    //Make sure to compile first to place the game object into player
    [SerializeField] private Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update () 
    {
        //Remember to put the SetBool in Update to get it every frame 
        animator.SetBool(IS_WALKING, player.IsWalking());
    }


}
