using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Seeker))] // To set path
public class PetUnlockerMovement : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float additionalSpeed = 3f; // When player is close to the unlocker, it'll increase its speed.

    [SerializeField] float nextWaypointDistance = 3f;
    [SerializeField] float stoppingDistance = 1f;

    Path path;
    Seeker seeker;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Rigidbody2D myRb;
    Animator animator;

    GameObject targetGO;
    Transform target;
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    public Transform GetTarget()
    {
        return target;
    }

    Transform player; 

    bool processingMovement = false;
    float startSpeed;

    private void OnDestroy()
    {
        if (targetGO != null)
            Destroy(targetGO);
    }

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        seeker = GetComponent<Seeker>();

        if (Player_Stats.instance)
            player = Player_Stats.instance.transform;

        startSpeed = speed;

        targetGO = new GameObject("PetUnlockerTarget");

        if (GameObject.Find("Pets"))
            targetGO.transform.parent = GameObject.Find("Pets").transform;

        target = targetGO.transform;

        SetNewTargetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorUpdate();

        if (target)
        {
            if (player && Vector3.Distance(myRb.position, player.position) < 6f)
            {
                if (speed == startSpeed)
                {
                    speed += additionalSpeed;
                }
            }
            else
            {
                if (speed != startSpeed)
                {
                    speed = startSpeed;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (player)
        {
            if (Vector3.Distance(myRb.position, player.position) < 10)
            {
                // If we're here player is close, so if pet stays at the same position, its means its stuck. 

                ProcessMovement();
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

    void ProcessMovement()
    {
        CreatePath();

        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;

            SetNewTargetPosition();

            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = (myRb.position - (Vector2)path.vectorPath[currentWaypoint]).normalized;

        float curWaypointDistance = Vector2.Distance(myRb.position, path.vectorPath[currentWaypoint]);

        if (curWaypointDistance < stoppingDistance)
        {
            myRb.velocity = Vector2.zero;
        }
        else
        {
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
            else if (direction.y < -0.2f) // upper
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

        if (curWaypointDistance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void UpdateTargetPath()
    {
        if (!target)
            return;

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

    void CreatePath()
    {
        if (target == null)
        {
            return;
        }

        float targetDistance = Vector2.Distance(myRb.position, target.position);

        if (targetDistance > stoppingDistance)
        {
            if (!processingMovement)
            {
                processingMovement = true;

                InvokeRepeating("UpdateTargetPath", 0f, .5f);
            }
        }
        else
        {
            if (processingMovement)
            {
                processingMovement = false;

                CancelInvoke();
            }
        }
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

    void SetNewTargetPosition()
    {
        // If we reach end of path we must create a new position. Not the same logic as AI_Enemy_Movement or PetMovement because here,
        // we want to pet unlocker to flies when player is close, so we create an empty around a defined position to set it as newTarget.
        // Each time pet unlocker reach the end of the path, target is set to null again.


        Vector3 playerDistance = transform.position - player.position;

        // ******************* MEMO *************************
        // playerDistance.x > 0 means player is left from the pet
        // playerDistance.x < 0 means player is right from the pet
        // playerDistance.y > 0 means player is bottom from the pet
        // playerDistance.y < 0 means player is upper from the pet

        // TODO Create a solution for when pet reach the end of the map.
        // 1 -> Test its position and if its the same after 1sec, determine a new pts to the inverse from the last known position

        Vector3 newPos = Vector3.zero;

        // We get the absolute value of x and y to know from which position player is closer
        if (Mathf.Abs(playerDistance.x) < Mathf.Abs(playerDistance.y))
        {
            if (playerDistance.x > 0)
            {
                // Go to the right
                newPos = new Vector3(Random.Range(transform.position.x + 3, transform.position.x + 7),
                                     Random.Range(transform.position.y + 3, transform.position.y - 3),
                                     0f);

            }
            else
            {
                // Go to the left
                newPos = new Vector3(Random.Range(transform.position.x - 3, transform.position.x - 7),
                                     Random.Range(transform.position.y + 3, transform.position.y - 3),
                                     0f);
            }
        }
        else
        {
            if (playerDistance.y > 0)
            {
                // Go upper
                newPos = new Vector3(Random.Range(transform.position.x - 3, transform.position.x + 3),
                                     Random.Range(transform.position.y + 3, transform.position.y + 7),
                                     0f);
            }
            else
            {
                // Go bottom
                newPos = new Vector3(Random.Range(transform.position.x - 3, transform.position.x + 3),
                                     Random.Range(transform.position.y - 3, transform.position.y - 7),
                                     0f);
            }
        }

        target.transform.position = newPos;
    }
}
