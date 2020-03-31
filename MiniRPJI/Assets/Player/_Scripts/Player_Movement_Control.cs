/* Player_Control.cs
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
    [SerializeField] GameObject dashEffect;

    GameObject currentDashEffect;

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

        SetPlayerPosition(0);
    }

    void Update()
    {
        SimpleAnimatorControl();

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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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

                    if (!currentDashEffect)
                    {
                        currentDashEffect = CreateDashEffect(-270, 90, -90);
                    }
                    else
                    {
                        if (currentDashEffect.transform.parent == transform)
                        {
                            currentDashEffect.transform.position = new Vector3(transform.position.x, transform.position.y - .5f, 0f);
                        }
                        else // If we're here, the last currentDashEffect was moved to "Effects". So we can set it with a new currentDashEffect
                        {
                            currentDashEffect = CreateDashEffect(-270, 90, -90);
                        }
                    }
                    break;
                case 2:
                    myRb.velocity = new Vector2(0f, -1f) * dashSpeed;

                    if (!currentDashEffect)
                    {
                        currentDashEffect = CreateDashEffect(-90, 0, 0);
                    }
                    else
                    {
                        if (currentDashEffect.transform.parent == transform)
                        {
                            currentDashEffect.transform.position = new Vector3(transform.position.x, transform.position.y + .5f, 0f);
                        }
                        else // If we're here, the last currentDashEffect was moved to "Effects". So we can set it with a new currentDashEffect
                        {
                            currentDashEffect = CreateDashEffect(-90, 0, 0);
                        }
                    }
                    break;
                case 3:
                    myRb.velocity = new Vector2(1f, 0f) * dashSpeed;

                    if (!currentDashEffect)
                    {
                        currentDashEffect = CreateDashEffect(-180, 90, -90);                   
                    }
                    else
                    {
                        if (currentDashEffect.transform.parent == transform)
                        {
                            currentDashEffect.transform.position = new Vector3(transform.position.x - .5f, transform.position.y - .5f, 0f);
                        }
                        else // If we're here, the last currentDashEffect was moved to "Effects". So we can set it with a new currentDashEffect
                        {
                            currentDashEffect = CreateDashEffect(-180, 90, -90);
                        }
                    }
                    break;
                case 4:
                    myRb.velocity = new Vector2(-1f, 0f) * dashSpeed;

                    if (!currentDashEffect)
                    {
                        currentDashEffect = CreateDashEffect(-180, 270, -270);
                    }
                    else
                    {
                        if (currentDashEffect.transform.parent == transform)
                        {
                            currentDashEffect.transform.position = new Vector3(transform.position.x + .5f, transform.position.y - .5f, 0f);
                        }
                        else // If we're here, the last currentDashEffect was moved to "Effects". So we can set it with a new currentDashEffect
                        {
                            currentDashEffect = CreateDashEffect(-180, 270, -270);
                        }
                    }
                    break;
            }
        }
        else
        {
            currentDashEffect.transform.parent = GameObject.Find("Effects").transform;
            isDashing = false;
            dashTime = startDashTime;
        }
    }

    GameObject CreateDashEffect(float xRotation, float yRotation, float zRotation)
    {
        if (dashEffect)
        {
            GameObject tempDashEffect = Instantiate(dashEffect, transform.position, Quaternion.identity);

            tempDashEffect.transform.SetParent(transform);
            tempDashEffect.transform.localScale = Vector3.one;
            tempDashEffect.transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);

            Destroy(tempDashEffect, .6f);

            return tempDashEffect;
        }

        return null;
    }

    public void SetPlayerPosition(int teleportIndex)
    {
        // First of all, set velocity to 0
        if (myRb.velocity != Vector2.zero)
            myRb.velocity = Vector2.zero;

        Teleport_Position_Level[] levelTps = FindObjectsOfType<Teleport_Position_Level>();

        for (int i = 0; i < levelTps.Length; i++)
        {
            if (levelTps[i].levelFromBuildIndex == teleportIndex)
            {
                transform.position = levelTps[i].transform.position;
                return;
            }
        }

        Debug.LogWarning("There is no Teleport_Position_Level set for this teleportation index in this scene. MUST BE SET !");
    }
}
