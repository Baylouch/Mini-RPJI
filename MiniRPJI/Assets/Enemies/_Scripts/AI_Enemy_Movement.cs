﻿/* AI_Enemy.cs
 * Gère les mouvements d'un ennemi via le system Pathfinding A*.
 * 
 * 
 * 
 * */


using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Seeker))] // To set path
[RequireComponent(typeof(AI_Enemy_Combat))] // To get target
[RequireComponent(typeof(AI_Stats))] // To get speed because of stats centralisation
public class AI_Enemy_Movement : MonoBehaviour
{
    [SerializeField] float nextWaypointDistance = 3f;
    [SerializeField] float stoppingDistance = 1f;

    Transform target;

    Path path;
    Seeker seeker;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Rigidbody2D myRb;
    Animator animator;
    Vector3 startPos; // Keep track where we from

    AI_Enemy_Combat ai_combat;
    AI_Moveset ai_moveset;
    AI_Stats ai_stats;

    Player_Combat player_combat; // To use for determine if player is in combat

    bool inChase = false;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        seeker = GetComponent<Seeker>();

        ai_combat = GetComponent<AI_Enemy_Combat>();
        ai_moveset = GetComponent<AI_Moveset>();
        ai_stats = GetComponent<AI_Stats>();

        startPos = transform.position;
        target = ai_combat.GetTarget();
    }

    void UpdateTargetPath()
    {
        if (seeker.IsDone())
            seeker.StartPath(myRb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Method for test if target is in chase range, if true create path to follow it.
    // else if was "inChase", create path to back to "startPos".
    void CreatePath()
    {
        float targetDistance = Vector2.Distance(myRb.position, target.position);

        if (targetDistance <= ai_combat.chasingDistance && targetDistance > stoppingDistance)
        {
            if (!inChase)
            {
                inChase = true;
                InvokeRepeating("UpdateTargetPath", 0f, .5f);
                if (ai_moveset)
                {
                    if (ai_moveset.GetAutoMove()) ai_moveset.SwitchAutoMove(false);
                }

                if (!player_combat)
                {
                    player_combat = FindObjectOfType<Player_Combat>();
                }

                if (player_combat)
                {
                    player_combat.isInCombat = true;
                    player_combat.endingCombat = false;
                    player_combat.ResetEndingCombatTimer();
                }
            }
        }
        else if (targetDistance > ai_combat.chasingDistance)
        {
            if (inChase)
            {
                inChase = false;
                CancelInvoke();

                if (seeker.IsDone())
                    seeker.StartPath(myRb.position, startPos, OnPathComplete);

                if (ai_moveset)
                {
                    if (!ai_moveset.GetAutoMove()) ai_moveset.SwitchAutoMove(true);
                }

                if (player_combat)
                {
                    player_combat.endingCombat = true;
                }
            }
        }
    }

    private void Update()
    {
        AnimatorUpdate();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CreatePath();

        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = (myRb.position - (Vector2)path.vectorPath[currentWaypoint]).normalized;

        float curWaypointDistance = Vector2.Distance(myRb.position, path.vectorPath[currentWaypoint]);

        // TODO Change the way it works. Use velocity addforce for instance. Because issue with animation when change direction
        // * For now i found a fix -> Exit time between each walk and idle animations. *
        if (curWaypointDistance < stoppingDistance)
        {
            myRb.velocity = Vector2.zero;
        }
        else 
        {

            if (direction.y < -0.2f && direction.x < -0.2f) // upper and right
            {
                myRb.velocity = new Vector2(1f, 1f) * ai_stats.GetSpeed();
            }
            else if (direction.y < -0.2f && direction.x > 0.2f) // upper and left
            {
                myRb.velocity = new Vector2(-1f, 1f) * ai_stats.GetSpeed();
            }
            else if (direction.y > 0.2f && direction.x < -0.2f) // lower and right
            {
                myRb.velocity = new Vector2(1f, -1f) * ai_stats.GetSpeed();
            }
            else if (direction.y > 0.2f && direction.x > 0.2f) // lower and left
            {
                myRb.velocity = new Vector2(-1f, -1f) * ai_stats.GetSpeed();
            }
            else if (direction.y < -0.2f) // upper
            {
                myRb.velocity = new Vector2(0f, 1f) * ai_stats.GetSpeed();
            }
            else if (direction.y > 0.2f) // lower
            {
                myRb.velocity = new Vector2(0f, -1f) * ai_stats.GetSpeed();
            }
            else if (direction.x < -0.2f) // right
            {
                myRb.velocity = new Vector2(1f, 0f) * ai_stats.GetSpeed();
            }
            else if (direction.x > 0.2f) // left
            {
                myRb.velocity = new Vector2(-1f, 0f) * ai_stats.GetSpeed();
            }
        }   
       
        if (curWaypointDistance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void AnimatorUpdate()
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
        else if (myRb.velocity == Vector2.zero && animator.GetBool("isMoving")) 
        {
            animator.SetBool("isMoving", false);           
        }
    }

    private void OnDestroy()
    {
        if (player_combat)
        {
            player_combat.endingCombat = true;
        }
    }
}
