/* Player_Control.cs
    Utilisé pour gérer les mouvements du joueur ainsi que les animations
*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player_Movement_Control : MonoBehaviour
{
    [SerializeField] float speed = 4;
    [SerializeField] int maxStraffingStep = 2;

    Rigidbody2D myRb;
    Animator animator;
    Vector2 animatorVector; // To set X,Y values into animator

    bool straffing = false;
    float straffingResetTimer = 2f;
    float currentStraffingResetTimer;
    float straffingImpulsionMultiplier = 70f;
    int currentStraffingStep = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        animatorVector = new Vector2();

        if (FindObjectOfType<StartPositionLevel>())
        {
            transform.position = FindObjectOfType<StartPositionLevel>().transform.position;
        }
        else
        {
            Debug.LogWarning("There is no StartPositionLevel in this scenes. MUST BE SET !");
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        //transform.position = startPosition.position;
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
        if (straffing)
            return;

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
        // Process straffe movement
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (!straffing) // Say we are straffing now
                straffing = true;
                
            // Reset animator moving condition for not switch direction
            if (animator.GetBool("isMoving"))
            {
                animator.SetBool("isMoving", false);
            }

            // First of all reset movement
            if (myRb.velocity != Vector2.zero)
            {
                myRb.velocity = Vector2.zero;
            }

            if (currentStraffingStep >= maxStraffingStep) // if we did the max straffing step
            {
                return; // Dont continue !!!!
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                myRb.velocity = new Vector2(0f, 1f) * straffingImpulsionMultiplier;
                currentStraffingStep++;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                myRb.velocity = new Vector2(0f, -1f) * straffingImpulsionMultiplier;
                currentStraffingStep++;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                myRb.velocity = new Vector2(1f, 0f) * straffingImpulsionMultiplier;
                currentStraffingStep++;
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                myRb.velocity = new Vector2(-1f, 0f) * straffingImpulsionMultiplier;
                currentStraffingStep++;
            }

            if (currentStraffingStep >= maxStraffingStep) // if we did the max straffing step
            {
                // Then reset timer before straffing again
                currentStraffingResetTimer = straffingResetTimer;
            }

            return;
        }

        // If we're here we're not straffing anymore
        if (straffing)
            straffing = false;

        // Use timer for reset currentStraffingStep
        if (currentStraffingResetTimer > 0f)
        {
            currentStraffingResetTimer -= Time.deltaTime;
        }
        else
        {
            if (currentStraffingStep != 0)
                currentStraffingStep = 0;
        }



        // Process normal movement
        if (Input.GetKey(KeyCode.Z))
        {
            myRb.velocity = new Vector2(0f, 1f) * speed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            myRb.velocity = new Vector2(0f, -1f) * speed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            myRb.velocity = new Vector2(1f, 0f) * speed;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            myRb.velocity = new Vector2(-1f, 0f) * speed;
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
