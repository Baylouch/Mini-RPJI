using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Enemy_Projectile))]
[RequireComponent(typeof(Seeker))] // To set path
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_Projectile_Movement : MonoBehaviour
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
    Enemy_Projectile projectile;
    Animator animator;

    bool processingMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Animator>())
            animator = GetComponent<Animator>();

        myRb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        projectile = GetComponent<Enemy_Projectile>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetTarget()
    {

    }
}
