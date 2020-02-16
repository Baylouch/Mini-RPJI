/* Player_Control.cs
    Utilisé pour gérer les mouvements des NPC
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Stats_Control))]
[RequireComponent(typeof(AI_Health))]
public class AI_Movement_Control : MonoBehaviour
{
    [SerializeField] float nextMoveTimerMin = 1.5f;
    [SerializeField] float nextMoveTimerMax = 2f;
    [SerializeField] float movingDuration = 0.45f;
    float currentMovingTimer;
    bool currentlyMoving = false;
    int wayToGo; // To choose move direction
    int useMoveNumber; // To choose a predefined move set

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
    Stats_Control currentStats;
    AI_Health ai_health;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentStats = GetComponent<Stats_Control>();
        ai_health = GetComponent<AI_Health>();

        animatorVector = new Vector2();
        currentMovingTimer = Random.Range(nextMoveTimerMin, nextMoveTimerMax);
        wayToGo = 0;
        useMoveNumber = -1;

        startPos = transform.position; // Keep track from start pos
        backToStartPos = true; // At start we can suppose we're in start pos

        target = GameObject.FindGameObjectWithTag("Player").transform; // For now we set manually target as player, to change later
    }

    void Update()
    {
        // Think about set player as target when he's close with vector3.distance 
        if (ai_health.IsDead())
        {
            return;
        }

        SimpleAnimatorControl();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ai_health.IsDead())
        {
            return;
        }

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
                StopAllCoroutines(); // If we're already moving by moveset, stop coroutines.
                backToStartPos = false; // we go to undifined position so we need a way to go back to our startpos after that

                // Reset moveset params
                if (currentlyMoving)
                    currentlyMoving = false;

                ResetMovementParams();
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
            if (currentMovingTimer > 0f)
                currentMovingTimer -= Time.deltaTime;
            else if (!currentlyMoving)
            {
                StartCoroutine("ProcessMovementSet");
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
                    myRb.velocity = new Vector2(0f, 1f) * currentStats.GetSpeed();

                }
                else if (direction.y > 0.2f)
                {
                    myRb.velocity = new Vector2(0f, -1f) * currentStats.GetSpeed();
                }
            }

            if (direction.x > 0.3f || direction.x < -0.3f)
            {
                if (direction.x < 0.2f)
                {
                    myRb.velocity = new Vector2(1f, 0f) * currentStats.GetSpeed();
                }
                else if (direction.x > 0.2f)
                {
                    myRb.velocity = new Vector2(-1f, 0f) * currentStats.GetSpeed();
                }
            }
        }
        else
        {
            if (myRb.velocity != Vector2.zero)
                myRb.velocity = Vector2.zero;
        }
    }

    IEnumerator ProcessMovementSet()
    {
        currentlyMoving = true;

        // If it's the first time we're here. (To get more randomness on the start moveset, without all npc will use the same)
        if (useMoveNumber == -1)
        {
            useMoveNumber = Random.Range(0, 4);
        }

        switch (useMoveNumber)
        {
            case 0:
                MoveAroundX();
                break;
            case 1:
                MoveAroundSquare();
                break;
            case 2:
                MoveAroundLeftAndRight();
                break;
            case 3:
                MoveAroundUpDown();
                break;
            default:
                Debug.Log("No moveset processing.");
                break;
        }
        
        yield return new WaitForSeconds(movingDuration);

        currentMovingTimer = Random.Range(nextMoveTimerMin, nextMoveTimerMax);
        currentlyMoving = false;
        wayToGo++;

        if (myRb.velocity != Vector2.zero)
        {
            myRb.velocity = Vector2.zero;
        }

        StopCoroutine("ProcessMovementSet");
    }

    void ResetMovementParams()
    {
        wayToGo = -1;
        useMoveNumber = Random.Range(0, 4);
    }

    #region AI Moveset
    void MoveAroundX()
    {
        if (wayToGo == 0)
        {
            myRb.velocity = new Vector2(0f, 1f) * currentStats.GetSpeed();
        }
        else if (wayToGo == 1)
        {
            myRb.velocity = new Vector2(0f, -1f) * currentStats.GetSpeed();
        }
        else if (wayToGo == 2)
        {
            myRb.velocity = new Vector2(1f, 0f) * currentStats.GetSpeed();
        }
        else if (wayToGo >= 3)
        {
            myRb.velocity = new Vector2(-1f, 0f) * currentStats.GetSpeed();
            ResetMovementParams();
        }
    }

    void MoveAroundSquare()
    {
        if (wayToGo == 0)
        {
            myRb.velocity = new Vector2(0f, 1f) * currentStats.GetSpeed();
        }
        else if (wayToGo == 1)
        {            
            myRb.velocity = new Vector2(1f, 0f) * currentStats.GetSpeed();
        }
        else if (wayToGo == 2)
        {
            myRb.velocity = new Vector2(0f, -1f) * currentStats.GetSpeed();
        }
        else if (wayToGo >= 3)
        {
            myRb.velocity = new Vector2(-1f, 0f) * currentStats.GetSpeed();
            ResetMovementParams();
        }
    }

    void MoveAroundLeftAndRight()
    {
        if (wayToGo == 0)
        {
            myRb.velocity = new Vector2(-1f, 0f) * currentStats.GetSpeed();
        }
        else if (wayToGo == 1)
        {
            myRb.velocity = new Vector2(1f, 0f) * currentStats.GetSpeed();
        }
        else if (wayToGo == 2)
        {
            myRb.velocity = new Vector2(-1f, 0f) * currentStats.GetSpeed();
        }
        else if (wayToGo >= 3)
        {
            myRb.velocity = new Vector2(1f, 0f) * currentStats.GetSpeed();
            ResetMovementParams();
        }
    }

    void MoveAroundUpDown()
    {
        if (wayToGo == 0)
        {
            myRb.velocity = new Vector2(0f, -1f) * currentStats.GetSpeed();
        }
        else if (wayToGo == 1)
        {
            myRb.velocity = new Vector2(0f, 1f) * currentStats.GetSpeed();
        }
        else if (wayToGo == 2)
        {
            myRb.velocity = new Vector2(0f, -1f) * currentStats.GetSpeed();
        }
        else if (wayToGo >= 3)
        {
            myRb.velocity = new Vector2(0f, 1f) * currentStats.GetSpeed();
            ResetMovementParams();
        }
    }

    #endregion
}
