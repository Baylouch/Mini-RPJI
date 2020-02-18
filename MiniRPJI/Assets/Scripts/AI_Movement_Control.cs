/* Player_Control.cs
    Utilisé pour gérer les mouvements des NPC
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AI_Moveset))]
public class AI_Movement_Control : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    [SerializeField] float followingDistance = 2f; // Distance before start follow target
    [SerializeField] float stopFollowingOffset = 0.2f;
    float backToStartPosOffset = .4f; // When npc go back to his startPos, we need a little offset
    bool followingTarget = false; // Are we currently following ?
    Vector3 startPos; // Keep track where we from
    bool backToStartPos = true;
    Transform target;

    Rigidbody2D myRb;
    Animator animator;
    Vector2 animatorVector; // To set X,Y values into animator
    AI_Moveset ai_moveset;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ai_moveset = GetComponent<AI_Moveset>();

        animatorVector = new Vector2();

        startPos = transform.position; // Keep track from start pos
        backToStartPos = true; // At start we can suppose we're in start pos

        target = GameObject.FindGameObjectWithTag("Player").transform; // For now we set manually target as player, to change later
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, followingDistance);
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
        if (target) // If there is a target
        {
            if (Vector3.Distance(transform.position, target.position) <= followingDistance && !followingTarget) // if our distance from target is lesser than following distance and we're not already following
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
            }
            else if (Vector3.Distance(transform.position, target.position) > followingDistance && followingTarget) // if our distance is too far from target and we're following
            {
                followingTarget = false;
            }

            if (followingTarget) // If we're following target
            {
                ProcessFollowMovement(target.position);

                return;
            }
        }

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

    // Method for follow a target
    void ProcessFollowMovement(Vector3 targetPos)
    {
        Vector3 direction = transform.position - targetPos;
        direction.z = 0;

        if (Vector3.Distance(transform.position, targetPos) > stopFollowingOffset)
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
}
