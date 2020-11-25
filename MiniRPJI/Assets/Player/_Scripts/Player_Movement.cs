/* Player_Control.cs
    Utilisé pour gérer les mouvements du joueur ainsi que les animations
*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player_Stats))]
public class Player_Movement : MonoBehaviour
{
    public bool canMove = true;
    public bool canDash = true;

    [HideInInspector] public bool moveFaster = false;
    [HideInInspector] public bool moveMoreFaster = false;

    [SerializeField] float dashSpeed = 40f;
    [SerializeField] float dashTime = .2f;
    [SerializeField] float energyNeedToDash = 5f; // In percent
    [SerializeField] GameObject dashEffect;

    GameObject currentDashEffect;

    float startDashTime;
    bool isDashing = false;
    int dashDirection = 0; // 1 = dashUp, 2 = dashDown, 3 = dashRight, 4 = dashLeft

    Player_Stats player_Stats;
    Rigidbody2D myRb;
    Animator animator;

    bool controlPlayerWhileHeCantMove = false; // Simply to be able to control player while we tell to the script he can't move. (Because of myRb.velocity = Vector2.zero security when we apply canMove = false in FixedUpdate).
    

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player_Stats = GetComponent<Player_Stats>();

        startDashTime = dashTime;

        SetPlayerPosition(0);
    }

    void Update()
    {
        AnimatorUpdate();

        if (!canMove)
            return;

        if (!canDash)
            return;

        if (Player_Shortcuts.GetShortCuts() == 0 || Player_Shortcuts.GetShortCuts() == 1)
        {
            // Process Mouse dashing
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (!isDashing)
                    {
                        if (GetPlayerEnergyAsPercentage() >= energyNeedToDash)
                        {
                            isDashing = true;
                            DecreasePlayerEnergy();
                        }
                    }
                }
            }
        }
        else
        {
            // Process Arrows dashing
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (!isDashing)
                    {
                        if (GetPlayerEnergyAsPercentage() >= energyNeedToDash)
                        {
                            isDashing = true;
                            DecreasePlayerEnergy();
                        }
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!canMove && !controlPlayerWhileHeCantMove)
        {
            if (myRb.velocity != Vector2.zero)
            {
                myRb.velocity = Vector2.zero;
            }

            controlPlayerWhileHeCantMove = true;

            return;
        }

        if (!canMove)
        {
            return;
        }

        if (canMove && controlPlayerWhileHeCantMove)
        {
            controlPlayerWhileHeCantMove = false;
        }

        if (isDashing)
        {
            ProcessDash();
        }

        if (!animator.GetBool("isAttacking"))
        {
            //if (Player_Shortcuts.GetShortCuts() == 0)
            //{
            //    // Process ZQSD movements
            //    SimplePlayerZQSDMovement();
            //}
            //else if (Player_Shortcuts.GetShortCuts() == 1)
            //{
            //    // Process WASD movements
            //    SimplePlayerWASDMovement();
            //}
            //else
            //{
            //    // Process Arrows movements
            //    SimplePlayerARROWSMovement();
            //}
        }
        else
        {
            if (myRb.velocity != Vector2.zero)
            {
                myRb.velocity = Vector2.zero;
            }
        }
    }

    void DecreasePlayerEnergy()
    {
        float energyToDecrease = Mathf.Round(Player_Stats.instance.playerEnergy.GetTotalEnergyPoints() * (energyNeedToDash / 100));

        Player_Stats.instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() - energyToDecrease);
    }

    // Return the current percentage of energy
    float GetPlayerEnergyAsPercentage()
    {
        float currentEnergyPercentage = (float)Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() / Player_Stats.instance.playerEnergy.GetTotalEnergyPoints();

        currentEnergyPercentage *= 100f;

        return currentEnergyPercentage;
    }

    void AnimatorUpdate()
    {
        if (myRb.velocity != Vector2.zero && !animator.GetBool("isMoving"))
        {
            animator.SetBool("isMoving", true);
        }
        else if (myRb.velocity == Vector2.zero && animator.GetBool("isMoving"))
        {
            animator.SetBool("isMoving", false);            
        }

        if (animator.GetFloat("VectorX") != myRb.velocity.x)
        {
            animator.SetFloat("VectorX", myRb.velocity.x);
        }
        if (animator.GetFloat("VectorY") != myRb.velocity.y)
        {
            animator.SetFloat("VectorY", myRb.velocity.y);
        }
    }

    void SimplePlayerZQSDMovement()
    {
        if (isDashing)
            return;

        float movementSpeed = player_Stats.GetSpeed();

        if (moveFaster)
            movementSpeed *= 1.5f;

        if (moveMoreFaster)
            movementSpeed *= 3f;

        // Process normal movement
        if (Input.GetKey(KeyCode.Z))
        {
            myRb.velocity = new Vector2(0f, 1f) * movementSpeed;
            if (dashDirection != 1)
                dashDirection = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            myRb.velocity = new Vector2(0f, -1f) * movementSpeed;
            if (dashDirection != 2)
                dashDirection = 2;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            myRb.velocity = new Vector2(1f, 0f) * movementSpeed;
            if (dashDirection != 3)
                dashDirection = 3;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            myRb.velocity = new Vector2(-1f, 0f) * movementSpeed;
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

    void SimplePlayerWASDMovement()
    {
        if (isDashing)
            return;

        float movementSpeed = player_Stats.GetSpeed();

        if (moveFaster)
            movementSpeed *= 1.5f;

        if (moveMoreFaster)
            movementSpeed *= 3f;

        // Process normal movement
        if (Input.GetKey(KeyCode.W))
        {
            myRb.velocity = new Vector2(0f, 1f) * movementSpeed;
            if (dashDirection != 1)
                dashDirection = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            myRb.velocity = new Vector2(0f, -1f) * movementSpeed;
            if (dashDirection != 2)
                dashDirection = 2;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            myRb.velocity = new Vector2(1f, 0f) * movementSpeed;
            if (dashDirection != 3)
                dashDirection = 3;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            myRb.velocity = new Vector2(-1f, 0f) * movementSpeed;
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

    void SimplePlayerARROWSMovement()
    {
        if (isDashing)
            return;

        float movementSpeed = player_Stats.GetSpeed();

        if (moveFaster)
            movementSpeed *= 1.5f;

        if (moveMoreFaster)
            movementSpeed *= 3f;

        // Process normal movement
        if (Input.GetKey(KeyCode.UpArrow))
        {
            myRb.velocity = new Vector2(0f, 1f) * movementSpeed;
            if (dashDirection != 1)
                dashDirection = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            myRb.velocity = new Vector2(0f, -1f) * movementSpeed;
            if (dashDirection != 2)
                dashDirection = 2;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            myRb.velocity = new Vector2(1f, 0f) * movementSpeed;
            if (dashDirection != 3)
                dashDirection = 3;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            myRb.velocity = new Vector2(-1f, 0f) * movementSpeed;
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
                default:
                    Debug.Log("You can't dash if dashing direction isn't set !");
                    break;
            }
        }
        else
        {
            if (GameObject.Find("Effects"))
            {
                if (currentDashEffect)
                    currentDashEffect.transform.parent = GameObject.Find("Effects").transform;
            }
            else
            {
                GameObject effectsGO = new GameObject("Effects");
                if (currentDashEffect)
                    currentDashEffect.transform.parent = GameObject.Find("Effects").transform;
            }

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

            // Then play sound before return
            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.playerDash);
            }

            return tempDashEffect;
        }

        return null;
    }

    public void SetPlayerPosition(int teleportIndex)
    {
        // First of all, set velocity to 0 to avoid player move when he's teleported
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

        if (levelTps[0] != null)
        {
            transform.position = levelTps[0].transform.position;
        }

        Debug.LogWarning("There is no Teleport_Position_Level set for this teleportation index in this scene. MUST BE SET !");
    }

    public void SetPlayerVelocity(Vector2 vel)
    {
        myRb.velocity = vel;
    }

    public void ProcessUpperMove()
    {
        if (isDashing)
            return;

        float movementSpeed = player_Stats.GetSpeed();

        if (moveFaster)
            movementSpeed *= 1.5f;

        if (moveMoreFaster)
            movementSpeed *= 3f;

        myRb.velocity = new Vector2(0f, 1f) * movementSpeed;
        if (dashDirection != 1)
            dashDirection = 1;
    }

    public void ProcessDownMove()
    {
        if (isDashing)
            return;

        float movementSpeed = player_Stats.GetSpeed();

        if (moveFaster)
            movementSpeed *= 1.5f;

        if (moveMoreFaster)
            movementSpeed *= 3f;

        myRb.velocity = new Vector2(0f, -1f) * movementSpeed;
        if (dashDirection != 2)
            dashDirection = 2;
    }

    public void ProcessLeftMove()
    {
        if (isDashing)
            return;

        float movementSpeed = player_Stats.GetSpeed();

        if (moveFaster)
            movementSpeed *= 1.5f;

        if (moveMoreFaster)
            movementSpeed *= 3f;

        myRb.velocity = new Vector2(-1f, 0f) * movementSpeed;
        if (dashDirection != 4)
            dashDirection = 4;
    }

    public void ProcessRightMove()
    {
        if (isDashing)
            return;

        float movementSpeed = player_Stats.GetSpeed();

        if (moveFaster)
            movementSpeed *= 1.5f;

        if (moveMoreFaster)
            movementSpeed *= 3f;

        myRb.velocity = new Vector2(1f, 0f) * movementSpeed;
        if (dashDirection != 3)
            dashDirection = 3;
    }

    public void ProcessDashing()
    {
        if (!isDashing)
        {
            if (GetPlayerEnergyAsPercentage() >= energyNeedToDash)
            {
                isDashing = true;
                DecreasePlayerEnergy();
            }
        }
    }

    public void StopMove()
    {
        if (myRb.velocity != Vector2.zero)
        {
            myRb.velocity = Vector2.zero;
        }
    }
}
