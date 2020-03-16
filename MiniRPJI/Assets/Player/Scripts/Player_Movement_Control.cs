﻿/* Player_Control.cs
    Utilisé pour gérer les mouvements du joueur ainsi que les animations
*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player_Stats))]
public class Player_Movement_Control : MonoBehaviour
{
    [SerializeField] float speed = 4;
    [SerializeField] float dashSpeed = 40f;
    [SerializeField] float dashTime = .2f;
    [SerializeField] float energyNeedToDash = 8f;

    float startDashTime;
    bool isDashing = false;
    int dashDirection = 0; // 1 = dashUp, 2 = dashDown, 3 = dashRight, 4 = dashLeft

    Rigidbody2D myRb;
    Animator animator;
    

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        startDashTime = dashTime;

        if (FindObjectOfType<StartPositionLevel>())
        {
            transform.position = FindObjectOfType<StartPositionLevel>().transform.position;
        }
        else
        {
            Debug.LogWarning("There is no StartPositionLevel in this scenes. MUST BE SET !");
        }
    }

    void OnLevelWasLoaded(int level)
    {
        // TO make sure we acces to the right Player_Movement_Control script and not the one who'll be delete (if player already exist in the scene)
        // because top hierarchy gameobject is a singleton dontdestroyonload gameobject (Player_Stats)
        if (Player_Stats.stats_instance.gameObject == this.gameObject)
        {
            if (FindObjectOfType<StartPositionLevel>())
            {
                transform.position = FindObjectOfType<StartPositionLevel>().transform.position;
            }
            else
            {
                Debug.LogWarning("There is no StartPositionLevel in this scenes. MUST BE SET !");
            }
        }      
    }

    void Update()
    {
        SimpleAnimatorControl();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!isDashing)
                {
                    if (Player_Stats.stats_instance.playerEnergy.GetCurrentEnergyPoints() >= energyNeedToDash)
                    {
                        isDashing = true;
                        Player_Stats.stats_instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.stats_instance.playerEnergy.GetCurrentEnergyPoints() - energyNeedToDash);
                    }                    
                }             
            }
        }

        if (isDashing)
        {
            ProcessDash();
        }

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

    void SimplePlayerMovement()
    {
        if (isDashing)
            return;

        // Process normal movement
        if (Input.GetKey(KeyCode.Z))
        {
            myRb.velocity = new Vector2(0f, 1f) * speed;
            if (dashDirection != 1)
                dashDirection = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            myRb.velocity = new Vector2(0f, -1f) * speed;
            if (dashDirection != 2)
                dashDirection = 2;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            myRb.velocity = new Vector2(1f, 0f) * speed;
            if (dashDirection != 3)
                dashDirection = 3;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            myRb.velocity = new Vector2(-1f, 0f) * speed;
            if (dashDirection != 4)
                dashDirection = 4;
        }
        else
        {
            if (myRb.velocity != Vector2.zero)
            {
                myRb.velocity = Vector2.zero;
            }
        }
    }
    
    void ProcessDash()
    {
        if (dashTime > 0)
        {
            dashTime -= Time.deltaTime;

            switch (dashDirection)
            {
                case 1:
                    myRb.velocity = new Vector2(0f, 1f) * dashSpeed;
                    break;
                case 2:
                    myRb.velocity = new Vector2(0f, -1f) * dashSpeed;
                    break;
                case 3:
                    myRb.velocity = new Vector2(1f, 0f) * dashSpeed;
                    break;
                case 4:
                    myRb.velocity = new Vector2(-1f, 0f) * dashSpeed;
                    break;
            }
        }
        else
        {
            isDashing = false;
            dashTime = startDashTime;
        }
    }
}
