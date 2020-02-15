/* Player_Control.cs
    Utilisé pour gérer les mouvements du joueur ainsi que les animations
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Stats_Control))]
public class AI_Movement_Control : MonoBehaviour
{
    [SerializeField] float nextMoveTimer = 3f;
    [SerializeField] float movingDuration = 4f;
    float currentMovingTimer;
    bool currentlyMoving = false;
    int wayToGo;
    int useMoveNumber;

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
        currentMovingTimer = nextMoveTimer;
        wayToGo = 0;
        int useMoveNumber = Random.Range(0, 4);
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
        if (currentMovingTimer > 0f)
            currentMovingTimer -= Time.deltaTime;
        else if (!currentlyMoving)
        {         
            StartCoroutine("ProcessMovement");
        }
    }

    IEnumerator ProcessMovement()
    {
        currentlyMoving = true;

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
                Debug.Log("No move processing.");
                break;
        }
        
        yield return new WaitForSeconds(movingDuration);

        currentMovingTimer = nextMoveTimer;
        currentlyMoving = false;
        wayToGo++;

        if (myRb.velocity != Vector2.zero)
        {
            myRb.velocity = Vector2.zero;
        }

        StopCoroutine("ProcessMovement");
    }

    void ResetMovementParams()
    {
        wayToGo = -1;
        useMoveNumber = Random.Range(0, 4);
    }

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
            myRb.velocity = new Vector2(-1f, -0f) * currentStats.GetSpeed();
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
}
