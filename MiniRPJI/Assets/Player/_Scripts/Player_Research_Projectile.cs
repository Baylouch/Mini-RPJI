/* Research_Projectile.cs
 * 
 * Script a attacher sur un projectile du joueur afin qu'il devienne a tête chercheuse.
 * 
 * Prend pour cible l'ennemi le plus proche lors de sa création.
 * 
 * TODO Créer des projectiles ennemis a tête chercheuse aussi ?
 * 
 * */

using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Seeker))] // To set path
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player_Projectile))]
public class Player_Research_Projectile : MonoBehaviour
{
    // The distance maximum the arrow will search a target
    [SerializeField] float researchingTargetDistance = 15f;

    [SerializeField] float nextWaypointDistance = 3f;
    [SerializeField] float stoppingDistance = 1f;

    Path path;
    Seeker seeker;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Rigidbody2D myRb;
    Transform target;
    Player_Projectile projectile;
    Animator animator;

    bool processingMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        projectile = GetComponent<Player_Projectile>();
        animator = GetComponent<Animator>();

        // To set the arrow direction at start (to avoid arrow spawn from top direction everytime because of animator component).
        if (FindObjectOfType<Player_Combat>())
        {
            float currentFirePointRotation = FindObjectOfType<Player_Combat>().GetFirePointRotationZ();

            if (currentFirePointRotation == 0) // Top direction
            {
                animator.SetTrigger("Top");
            }
            else if (currentFirePointRotation == 180) // Bottom direction
            {
                animator.SetTrigger("Bottom");
            }
            else if (currentFirePointRotation == 90) // Left direction
            {
                animator.SetTrigger("Left");
            }
            else if (currentFirePointRotation == 270) // Right direction
            {
                animator.SetTrigger("Right");
            }
        }

        SetTarget();
    }

    private void Update()
    {
        AnimatorUpdate();
    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    void SetTarget()
    {
        // Find the closer enemy and set it as target
        AI_Health[] aiHealth = FindObjectsOfType<AI_Health>(); // Search for all AI_Health in the scene to compare their distance.
        Transform closerTarget = null;

        for (int i = 0; i < aiHealth.Length - 1; i++)
        {
            // if closerTarget == null we're at the first iteration
            if (closerTarget == null)
            {
                // Check if aiHealth[i] position is greater than the researchingTargetDistance
                if (Vector3.Distance(transform.position, aiHealth[i].transform.position) > researchingTargetDistance)
                {
                    // If its the case, just continue to go to the next iteration. Testing aiHealth[i + 1] is useless because we'll test it on the next iteration.
                    // And if we pass trough this condition, aiHealth[i + 1] is out of researchingTargetDistance range so aiHealth[i] is closer and will be set as target.
                    continue;
                }

                if (Vector3.Distance(transform.position, aiHealth[i].transform.position) > Vector3.Distance(transform.position, aiHealth[i + 1].transform.position))
                {
                    closerTarget = aiHealth[i + 1].transform;
                }
                else
                {
                    closerTarget = aiHealth[i].transform;
                }

                continue; // We set closerTarget, go to the next iteration to compare with next aiHealth
            }

            // Check if aiHealth[i] isn't out of range, then compare it with closerTarget position to know if its closer or not.
            if (Vector3.Distance(transform.position, aiHealth[i].transform.position) <= researchingTargetDistance)
            {
                if (Vector3.Distance(transform.position, closerTarget.position) > Vector3.Distance(transform.position, aiHealth[i].transform.position))
                {
                    closerTarget = aiHealth[i].transform;
                }
            }
        }

        // closerTarget can't be null here, else we got a problem so destroy the script and let the arrow die itself because of destroy timer on it when instantiate from Player_Combat.
        if (closerTarget != null)
        {
            target = closerTarget;
        }
        else
        {
            // No target found, the arrow will simply be a normal arrow.
            Destroy(this);
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
                myRb.velocity = new Vector2(1f, 1f) * projectile.GetProjectileSpeed();
            }
            else if (direction.y < -0.2f && direction.x > 0.2f) // upper and left
            {
                myRb.velocity = new Vector2(-1f, 1f) * projectile.GetProjectileSpeed();
            }
            else if (direction.y > 0.2f && direction.x < -0.2f) // lower and right
            {
                myRb.velocity = new Vector2(1f, -1f) * projectile.GetProjectileSpeed();
            }
            else if (direction.y > 0.2f && direction.x > 0.2f) // lower and left
            {
                myRb.velocity = new Vector2(-1f, -1f) * projectile.GetProjectileSpeed();
            }
            else if (direction.y < -0.2f) // upper
            {
                myRb.velocity = new Vector2(0f, 1f) * projectile.GetProjectileSpeed();
            }
            else if (direction.y > 0.2f) // lower
            {
                myRb.velocity = new Vector2(0f, -1f) * projectile.GetProjectileSpeed();
            }
            else if (direction.x < -0.2f) // right
            {
                myRb.velocity = new Vector2(1f, 0f) * projectile.GetProjectileSpeed();
            }
            else if (direction.x > 0.2f) // left
            {
                myRb.velocity = new Vector2(-1f, 0f) * projectile.GetProjectileSpeed();
            }
        }

        if (curWaypointDistance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void UpdateTargetPath()
    {       
        if (target && seeker.IsDone())
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
            Debug.Log("La fleche n'as pas de cible.");
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
        if (animator.GetFloat("VectorX") != myRb.velocity.normalized.x)
        {
            animator.SetFloat("VectorX", myRb.velocity.normalized.x);
        }
        if (animator.GetFloat("VectorY") != myRb.velocity.normalized.y)
        {
            animator.SetFloat("VectorY", myRb.velocity.normalized.y);
        }
    }
}
