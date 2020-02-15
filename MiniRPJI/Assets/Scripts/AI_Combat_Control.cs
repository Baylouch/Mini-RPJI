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

    Animator animator;
    Stats_Control currentStats;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentStats = GetComponent<Stats_Control>();
        currentTimerBeforeAttack = timerBeforeAttack;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTimerBeforeAttack > 0f)
        {
            currentTimerBeforeAttack -= Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (currentTimerBeforeAttack <= 0f && !animator.GetBool("isAttacking"))
            {
                UseAttack();
                if (collision.gameObject.GetComponent<Health>())
                {
                    Health enemyHealth = collision.gameObject.GetComponent<Health>();
                    enemyHealth.GetDamage(currentStats.GetCurrentAttackDamage());
                    currentTimerBeforeAttack = timerBeforeAttack;
                }
            }
        }
    }

    void UseAttack()
    {
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
    }
}
