/* Player_Control.cs
    Utilisé pour gérer les mouvements des NPC
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AI_Moveset))]
[RequireComponent(typeof(AI_Combat_Control))] // To know target and combat relative stuff
public class AI_Movement_Control : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    [SerializeField] float stopFollowingOffset = 0.2f;
    float backToStartPosOffset = .4f; // When npc go back to his startPos, we need a little offset
    bool followingTarget = false; // Are we currently following ?
    Vector3 startPos; // Keep track where we from
    bool backToStartPos = true;

    Rigidbody2D myRb;
    Animator animator;
    Vector2 animatorVector; // To set X,Y values into animator
    AI_Moveset ai_moveset;
    AI_Combat_Control ai_combat;
    Player_Combat_Control player_combat; // To use for determine if player is in combat

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ai_moveset = GetComponent<AI_Moveset>();
        ai_combat = GetComponent<AI_Combat_Control>();

        animatorVector = new Vector2();

        startPos = transform.position; // Keep track from start pos
        backToStartPos = true; // At start we can suppose we're in start pos
    }

    void Update()
    {
        //// Think about set player as target when he's close with vector3.distance 

        SimpleAnimatorControl();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!animator.GetBool("isAttacking"))
        {
            SimpleAIMovement();
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
        else if (animatorVector == Vector2.zero)
        {
            if (animator.GetBool("isMoving"))
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    void SimpleAIMovement()
    {
        if (ai_combat.GetTarget()) // If there is a target
        {
            if (Vector3.Distance(transform.position, ai_combat.GetTarget().position) <= ai_combat.chasingDistance && !followingTarget) // if our distance from target is lesser than following distance and we're not already following
            {
                StartFollowingTarget();
            }
            else if (Vector3.Distance(transform.position, ai_combat.GetTarget().position) > ai_combat.chasingDistance && followingTarget) // if our distance is too far from target and we're following
            {
                StopFollowingTarget();
                return;
            }

            if (followingTarget) // If we're following target
            {
                ProcessFollowingTarget();
                return;
            }
        }

        ProcessAutoMove();
    }

    // Method to go back to start position and start automatique move via AI_Movement
    void ProcessAutoMove()
    {
        if (backToStartPos) // If we're back to start pos then process moveset
        {
            if (!ai_moveset.AutoMove)
            {
                ai_moveset.AutoMove = true;
            }
            if (!ai_moveset.readyForNextMove)
                return;
            else if (!ai_moveset.currentlyAutoMove)
            {
                ai_moveset.StartCoroutine("ProcessMovementSet");
            }
        }
        else // Else let's go  back to the startpos
        {
            ProcessFollowMovement(startPos);

            if (Vector3.Distance(transform.position, startPos) <= backToStartPosOffset)
            {
                backToStartPos = true;

                if (myRb.velocity != Vector2.zero) // Make sure we stop the npc to his startpos
                {
                    myRb.velocity = Vector2.zero;
                }
            }
        }
    }

    void StartFollowingTarget()
    {
        followingTarget = true; // start following         
        backToStartPos = false; // we go to undifined position so we need a way to go back to our startpos after that

        // Reset ai_moveset params
        if (ai_moveset.AutoMove)
            ai_moveset.AutoMove = false;

        if (ai_moveset.currentlyAutoMove)
            ai_moveset.currentlyAutoMove = false;

        ai_moveset.StopAllCoroutines(); // If we're already moving by moveset, stop coroutines.
        ai_moveset.ResetMovementParams();

        // Target is only the player for now, so we can say Player is in combat.
        player_combat = ai_combat.GetTarget().GetComponent<Player_Combat_Control>();
        if (player_combat)
        {
            player_combat.isInCombat = true;
            player_combat.ResetEndingCombatTimer();
        }
    }

    void StopFollowingTarget()
    {
        followingTarget = false;
        // Start player's ending combat timer
        // Target is only the player for now, so we can say Player is ending combat.
        player_combat = ai_combat.GetTarget().GetComponent<Player_Combat_Control>();
        if (player_combat)
        {
            player_combat.endingCombat = true;
        }
    }

    void ProcessFollowingTarget()
    {
        ProcessFollowMovement(ai_combat.GetTarget().position);
        if (player_combat) // Condition in case 2 or more enemies follow the player
        {
            if (player_combat.endingCombat)
            {
                player_combat.endingCombat = false;
                player_combat.ResetEndingCombatTimer();
            }
        }
    }

    // Method for follow a target
    void ProcessFollowMovement(Vector3 targetPos)
    {
        Vector3 direction = transform.position - targetPos;
        direction.z = 0;

        if (Vector3.Distance(transform.position, targetPos) > stopFollowingOffset) // To limit AI movement when he's close to the player
        {
            if (direction.y > 0.3f || direction.y < -0.3f) // I found 0.3 seems right for avoid "looping toggle direction bug" ( when there is no offset, npc loop infinitly right and left in a weird way)
            {
                if (direction.y < 0.2f)
                {
                    myRb.velocity = new Vector2(0f, 1f) * speed;

                }
                else if (direction.y > 0.2f)
                {
                    myRb.velocity = new Vector2(0f, -1f) * speed;
                }
            }

            if (direction.x > 0.3f || direction.x < -0.3f)
            {
                if (direction.x < 0.2f)
                {
                    myRb.velocity = new Vector2(1f, 0f) * speed;
                }
                else if (direction.x > 0.2f)
                {
                    myRb.velocity = new Vector2(-1f, 0f) * speed;
                }
            }
        }
        else
        {
            if (myRb.velocity != Vector2.zero)
                myRb.velocity = Vector2.zero;
        }
    }

    // Security for avoid player stay always in combat
    private void OnDestroy()
    {
        if (player_combat)
        {
            player_combat.endingCombat = true;
        }
    }
}
