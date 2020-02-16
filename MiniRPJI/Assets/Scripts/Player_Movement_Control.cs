/* Player_Control.cs
    Utilisé pour gérer les mouvements du joueur ainsi que les animations
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Stats_Control))]
public class Player_Movement_Control : MonoBehaviour
{
    Rigidbody2D myRb;
    Animator animator;
    Vector2 animatorVector; // To set X,Y values into animator
    Stats_Control currentStats;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentStats = GetComponent<Stats_Control>();

        animatorVector = new Vector2();
    }

    void Update()
    {
        SimpleAnimatorControl();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!animator.GetBool("isAttacking"))
        {
            SimplePlayerMovement();
        }
        else
        {
            if (myRb.velocity != Vector2.zero)
            {
                myRb.velocity = Vector2.zero;
            }
        }
        
    }

    void SimpleAnimatorControl()
    {
        if (animatorVector != myRb.velocity)
        {
            animatorVector = myRb.velocity;
            animator.SetFloat("VectorX", animatorVector.x);
            animator.SetFloat("VectorY", animatorVector.y);
        }

        if (animatorVector != Vector2.zero && !animator.GetBool("isMoving"))
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            if (animator.GetBool("isMoving"))
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    void SimplePlayerMovement()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            myRb.velocity = new Vector2(0f, 1f) * currentStats.GetSpeed();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            myRb.velocity = new Vector2(0f, -1f) * currentStats.GetSpeed();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            myRb.velocity = new Vector2(1f, 0f) * currentStats.GetSpeed();
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            myRb.velocity = new Vector2(-1f, 0f) * currentStats.GetSpeed();
        }
        else
        {
            if (myRb.velocity != Vector2.zero)
            {
                myRb.velocity = Vector2.zero;
            }
        }
    }

    
}
