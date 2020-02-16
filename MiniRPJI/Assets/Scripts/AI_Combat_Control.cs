/* Combat_Control.cs
Utilisé pour gérer les combats
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Stats_Control))]
[RequireComponent(typeof(Collider2D))]
public class AI_Combat_Control : MonoBehaviour
{
    // Basic timer for now
    [SerializeField] float timerBeforeAttack;
    float currentTimerBeforeAttack;

    // Range for attacking
    [SerializeField] float attackRange = 1f;

    [SerializeField] bool attacking = false;

    Animator animator;
    Stats_Control currentStats;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentStats = GetComponent<Stats_Control>();
        currentTimerBeforeAttack = timerBeforeAttack;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            // If player is in attack range
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                if (currentTimerBeforeAttack >= 0f && !attacking) // Test timer before attack
                {
                    currentTimerBeforeAttack -= Time.deltaTime;
                }
                else if (!attacking) // Now attack
                {
                    UseAttack();
                    Player_Health playerHealth = player.gameObject.GetComponent<Player_Health>();
                    if (playerHealth)
                    {
                        playerHealth.GetDamage(currentStats.GetCurrentAttackDamage());
                    }
                    currentTimerBeforeAttack = timerBeforeAttack;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void UseAttack()
    {
        attacking = true;
        animator.SetBool("isAttacking", true);
        animator.SetBool("normalAttack", true);
    }

    // To use in each event in animations attack
    public void TurnOffAnimatorParamAttack()
    {
        animator.SetBool("isAttacking", false);

        if (animator.GetBool("normalAttack"))
        {
            animator.SetBool("normalAttack", false);
        }

        if (attacking)
        {
            attacking = false;
        }
    }
}
