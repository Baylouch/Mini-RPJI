using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Enemy_Projectile))]
[RequireComponent(typeof(Seeker))] // To set path
[RequireComponent(typeof(FunnelModifier))]
[RequireComponent(typeof(SimpleSmoothModifier))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_Research_Projectile : MonoBehaviour
{
    [HideInInspector]
    public AI_Enemy_Combat theEnemyWhoLaunchTheProjectile;

    [SerializeField] float nextWaypointDistance = .2f;
    [SerializeField] float stoppingDistance = .1f;

    Path path;
    Seeker seeker;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Rigidbody2D myRb;
    Transform target;
    Enemy_Projectile projectile;

    bool processingMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        projectile = GetComponent<Enemy_Projectile>();

        if (GetComponent<MakeProjectileRotate>())
            Destroy(GetComponent<MakeProjectileRotate>());

        SetTarget();
    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    void SetTarget()
    {
        if (Player_Stats.instance)
        {
            target = Player_Stats.instance.transform;
        }
        else
        {
            Debug.Log("No target found from Enemy_Research_Projectile.");
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
                if (myRb.velocity != new Vector2(1f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, 1f) * projectile.GetProjectileSpeed();

                if (transform.rotation.eulerAngles.z != 0f)
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (direction.y < -0.2f && direction.x > 0.2f) // upper and left
            {
                if (myRb.velocity != new Vector2(-1f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, 1f) * projectile.GetProjectileSpeed();

                if (transform.rotation.eulerAngles.z != 0f)
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (direction.y > 0.2f && direction.x < -0.2f) // lower and right
            {
                if (myRb.velocity != new Vector2(1f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, -1f) * projectile.GetProjectileSpeed();

                if (transform.rotation.eulerAngles.z != 180f)
                    transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            }
            else if (direction.y > 0.2f && direction.x > 0.2f) // lower and left
            {
                if (myRb.velocity != new Vector2(-1f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, -1f) * projectile.GetProjectileSpeed();

                if (transform.rotation.eulerAngles.z != 180f)
                    transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            }
            else if (direction.y < -0.2f) // upper
            {
                if (myRb.velocity != new Vector2(0f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(0f, 1f) * projectile.GetProjectileSpeed();

                if (transform.rotation.eulerAngles.z != 0f)
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (direction.y > 0.2f) // lower
            {
                if (myRb.velocity != new Vector2(0f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(0f, -1f) * projectile.GetProjectileSpeed();

                if (transform.rotation.eulerAngles.z != 180f)
                    transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            }
            else if (direction.x < -0.2f) // right
            {
                if (myRb.velocity != new Vector2(1f, 0f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, 0f) * projectile.GetProjectileSpeed();

                if (transform.rotation.eulerAngles.z != -90f)
                    transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else if (direction.x > 0.2f) // left
            {
                if (myRb.velocity != new Vector2(-1f, 0f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, 0f) * projectile.GetProjectileSpeed();

                if (transform.rotation.eulerAngles.z != 90)
                    transform.rotation = Quaternion.Euler(0f, 0f, 90f);
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
            SetTarget();
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
}
