/* Combat_Control.cs
Utilisé pour gérer les combats du joueur
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Stats_Control))]
[RequireComponent(typeof(Collider2D))]
public class Player_Combat_Control : MonoBehaviour
{

    [SerializeField] Transform firePoint;
    [SerializeField] GameObject projectile;

    Animator animator;
    Stats_Control currentStats;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentStats = GetComponent<Stats_Control>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !animator.GetBool("isAttacking"))
        {
            UseNormalAttack();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !animator.GetBool("isAttacking"))
        {
            UseBowAttack();
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && !animator.GetBool("isAttacking"))
            {
                UseNormalAttack();
                if (collision.gameObject.GetComponent<AI_Health>())
                {
                    AI_Health enemyHealth = collision.gameObject.GetComponent<AI_Health>();
                    enemyHealth.GetDamage(currentStats.GetCurrentAttackDamage());
                }
            }
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
        GameObject _projectile = Instantiate(projectile, firePoint.position, firePoint.rotation);
        _projectile.GetComponent<Rigidbody2D>().velocity = firePoint.up * currentStats.GetProjectileSpeed();
        _projectile.GetComponent<Projectile>().projectileDamage = currentStats.GetCurrentRangedAttackDamage();
    }

    // Use for set firepoint rotation depending of player's movement (as animation's event)
    public void SetFirePointRotation(float newRotation)
    {
        firePoint.rotation = Quaternion.Euler(new Vector3(0f, 0f, newRotation));
    }
}
