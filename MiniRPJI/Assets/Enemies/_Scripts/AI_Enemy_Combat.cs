/* Combat_Control.cs
Utilisé pour gérer les combats
*/

using System.Collections;
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

    // Range for attacking
    [SerializeField] float attackRange = 1f;
    [SerializeField] float timerBeforeAttack;

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

    Coroutine attackCoroutine; // To know if an attack coroutine is already executing
    Coroutine rangedAttackCoroutine; // To know if a ranged attack coroutine is already executing

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

        CheckTargetDistance();
    }

    // Method to check target distance and do stuff relative
    void CheckTargetDistance()
    {
        if (target)
        {
            float targetDistance = Vector3.Distance(transform.position, target.position);

            // If player is in attack range
            if (targetDistance <= attackRange)
            {
                // If enemy was attacking in range, cancel that
                if (rangedAttackCoroutine != null)
                {
                    StopCoroutine(rangedAttackCoroutine);
                    rangedAttackCoroutine = null;
                }

                if (attackCoroutine == null)
                {
                    attackCoroutine = StartCoroutine(UseAttack());
                }
            }
            // Else If isn't in attack range, is in shoot range and AI got projectile
            else if (targetDistance > attackRange && targetDistance <= shootRange && projectile)
            {
                // If enemy was attacking cancel that.
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }

                // use timer to know when enemy is ready to shoot
                if (currentTimerBeforeShoot >= 0f)
                {
                    currentTimerBeforeShoot -= Time.deltaTime;
                }
                else
                {
                    // Check by raycasthit if enemy got player in view
                    RaycastHit2D hit2D = Physics2D.Raycast(transform.position, firePoint.up, Mathf.Infinity, LayerMask.GetMask("Player"));

                    if (hit2D != false)
                    {
                        if (hit2D.collider.gameObject.tag == "Player")
                        {
                            if (rangedAttackCoroutine == null)
                            {
                                rangedAttackCoroutine = StartCoroutine(UseRangedAttack(0));
                                currentTimerBeforeShoot = timerBeforeShoot;
                            }
                        }
                    }
                }
            }
            // Else if AI isn't in chasing distance cancel attacks
            else if (targetDistance > chasingDistance)
            {
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    TurnOffAnimatorParamAttack();
                }
                if (rangedAttackCoroutine != null)
                {
                    StopCoroutine(rangedAttackCoroutine);
                    TurnOffAnimatorParamAttack();
                }
            }
        }
    }

    IEnumerator UseAttack()
    {
        // Wait attack timer
        yield return new WaitForSeconds(timerBeforeAttack);

        // Then attack
        PlayerNormalAttackAnimation();

        // Look if we found Player_Health component to deal damage to the player
        Player_Health playerHealth = target.gameObject.GetComponent<Player_Health>();
        if (playerHealth)
        {
            playerHealth.TakeDamage(GetAttackDamage());
        }

        // Play attack sound
        if (attackSounds.Length >= 1)
        {
            Sound_Manager.instance.PlaySound(attackSounds[Random.Range(0, attackSounds.Length)]);
        }
    }

    IEnumerator UseRangedAttack(float delayBeforeShoot)
    {
        yield return new WaitForSeconds(delayBeforeShoot);

        PlayRangedAttackAnimation(); // Shoot method is called inside ranged attack animation

    }

    void PlayerNormalAttackAnimation()
    {
        animator.SetBool("isAttacking", true);
        animator.SetBool("normalAttack", true);
    }

    void PlayRangedAttackAnimation()
    {
        animator.SetBool("isAttacking", true);
        animator.SetBool("rangedAttack", true);
    }

    int GetAttackDamage()
    {
        bool tempCritCondition = ai_stats.GetCriticalRate() > Random.Range(0, 101);

        if (tempCritCondition)
        {
            float criticalAttack = Random.Range(ai_stats.GetDamageMin(), ai_stats.GetDamageMax());
            criticalAttack = criticalAttack + criticalAttack * 0.2f;
            return Mathf.RoundToInt(criticalAttack);
        }
        else
        {
            return Random.Range(ai_stats.GetDamageMin(), ai_stats.GetDamageMax());
        }
    }

    int GetProjectileDamage()
    {
        bool tempCritCondition = ai_stats.GetRangedCriticalRate() > Random.Range(0, 101);

        if (tempCritCondition)
        {
            float criticalAttack = Random.Range(ai_stats.GetProjectileDamageMin(), ai_stats.GetProjectileDamageMax());
            criticalAttack = criticalAttack + criticalAttack * 0.2f;
            return Mathf.RoundToInt(criticalAttack);
        }
        else
        {
            return Random.Range(ai_stats.GetProjectileDamageMin(), ai_stats.GetProjectileDamageMax());
        }
    }

    public void Shoot()
    {
        GameObject _projectile = Instantiate(projectile, firePoint.position, firePoint.rotation);
        Enemy_Projectile proj = _projectile.GetComponent<Enemy_Projectile>();

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

        attackCoroutine = null;
        rangedAttackCoroutine = null;
    }
}
