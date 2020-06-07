/* PetMovement.cs :
 * 
 * Permet de gérer les déplacements du pet
 * 
 * 
 * */

using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Seeker))] // To set path
public class PetMovement : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    [SerializeField] float nextWaypointDistance = 3f;
    [SerializeField] float stoppingDistance = 1f;

    Path path;
    Seeker seeker;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Rigidbody2D myRb;
    Animator animator;

    Transform target;

    bool processingMovement = false;
    float startSpeed;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        seeker = GetComponent<Seeker>();

        target = Player_Stats.instance.transform;

        startSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorUpdate();

        if (target)
        {
            if (Vector3.Distance(myRb.position, target.position) > 6f)
            {
                if (speed == startSpeed)
                {
                    speed += 2;
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
        ProcessMovement();
    }

    void ProcessMovement()
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
        Vector3 posToGo = new Vector3(Random.Range(target.position.x - 2.5f, target.position.x + 2.5f), Random.Range(target.position.y - 2.5f, target.position.y + 2.5f), 0f);

        if (seeker.IsDone())
            seeker.StartPath(myRb.position, posToGo, OnPathComplete);
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
            Debug.Log("Le pet n'as pas de cible a suivre.");
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
}
