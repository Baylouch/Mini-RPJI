/* Combat_Control.cs
Utilisé pour gérer les combats du joueur
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType { Normal, Frost, Fire };

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Player_Combat_Control : MonoBehaviour
{
    public bool isInCombat = false; // Set in AI_Movement_Control
    public bool endingCombat = false; // Same as isInCombat
    [SerializeField] float timerBeforeEndCombat = 5f;
    float currentTimerBeforeEndCombat;

    [SerializeField] Transform firePoint;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is in combat
        if (isInCombat)
        {
            if (endingCombat) // If combat is ending start ending timer
            {
                if (currentTimerBeforeEndCombat > 0f)
                {
                    currentTimerBeforeEndCombat -= Time.deltaTime;
                }
                else
                {
                    isInCombat = false;
                    currentTimerBeforeEndCombat = timerBeforeEndCombat;
                    endingCombat = false;
                }
            }
        }

        if (!animator.GetBool("isAttacking"))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) // To centralise in Player_Input.cs later
            {
                UseNormalAttack();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) // To centralise in Player_Input.cs later
            {
                if (Player_Inventory.inventory_instance.GetCurrentBow())
                    UseBowAttack();
                else
                    Debug.Log("No bow equiped");
            }
        }
    }

    void UseNormalAttack()
    {
        animator.SetBool("isAttacking", true);
        animator.SetBool("normalAttack", true);
    }

    void UseBowAttack()
    {
        animator.SetBool("isAttacking", true);
        animator.SetBool("bowAttack", true);
    }

    private void OnTriggerStay2D(Collider2D collision) // Find other way later
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && !animator.GetBool("isAttacking")) // To centralise in Player_Input.cs later
            {
                UseNormalAttack();
                if (collision.gameObject.GetComponent<AI_Health>())
                {
                    AI_Health enemyHealth = collision.gameObject.GetComponent<AI_Health>();
                    enemyHealth.GetDamage(GetAttackDamage());

                    // If we kill the enemy
                    if (enemyHealth.IsDead())
                    {
                        // Check if npc got AI_Stats on him
                        AI_Stats enemyStats = collision.gameObject.GetComponent<AI_Stats>();
                        if (enemyStats)
                        {
                            // If yes add experience to player (to securise later Stats_Control)
                            Player_Stats.stats_instance.AddExperience(enemyStats.GetExperienceGain());
                        }
                    }
                }
            }
        }
    }

    int GetAttackDamage()
    {
        float tempCritCondition = Random.Range(0, 100);
        if (tempCritCondition <= Player_Stats.stats_instance.getCriticalRate()) // Do critical strike
        {
            int criticalAttack = Mathf.RoundToInt((Random.Range(Player_Stats.stats_instance.getCurrentMinDamage(), Player_Stats.stats_instance.getCurrentMaxDamage()) * 1.5f));
            return criticalAttack;
        }
        else
        {
            int currAttack = (Random.Range(Player_Stats.stats_instance.getCurrentMinDamage(), Player_Stats.stats_instance.getCurrentMaxDamage()));
            return currAttack;
        }

    }

    int GetRangedAttackDamage()
    {
        float tempCritCondition = Random.Range(0, 100);
        if (tempCritCondition <= Player_Stats.stats_instance.getRangedCriticalRate())
        {
            int criticalRangedAttack = Mathf.RoundToInt((Random.Range(Player_Stats.stats_instance.getCurrentRangedMinDamage(), Player_Stats.stats_instance.getCurrentRangedMaxDamage()) * 1.5f));
            return criticalRangedAttack;
        }
        else
        {
            int currRangedattack = (Random.Range(Player_Stats.stats_instance.getCurrentRangedMinDamage(), Player_Stats.stats_instance.getCurrentRangedMaxDamage()));
            return currRangedattack;
        }
    }

    // To use in each event in animations attack
    public void TurnOffAnimatorParamAttack()
    {
        animator.SetBool("isAttacking", false);

        if (animator.GetBool("normalAttack"))
        {
            animator.SetBool("normalAttack", false);
        }
        if (animator.GetBool("bowAttack"))
        {
            animator.SetBool("bowAttack", false);
        }
    }

    // used in each bow attack for launch arrow
    public void Shoot()
    {
        if (Player_Inventory.inventory_instance.GetCurrentBow()) // IF player got equiped bow
        {
            GameObject _projectile = Instantiate(Player_Inventory.inventory_instance.GetCurrentBow().projectile, firePoint.position, firePoint.rotation);
            Projectile currentProjectileComponent = _projectile.GetComponent<Projectile>();

            if (currentProjectileComponent == null)
                return; // There is a bug here or a miss by game master. Every projectile must have Projectile.cs attach

            currentProjectileComponent.projectileDamage = GetRangedAttackDamage();
        }
    }

    // Use for set firepoint rotation depending of player's movement (as animation's event)
    public void SetFirePointRotation(float newRotation)
    {
        firePoint.rotation = Quaternion.Euler(new Vector3(0f, 0f, newRotation));
    }

    public void ResetEndingCombatTimer()
    {
        currentTimerBeforeEndCombat = timerBeforeEndCombat;
    }
}
