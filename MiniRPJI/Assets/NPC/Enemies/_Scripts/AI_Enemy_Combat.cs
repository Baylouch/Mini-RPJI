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
        if (firePoint.rotation.z != newRotation) // Not must, but can avoid sometimes to repeat rotation (not every time because of Quaternion, value is weird)
        {
            firePoint.rotation = Quaternion.Euler(new Vector3(0f, 0f, newRotation));
        }
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

    Player_Combat player_combat; // To use for determine if player is in combat (AI_Enemy_Movement.cs got this too)

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

        currentTimerBeforeShoot = 0f; 
        initialChasingDistance = chasingDistance;
    }

    private void OnEnable()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        attackCoroutine = null;
        rangedAttackCoroutine = null;
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

        if (target == null)
        {
            if (Player_Stats.instance)
            {
                target = Player_Stats.instance.transform;
            }

            return;
        }

        CheckForPlayerDecoy();

        CheckTargetDistance();
    }

    // Method to check target distance and do stuff relative
    void CheckTargetDistance()
    {
        if (target)
        {
            float targetDistance = Vector3.Distance(transform.position, target.position);

            // A security for player dont get damage when he run out attack range
            if (targetDistance > attackRange + 1.5f)
            {
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    TurnOffAnimatorParamAttack();
                }
            }

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
            else if (targetDistance > attackRange && targetDistance <= shootRange)
            {
                // If enemy was attacking cancel that.
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }

                // If enemy got a projectile
                if (projectile)
                {
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

        // Check if we found Player_Health component to deal damage to the player
        Player_Health playerHealth = target.gameObject.GetComponent<Player_Health>();
        if (playerHealth)
        {
            playerHealth.TakeDamage(GetAttackDamage(), true);
        }

        // Or maybe a Decoy_Health component
        Decoy_Health decoyHealth = target.gameObject.GetComponent<Decoy_Health>();
        if (decoyHealth)
        {
            decoyHealth.TakeDamage(GetAttackDamage(), true);
        }

        // Play attack sound
        if (attackSounds.Length >= 1)
        {
            Sound_Manager.instance.PlaySound(attackSounds[Random.Range(0, attackSounds.Length)]);
        }

        if (!player_combat)
        {
            player_combat = FindObjectOfType<Player_Combat>();
        }

        if (player_combat && player_combat.endingCombat == true)
        {
            player_combat.isInCombat = true;
            player_combat.endingCombat = false;
            player_combat.ResetEndingCombatTimer();
        }
    }

    IEnumerator UseRangedAttack(float delayBeforeShoot)
    {
        yield return new WaitForSeconds(delayBeforeShoot);

        PlayRangedAttackAnimation(); // Shoot method is called inside ranged attack animation

        if (!player_combat)
        {
            player_combat = FindObjectOfType<Player_Combat>();
        }

        if (player_combat && player_combat.endingCombat == true)
        {
            player_combat.isInCombat = true;
            player_combat.endingCombat = false;
            player_combat.ResetEndingCombatTimer();
        }
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

    // Method to check when player invoke a decoy. AI will target decoy until there is one.
    void CheckForPlayerDecoy()
    {
        if (FindObjectOfType<Ability_Decoy>())
        {
            float decoyDistance = Vector3.Distance(transform.position, FindObjectOfType<Ability_Decoy>().transform.position);

            if (decoyDistance < chasingDistance)
            {
                target = FindObjectOfType<Ability_Decoy>().transform;
            }
            else
            {
                if (GameObject.FindGameObjectWithTag("Player"))
                {
                    if (target == null || target != GameObject.FindGameObjectWithTag("Player").transform)
                    {
                        target = GameObject.FindGameObjectWithTag("Player").transform;
                    }
                }
            }
        }        
    }

    public void Shoot()
    {
        GameObject _projectile = Instantiate(projectile, firePoint.position, firePoint.rotation);

        Enemy_Projectile proj = _projectile.GetComponent<Enemy_Projectile>();

        if (proj != null)
        {
            proj.projectileDamage = GetProjectileDamage();
        }
        else
        {
            if (_projectile.transform.childCount > 0)
            {
                for (int i = 0; i < _projectile.transform.childCount; i++)
                {
                    if (_projectile.transform.GetChild(i).GetComponent<Enemy_Projectile>())
                    {
                        _projectile.transform.GetChild(i).GetComponent<Enemy_Projectile>().projectileDamage = GetProjectileDamage();
                    }
                }
            }
        }

        if (projectileSound && Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(projectileSound);
        }
    }

    // To use in each event in animations attack and in Player_health when player died to reset AI_Combat_Control
    public void TurnOffAnimatorParamAttack()
    {
        if (!animator)
        {
            attackCoroutine = null;
            rangedAttackCoroutine = null;

            return;
        }

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
