/* Combat_Control.cs
Utilisé pour gérer les combats
*/

using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AI_Health))]
[RequireComponent(typeof(AI_Stats))]
public class AI_Enemy_Combat : MonoBehaviour
{
    public float chasingDistance = 2.5f; // Used in AI_Movement_Control to know when start chasing
    [SerializeField] float damagedChasingDistance = 10f;
    float initialChasingDistance;

    // if projectile set, enemy can distance attack
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;
    public void SetFirePointRotation(float newRotation)
    {
        firePoint.rotation = Quaternion.Euler(new Vector3(0f, 0f, newRotation));
    }
    [SerializeField] private float shootRange = 3f;
    [SerializeField] private float timerBeforeShoot;
    float currentTimerBeforeShoot;

    // Basic timer for now
    [SerializeField] float timerBeforeAttack;
    float currentTimerBeforeAttack;

    // Range for attacking
    [SerializeField] float attackRange = 1f;
    [SerializeField] bool attacking = false;

    // Sounds
    [SerializeField] AudioClip[] attackSounds;
    [SerializeField] AudioClip projectileSound;

    Transform target;
    public Transform GetTarget()
    {
        return target;
    }

    Animator animator;

    AI_Health ai_health;
    AI_Stats ai_stats;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chasingDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ai_health = GetComponent<AI_Health>();
        ai_stats = GetComponent<AI_Stats>();

        currentTimerBeforeAttack = timerBeforeAttack;
        currentTimerBeforeShoot = timerBeforeShoot;
        initialChasingDistance = chasingDistance;
    }

    private void OnEnable()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
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
                        playerHealth.GetDamage(GetAttackDamage());
                    }

                    currentTimerBeforeAttack = timerBeforeAttack;
                }
            }
            else if (Vector3.Distance(transform.position, target.position) > attackRange && projectile) // else if we got projectile
            {
                if (Vector3.Distance(transform.position, target.position) <= shootRange)
                {
                    if (currentTimerBeforeShoot >= 0f && !attacking)
                    {
                        currentTimerBeforeShoot -= Time.deltaTime;
                    }
                    else
                    {
                        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, firePoint.up, Mathf.Infinity, LayerMask.GetMask("Player"));

                        if (hit2D != false)
                        {
                            if (hit2D.collider.gameObject.tag == "Player")
                            {
                                if (!attacking)
                                {
                                    UseRangedAttack();
                                    currentTimerBeforeShoot = timerBeforeShoot;
                                }
                            }
                        }
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

    // Security in addition of Update because sometimes enemies dont attack when they're in range
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
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
                    playerHealth.GetDamage(GetAttackDamage());
                }

                currentTimerBeforeAttack = timerBeforeAttack;
            }
        }
    }

    void UseAttack()
    {
        attacking = true;
        animator.SetBool("isAttacking", true);
        animator.SetBool("normalAttack", true);

        if (attackSounds.Length >= 1)
        {
            Sound_Manager.instance.PlaySound(attackSounds[Random.Range(0, attackSounds.Length)]);
        }
    }

    void UseRangedAttack()
    {
        attacking = true;
        animator.SetBool("isAttacking", true);
        animator.SetBool("rangedAttack", true);
    }

    int GetAttackDamage()
    {
        return Random.Range(ai_stats.GetDamageMin(), ai_stats.GetDamageMax());
    }

    int GetProjectileDamage()
    {
        return Random.Range(ai_stats.GetProjectileDamageMin(), ai_stats.GetProjectileDamageMax());
    }

    public void Shoot()
    {
        GameObject _projectile = Instantiate(projectile, firePoint.position, firePoint.rotation);
        Projectile proj = _projectile.GetComponent<Projectile>();

        if (proj != null)
            proj.projectileDamage = GetProjectileDamage();

        if (projectileSound)
        {
            Sound_Manager.instance.PlaySound(projectileSound);
        }
    }

    // To use in each event in animations attack and in Player_health when player died to reset AI_Combat_Control
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
}
