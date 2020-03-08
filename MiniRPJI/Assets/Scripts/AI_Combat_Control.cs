/* Combat_Control.cs
Utilisé pour gérer les combats
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AI_Health))]
public class AI_Combat_Control : MonoBehaviour
{
    public float chasingDistance = 2.5f; // Used in AI_Movement_Control to know when start chasing
    [SerializeField] float damagedChasingDistance = 10f;
    float initialChasingDistance;

    // attack
    [SerializeField] private int damageMin = 10;
    [SerializeField] private int damageMax = 15;

    // if projectile set, enemy can distance attack
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootRange = 3f;
    [SerializeField] private float timerBeforeShoot;
    float currentTimerBeforeShoot;

    // Basic timer for now
    [SerializeField] float timerBeforeAttack;
    float currentTimerBeforeAttack;

    // Range for attacking
    [SerializeField] float attackRange = 1f;

    [SerializeField] bool attacking = false;

    Animator animator;
    Transform target;
    AI_Health ai_health;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ai_health = GetComponent<AI_Health>();

        target = GameObject.FindGameObjectWithTag("Player").transform; // For now we set manually target as player, to change later

        currentTimerBeforeAttack = timerBeforeAttack;
        currentTimerBeforeShoot = timerBeforeShoot;
        initialChasingDistance = chasingDistance;

    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            // If player is in attack range
            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                if (currentTimerBeforeAttack >= 0f && !attacking) // Test timer before attack
                {
                    currentTimerBeforeAttack -= Time.deltaTime;
                }
                else if (!attacking) // Now attack
                {
                    UseAttack();
                    Player_Health playerHealth = target.gameObject.GetComponent<Player_Health>();
                    if (playerHealth)
                    {
                        int currAttack = (Random.Range(damageMin, damageMax));
                        playerHealth.GetDamage(currAttack);
                    }
                    currentTimerBeforeAttack = timerBeforeAttack;
                }
            }
            else if (projectile) // else if we got projectile
            {
                if (Vector3.Distance(transform.position, target.position) <= shootRange)
                {
                    if (currentTimerBeforeShoot >= 0f && !attacking)
                    {
                        currentTimerBeforeShoot -= Time.deltaTime;
                    }
                    else if (!attacking)
                    {
                        UseRangedAttack();
                        currentTimerBeforeShoot = timerBeforeShoot;
                    }
                }
            }
            
        }

        if (ai_health.damaged) // If we're damaged we want to upgrade chasing distance
        {
            if (chasingDistance != damagedChasingDistance)
            {
                chasingDistance = damagedChasingDistance;
            }
        }
        else
        {
            if (chasingDistance != initialChasingDistance)
            {
                chasingDistance = initialChasingDistance;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chasingDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }

    void UseAttack()
    {
        attacking = true;
        animator.SetBool("isAttacking", true);
        animator.SetBool("normalAttack", true);
    }

    void UseRangedAttack()
    {
        attacking = true;
        animator.SetBool("isAttacking", true);
        animator.SetBool("rangedAttack", true);
    }

    public void Shoot()
    {
        GameObject _projectile = Instantiate(projectile, firePoint.position, firePoint.rotation);
    }

    // To use in each event in animations attack
    public void TurnOffAnimatorParamAttack()
    {
        animator.SetBool("isAttacking", false);

        if (animator.GetBool("normalAttack"))
        {
            animator.SetBool("normalAttack", false);
        }

        if (animator.GetBool("rangedAttack"))
        {
            animator.SetBool("rangedAttack", false);
        }

        if (attacking)
        {
            attacking = false;
        }
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void SetFirePointRotation(float newRotation)
    {
        firePoint.rotation = Quaternion.Euler(new Vector3(0f, 0f, newRotation));
    }
}
