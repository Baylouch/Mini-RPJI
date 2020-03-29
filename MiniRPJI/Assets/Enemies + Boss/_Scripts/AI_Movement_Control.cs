/* Player_Control.cs
    Utilisé pour gérer les mouvements des NPC
*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AI_Moveset))]
[RequireComponent(typeof(AI_Combat_Control))] // To know target and combat relative stuff
public class AI_Movement_Control : MonoBehaviour
{
    public float speed = 2f;

    [SerializeField] float stopFollowingOffset = 0.2f;
    float backToStartPosOffset = 2.5f; // When npc go back to his startPos, we need a little offset must be more than stopFollowingOffset
    bool followingTarget = false; // Are we currently following ?
    Vector3 startPos; // Keep track where we from
    bool backToStartPos = true;

    // Navigation AI (for avoid stuck stuff) really basic
    Vector3 trackCurrentPos; // To get a track of AI position when following and change it if he stay too long.
    float trackPosTimer = 1f; // To refresh currentPos
    float trackCurrentPosTime; // To put Time.time in
    float stuckOffset = 0.5f; // Because its never EXACLTY the same pos, we need a little offset
    bool isStuck;
    float unstuckTimer = 2f;
    int unstuckDirection = -1; // 1 = Top, 2 = Bottom, 3 = Right, 4 = Left
    Vector3 beforeStuckDirection; // To put in the last knowed position AI wanted to go

    Rigidbody2D myRb;
    Animator animator;

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
        if (isStuck)
        {
            UnstuckAI();
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

    void SimpleAnimatorControl()
    {
        if (animator.GetFloat("VectorX") != myRb.velocity.x)
        {
            animator.SetFloat("VectorX", myRb.velocity.x);
        }
        if (animator.GetFloat("VectorY") != myRb.velocity.y)
        {
            animator.SetFloat("VectorY", myRb.velocity.y);
        }

        if (myRb.velocity != Vector2.zero && !animator.GetBool("isMoving"))
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
        backToStartPos = false; // we go to undefined position so we need a way to go back to our startpos after that
        trackCurrentPosTime = Time.time;

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
    // See to upgrade : because movement is a bit weird (to see in game)
    void ProcessFollowMovement(Vector3 targetPos)
    {
        Vector3 direction = transform.position - targetPos;
        direction.z = 0;

        if (Vector3.Distance(transform.position, targetPos) > stopFollowingOffset) // To limit AI movement when he's close to the player
        {
            // TODO FINISH IT : AI must avoid obstacles because it knows when it's blocked.
            // For now : basic avoiding
            if (Time.time > trackCurrentPosTime + trackPosTimer) // Track pos timer
            {
                if (Vector3.Distance(transform.position, trackCurrentPos) <= stuckOffset) // If distance between current position and tracked one is less than stuckOffset, AI is stuck
                {
                    isStuck = true;
                    beforeStuckDirection = direction; // Now we know where AI tryed to go before be stuck, so the direction blocked.
                    return;
                }
                else // Else just reset tracked pos and tracked timer
                {
                    trackCurrentPos = transform.position;
                    trackCurrentPosTime = Time.time;
                    return;
                }
            }

            if (direction.y < -0.2f && direction.x < -0.2f) // upper and right
            {
                myRb.velocity = new Vector2(1f, 1f) * speed;
            }
            else if (direction.y < -0.2f && direction.x > 0.2f) // upper and left
            {
                myRb.velocity = new Vector2(-1f, 1f) * speed;
            }
            else if (direction.y > 0.2f && direction.x < -0.2f) // lower and right
            {
                myRb.velocity = new Vector2(1f, -1f) * speed;
            }
            else if (direction.y > 0.2f && direction.x > 0.2f) // lower and left
            {
                myRb.velocity = new Vector2(-1f, -1f) * speed;
            }
            else if (direction.y < -0.2f) // Direction is upper
            {
                myRb.velocity = new Vector2(0f, 1f) * speed;
            }
            else if (direction.y > 0.2f) // lower
            {
                myRb.velocity = new Vector2(0f, -1f) * speed;
            }

            else if (direction.x < -0.2f) // right
            {
                myRb.velocity = new Vector2(1f, 0f) * speed;
            }
            else if (direction.x > 0.2f) // left
            {
                myRb.velocity = new Vector2(-1f, 0f) * speed;
            }
        }
        else
        {
            if (myRb.velocity != Vector2.zero)
                myRb.velocity = Vector2.zero;
        }
    }


    // What we want is : if player is lower than AI, and AI is stuck trying to go to the bottom.
    // its avoid going left or right if there is no collider close on these sides for exemple.
    void UnstuckAI()
    {
        if (unstuckDirection <= 0)
        {
            // Tester les direction dans l'ordre.
            // Dabord si il voulait aller en haut a droite, ensuite en haut a gauche, ensuite en bas a droite, en bas a gauche, en haut, en bas, a droite, a gauche...

            if (beforeStuckDirection.y < -0.2f && beforeStuckDirection.x < -0.2f) // upper and right
            {
                // AI tried to go upper and right, so if its stuck in these directions, try to the bottom, if you cant go to the left
                if (ProcessUnstuckRaycast(-transform.up))
                {
                    unstuckDirection = 2;
                    return;
                }

                if (ProcessUnstuckRaycast(-transform.right))
                {
                    unstuckDirection = 4;
                    return;
                }
            }
            else if (beforeStuckDirection.y < -0.2f && beforeStuckDirection.x > 0.2f) // upper and left
            {
                if (ProcessUnstuckRaycast(-transform.up))
                {
                    unstuckDirection = 2;
                    return;
                }

                if (ProcessUnstuckRaycast(transform.right))
                {
                    unstuckDirection = 3;
                    return;
                }
            }
            else if (beforeStuckDirection.y > 0.2f && beforeStuckDirection.x < -0.2f) // lower and right
            {
                if (ProcessUnstuckRaycast(transform.up))
                {
                    unstuckDirection = 1;
                    return;
                }

                if (ProcessUnstuckRaycast(-transform.right))
                {
                    unstuckDirection = 4;
                    return;
                }
            }
            else if (beforeStuckDirection.y > 0.2f && beforeStuckDirection.x > 0.2f) // lower and left
            {
                if (ProcessUnstuckRaycast(transform.up))
                {
                    unstuckDirection = 1;
                    return;
                }


                if (ProcessUnstuckRaycast(transform.right))
                {
                    unstuckDirection = 3;
                    return;
                }
            }
            else if (beforeStuckDirection.y < -0.2f) // Direction is upper
            {
                if (ProcessUnstuckRaycast(transform.right))
                {
                    unstuckDirection = 3;
                    return;
                }

                if (ProcessUnstuckRaycast(-transform.right))
                {
                    unstuckDirection = 4;
                    return;
                }
            }
            else if (beforeStuckDirection.y > 0.2f) // lower
            {
                if (ProcessUnstuckRaycast(transform.right))
                {
                    unstuckDirection = 3;
                    return;
                }

                if (ProcessUnstuckRaycast(-transform.right))
                {
                    unstuckDirection = 4;
                    return;
                }
            }
            else if (beforeStuckDirection.x < -0.2f) // right
            {
                if (ProcessUnstuckRaycast(transform.up))
                {
                    unstuckDirection = 1;
                    return;
                }

                if (ProcessUnstuckRaycast(-transform.up))
                {
                    unstuckDirection = 2;
                    return;
                }
            }
            else if (beforeStuckDirection.x > 0.2f) // left
            {
                if (ProcessUnstuckRaycast(transform.up))
                {
                    unstuckDirection = 1;
                    return;
                }

                if (ProcessUnstuckRaycast(-transform.up))
                {
                    unstuckDirection = 2;
                    return;
                }
            }
        }

        if (unstuckDirection > 0)
        {
            if (unstuckTimer >= 0)
            {
                unstuckTimer -= Time.deltaTime;

                switch(unstuckDirection)
                {
                    case 1:
                        myRb.velocity = new Vector2(0f, 1f) * speed;
                        break;
                    case 2:
                        myRb.velocity = new Vector2(0f, -1f) * speed;
                        break;
                    case 3:
                        myRb.velocity = new Vector2(1f, 0f) * speed;
                        break;
                    case 4:
                        myRb.velocity = new Vector2(-1f, 0f) * speed;
                        break;
                }
            }
            else
            {
                isStuck = false;
                unstuckTimer = 2f;
                unstuckDirection = -1;
                beforeStuckDirection = Vector3.zero;
            }
        }
    }

    // method to process a raycast when AI is stuck.
    bool ProcessUnstuckRaycast(Vector3 rayDirection)
    {
        RaycastHit2D hit2D; // We create a new RaycastHit2D.
       
        hit2D = Physics2D.Raycast(transform.position, rayDirection, Mathf.Infinity, LayerMask.GetMask("Default"));
        if (hit2D)
        {
            if (Vector3.Distance(transform.position, hit2D.collider.transform.position) > 5f)
            {
                return true;
            }
        }

        return false;
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
